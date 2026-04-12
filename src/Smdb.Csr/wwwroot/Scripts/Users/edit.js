import { $, apiFetch, renderStatus, getQueryParam, captureUserForm } from '/scripts/common.js';
(async function initUserEdit() {
	const id = getQueryParam('id');
	const form = $('#form');
	const statusEl = $('#status');
	if (!id) {
		renderStatus(statusEl, 'err', 'Missing ?id in URL.');
		form.querySelectorAll('input,textarea,button,select').forEach(el => el.disabled = true);
		return;
	}
	try {
		const u = await apiFetch(`/users/${encodeURIComponent(id)}`);
		form.username.value = u.username ?? '';
		form.email.value = u.email ?? '';
		form.passwordHash.value = u.passwordHash ?? '';
		renderStatus(statusEl, 'ok', 'Loaded user. You can edit and save.');
	} catch (err) {
		renderStatus(statusEl, 'err', `Failed to load data: ${err.message}`);
		return;
	}
	form.addEventListener('submit', async (ev) => {
		ev.preventDefault();
		const payload = captureUserForm(form);
		try {
			const updated = await apiFetch(`/users/${encodeURIComponent(id)}`, {
				method: 'PUT',
				body: JSON.stringify(payload),
			});
			renderStatus(statusEl, 'ok', `Updated user #${updated.id} "${updated.username}".`);
		} catch (err) {
			renderStatus(statusEl, 'err', `Update failed: ${err.message}`);
		}
	});
})();
