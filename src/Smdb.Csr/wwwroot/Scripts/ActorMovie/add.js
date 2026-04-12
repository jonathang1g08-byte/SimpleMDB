import { $, apiFetch, renderStatus, captureActorMovieForm } from '/scripts/common.js';
(async function initActorMovieAdd() {
	const form = $('#form');
	const statusEl = $('#status');
	renderStatus(statusEl, 'ok', 'New actor-movie credit. You can edit and save.');
	form.addEventListener('submit', async (ev) => {
		ev.preventDefault();
		const payload = captureActorMovieForm(form);
		try {
			const created = await apiFetch('/actormovie', { method: 'POST', body: JSON.stringify(payload) });
			renderStatus(statusEl, 'ok', `Created credit #${created.id} for role "${created.role}".`);
			form.reset();
		} catch (err) {
			renderStatus(statusEl, 'err', `Create failed: ${err.message}`);
		}
	});
})();
