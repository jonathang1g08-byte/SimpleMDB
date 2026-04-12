import { $, apiFetch, renderStatus, getQueryParam } from '/scripts/common.js';
(async function initActorView() {
	const id = getQueryParam('id');
	const statusEl = $('#status');
	if (!id) return renderStatus(statusEl, 'err', 'Missing ?id in URL.');
	try {
		const a = await apiFetch(`/actors/${encodeURIComponent(id)}`);
		$('#title').textContent = a.name;
		const details = $('#actor-details');
		details.innerHTML = `
			<p><strong>ID:</strong> ${a.id}</p>
			<p><strong>Name:</strong> ${a.name}</p>
			<p><strong>Birth Year:</strong> ${a.birthYear}</p>
			<p><strong>Biography:</strong> ${a.biography || '—'}</p>
		`;
		$('#edit-link').href = `/Actors/edit.html?id=${encodeURIComponent(a.id)}`;
		$('#delete-btn').dataset.id = a.id;
		$('#delete-btn').addEventListener('click', async () => {
			if (!confirm('Delete this actor? This cannot be undone.')) return;
			try {
				await apiFetch(`/actors/${encodeURIComponent(a.id)}`, { method: 'DELETE' });
				renderStatus(statusEl, 'ok', 'Actor deleted.');
				setTimeout(() => window.location.href = '/Actors/', 2000);
			} catch (err) {
				renderStatus(statusEl, 'err', `Delete failed: ${err.message}`);
			}
		});
		renderStatus(statusEl, 'ok', 'Actor loaded successfully.');
	} catch (err) {
		renderStatus(statusEl, 'err', `Failed to load actor ${id}: ${err.message}`);
	}
})();
