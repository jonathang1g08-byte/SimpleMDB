import { $, apiFetch, renderStatus, captureActorForm } from '/scripts/common.js';
(async function initActorAdd() {
	const form = $('#form');
	const statusEl = $('#status');
	renderStatus(statusEl, 'ok', 'New actor. You can edit and save.');
	form.addEventListener('submit', async (ev) => {
		ev.preventDefault();
		const payload = captureActorForm(form);
		try {
			const created = await apiFetch('/actors', { method: 'POST', body: JSON.stringify(payload) });
			renderStatus(statusEl, 'ok', `Created actor #${created.id} "${created.name}" (${created.birthYear}).`);
			form.reset();
		} catch (err) {
			renderStatus(statusEl, 'err', `Create failed: ${err.message}`);
		}
	});
})();
