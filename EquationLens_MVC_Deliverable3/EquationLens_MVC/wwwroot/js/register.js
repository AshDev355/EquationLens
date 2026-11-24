// AJAX check: is the email already registered?
(function () {
  const input = document.querySelector('[data-email-check]');
  const status = document.getElementById('emailStatus');
  if (!input || !status) return;
  let timer;

  input.addEventListener('input', () => {
    clearTimeout(timer);
    const email = input.value.trim();
    if (!email.includes('@')) { status.textContent = ''; return; }
    timer = setTimeout(async () => {
      const res = await fetch(`${input.dataset.url}?email=${encodeURIComponent(email)}`);
      if (!res.ok) return;
      const data = await res.json();
      status.textContent = data.message;
      status.style.color = data.available ? 'var(--yale)' : '#8A2B1D';
    }, 400);
  });
})();
