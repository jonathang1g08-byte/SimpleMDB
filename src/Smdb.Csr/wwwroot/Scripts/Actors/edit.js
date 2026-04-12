import { $, apiFetch, renderStatus, getQueryParam, captureActorForm } from '/scripts/common.js';
(async function initActorEdit() {
	const id = getQueryParam('id');
	const form = $('#form');
	const statusEl = $('#status');
	if (!id) {
		renderStatus(statusEl, 'err', 'Missing ?id in URL.');
		form.querySelectorAll('input,textarea,button,select').forEach(el => el.disabled = true);
		return;
	}
	try {
		const a = await apiFetch(`/actors/${encodeURIComponent(id)}`);
		form.name.value = a.name ?? '';
		form.birthYear.value = a.birthYear ?? '';
		form.biography.value = a.biography ?? '';
		renderStatus(statusEl, 'ok', 'Loaded actor. You can edit and save.');
	} catch (err) {
		renderStatus(statusEl, 'err', `Failed to load data: ${err.message}`);
		return;
	}
	form.addEventListener('submit', async (ev) => {
		ev.preventDefault();
		const payload = captureActorForm(form);
		try {
			const updated = await apiFetch(`/actors/${encodeURIComponent(id)}`, {
				method: 'PUT',
				body: JSON.stringify(payload),
			});
			renderStatus(statusEl, 'ok', `Updated actor #${updated.id} "${updated.name}" (${updated.birthYear}).`);
		} catch (err) {
			renderStatus(statusEl, 'err', `Update failed: ${err.message}`);
		}
	});
})();
