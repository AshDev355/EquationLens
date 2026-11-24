// AJAX behavior for the workspace page: recognize, solve, history. No full page reloads.
(function () {
  const root = document.getElementById('workspace');
  if (!root) return;

  const token = root.querySelector('input[name="__RequestVerificationToken"]').value;
  const urls = {
    recognize: root.dataset.recognizeUrl, solve: root.dataset.solveUrl, history: root.dataset.historyUrl,
    steps: root.dataset.stepsUrl, remove: root.dataset.deleteUrl, clear: root.dataset.clearUrl
  };
  const $ = id => document.getElementById(id);
  const input = $('equation'), source = $('source'), preview = $('previewImg'), fileInput = $('fileInput'),
        dropzone = $('dropzone'), note = $('recognizeNote'), solveBtn = $('solveBtn'),
        stepsList = $('solutionSteps'), message = $('solutionMessage'), history = $('historyList'), clearBtn = $('clearBtn');

  // POST helper: sends the anti-forgery token in a header and returns { ok, data }.
  async function post(url, formData) {
    const res = await fetch(url, { method: 'POST', headers: { 'RequestVerificationToken': token }, body: formData });
    let data = {};
    try { data = await res.json(); } catch (e) { /* empty or non-JSON response */ }
    return { ok: res.ok, data };
  }

  async function refreshHistory() {
    const res = await fetch(urls.history);
    if (res.ok) history.innerHTML = await res.text();
  }

  // ---- 1. Upload and recognize ----
  async function handleFile(file) {
    if (!file) return;
    if (!['image/png', 'image/jpeg'].includes(file.type)) { note.textContent = 'Only PNG or JPG images are supported.'; return; }
    if (file.size > 5 * 1024 * 1024) { note.textContent = 'The image must be 5 MB or smaller.'; return; }

    preview.src = URL.createObjectURL(file);
    note.textContent = 'Reading the image...';
    const form = new FormData();
    form.append('file', file);
    const { ok, data } = await post(urls.recognize, form);
    if (!ok) { note.textContent = data.error || 'Could not read the image.'; return; }
    input.value = data.latex;
    source.value = 'image';
    note.textContent = data.note;
  }
  fileInput.addEventListener('change', () => handleFile(fileInput.files[0]));
  dropzone.addEventListener('dragover', e => e.preventDefault());
  dropzone.addEventListener('drop', e => { e.preventDefault(); handleFile(e.dataTransfer.files[0]); });
  input.addEventListener('input', () => { source.value = 'typed'; });

  // ---- 2. Solve ----
  async function solve() {
    stepsList.replaceChildren();
    message.textContent = 'Solving...';
    solveBtn.disabled = true;
    const form = new FormData();
    form.append('equation', input.value);
    form.append('source', source.value);
    const { ok, data } = await post(urls.solve, form);
    solveBtn.disabled = false;

    if (!ok) { message.textContent = data.error || 'Something went wrong. Try again.'; return; }
    message.textContent = '';
    data.steps.forEach((text, i) => {
      const li = document.createElement('li');
      li.textContent = `(${i + 1})  ${text}`;
      if (i === data.steps.length - 1) li.className = 'answer';
      stepsList.appendChild(li);
    });
    await refreshHistory();
  }
  solveBtn.addEventListener('click', solve);
  input.addEventListener('keydown', e => { if (e.key === 'Enter') { e.preventDefault(); solve(); } });

  // ---- 3. History: show steps, delete, clear ----
  history.addEventListener('click', async e => {
    const stepsBtn = e.target.closest('[data-steps-id]');
    if (stepsBtn) {
      const list = $('steps-' + stepsBtn.dataset.stepsId);
      if (!list.hidden) { list.hidden = true; return; }
      const res = await fetch(`${urls.steps}?id=${stepsBtn.dataset.stepsId}`);
      const steps = res.ok ? await res.json() : [];
      list.replaceChildren(...steps.map((text, i) => { const li = document.createElement('li'); li.textContent = `(${i + 1})  ${text}`; return li; }));
      list.hidden = false;
      return;
    }
    const delBtn = e.target.closest('[data-delete-id]');
    if (delBtn) {
      const form = new FormData();
      form.append('id', delBtn.dataset.deleteId);
      const { ok } = await post(urls.remove, form);
      if (ok) await refreshHistory();
    }
  });

  clearBtn.addEventListener('click', async () => {
    if (!confirm('Delete all saved equations?')) return;
    const { ok } = await post(urls.clear, new FormData());
    if (ok) await refreshHistory();
  });
})();
