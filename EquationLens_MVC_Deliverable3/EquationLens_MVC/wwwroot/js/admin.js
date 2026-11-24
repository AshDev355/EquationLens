// AJAX behavior for the admin page: live user search, paging and activity refresh.
(function () {
  const root = document.getElementById('adminPage');
  if (!root) return;

  const usersUrl = root.dataset.usersUrl, activityUrl = root.dataset.activityUrl;
  const search = document.getElementById('userSearch'), table = document.getElementById('usersTable'),
        activity = document.getElementById('activityList'), refresh = document.getElementById('refreshActivity');
  let timer;

  async function loadUsers(page) {
    const params = new URLSearchParams({ term: search.value, page: String(page) });
    const res = await fetch(`${usersUrl}?${params}`);
    if (res.ok) table.innerHTML = await res.text();
  }

  // Wait until the admin stops typing before searching.
  search.addEventListener('input', () => { clearTimeout(timer); timer = setTimeout(() => loadUsers(1), 300); });

  table.addEventListener('click', e => {
    const btn = e.target.closest('[data-page]');
    if (btn && !btn.disabled) loadUsers(Number(btn.dataset.page));
  });

  refresh.addEventListener('click', async () => {
    const res = await fetch(activityUrl);
    if (res.ok) activity.innerHTML = await res.text();
  });
})();
