import { $, apiFetch, renderStatus, getQueryParam } from '/scripts/common.js';
(async function initActorMovieView() {
	const id = getQueryParam('id');
	const statusEl = $('#status');
	if (!id) return renderStatus(statusEl, 'err', 'Missing ?id in URL.');
	try {
		const am = await apiFetch(`/actormovie/${encodeURIComponent(id)}`);
		$('#title').textContent = am.role;
		const details = $('#credit-details');
		details.innerHTML = `
			<p><strong>ID:</strong> ${am.id}</p>
			<p><strong>Role:</strong> ${am.role}</p>
			<p><strong>Actor ID:</strong> ${am.actorId}</p>
			<p><strong>Movie ID:</strong> ${am.movieId}</p>
		`;
		$('#edit-link').href = `/ActorMovie/edit.html?id=${encodeURIComponent(am.id)}`;
		$('#delete-btn').dataset.id = am.id;
		$('#delete-btn').addEventListener('click', async () => {
			if (!confirm('Delete this credit? This cannot be undone.')) return;
			try {
				await apiFetch(`/actormovie/${encodeURIComponent(am.id)}`, { method: 'DELETE' });
				renderStatus(statusEl, 'ok', 'Credit deleted.');
				setTimeout(() => window.location.href = '/ActorMovie/index.html', 2000);
			} catch (err) {
				renderStatus(statusEl, 'err', `Delete failed: ${err.message}`);
			}
		});
		renderStatus(statusEl, 'ok', 'Credit loaded successfully.');
	} catch (err) {
		renderStatus(statusEl, 'err', `Failed to load credit ${id}: ${err.message}`);
	}
})();
