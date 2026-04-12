import { $, apiFetch, renderStatus, getQueryParam } from '/scripts/common.js';
(async function initUserView() {
	const id = getQueryParam('id');
	const statusEl = $('#status');
	if (!id) return renderStatus(statusEl, 'err', 'Missing ?id in URL.');
	try {
		const u = await apiFetch(`/users/${encodeURIComponent(id)}`);
		$('#title').textContent = u.username;
		const details = $('#user-details');
		details.innerHTML = `
			<p><strong>ID:</strong> ${u.id}</p>
			<p><strong>Username:</strong> ${u.username}</p>
			<p><strong>Email:</strong> ${u.email}</p>
			<p><strong>Created:</strong> ${new Date(u.createdAt).toLocaleDateString()}</p>
		`;
		$('#edit-link').href = `/Users/edit.html?id=${encodeURIComponent(u.id)}`;
		$('#delete-btn').dataset.id = u.id;
		$('#delete-btn').addEventListener('click', async () => {
			if (!confirm('Delete this user? This cannot be undone.')) return;
			try {
				await apiFetch(`/users/${encodeURIComponent(u.id)}`, { method: 'DELETE' });
				renderStatus(statusEl, 'ok', 'User deleted.');
				setTimeout(() => window.location.href = '/Users/', 2000);
			} catch (err) {
				renderStatus(statusEl, 'err', `Delete failed: ${err.message}`);
			}
		});
		renderStatus(statusEl, 'ok', 'User loaded successfully.');
	} catch (err) {
		renderStatus(statusEl, 'err', `Failed to load user ${id}: ${err.message}`);
	}
})();
