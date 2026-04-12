import { $, apiFetch, renderStatus, getQueryParam, captureActorMovieForm } from '/scripts/common.js';
(async function initActorMovieEdit() {
	const id = getQueryParam('id');
	const form = $('#form');
	const statusEl = $('#status');
	if (!id) {
		renderStatus(statusEl, 'err', 'Missing ?id in URL.');
		form.querySelectorAll('input,textarea,button,select').forEach(el => el.disabled = true);
		return;
	}
	try {
		const am = await apiFetch(`/actormovie/${encodeURIComponent(id)}`);
		form.actorId.value = am.actorId ?? '';
		form.movieId.value = am.movieId ?? '';
		form.role.value = am.role ?? '';
		renderStatus(statusEl, 'ok', 'Loaded credit. You can edit and save.');
	} catch (err) {
		renderStatus(statusEl, 'err', `Failed to load data: ${err.message}`);
		return;
	}
	form.addEventListener('submit', async (ev) => {
		ev.preventDefault();
		const payload = captureActorMovieForm(form);
		try {
			const updated = await apiFetch(`/actormovie/${encodeURIComponent(id)}`, {
				method: 'PUT',
				body: JSON.stringify(payload),
			});
			renderStatus(statusEl, 'ok', `Updated credit #${updated.id} for role "${updated.role}".`);
		} catch (err) {
			renderStatus(statusEl, 'err', `Update failed: ${err.message}`);
		}
	});
})();
