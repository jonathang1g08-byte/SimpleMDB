import { $, apiFetch, renderStatus, captureUserForm } from '/scripts/common.js';
(async function initUserAdd() {
	const form = $('#form');
	const statusEl = $('#status');
	renderStatus(statusEl, 'ok', 'New user. You can edit and save.');
	form.addEventListener('submit', async (ev) => {
		ev.preventDefault();
		const payload = captureUserForm(form);
		try {
			const created = await apiFetch('/api/v1/users', { method: 'POST', body: JSON.stringify(payload) });
			renderStatus(statusEl, 'ok', `Created user #${created.id} "${created.username}".`);
			form.reset();
		} catch (err) {
			renderStatus(statusEl, 'err', `Create failed: ${err.message}`);
		}
	});
})();
