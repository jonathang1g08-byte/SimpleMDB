namespace Smdb.Core.ActorMovie;

using Shared.Http;
using Smdb.Core.Db;

public class ActorsMovieRepository : IActorsMovieRepository
{
	private readonly MemoryDatabase db;

	public ActorsMovieRepository(MemoryDatabase db)
	{
		this.db = db;
	}

	public async Task<PagedResult<ActorMovie>?> ReadActorMovies(int page, int size)
	{
		int totalCount = db.ActorMovies.Count;
		int start = Math.Clamp((page - 1) * size, 0, totalCount);
		int length = Math.Clamp(size, 0, totalCount - start);
		var values = db.ActorMovies.Slice(start, length);
		var result = new PagedResult<ActorMovie>(totalCount, values);
		return await Task.FromResult(result);
	}

	public async Task<ActorMovie?> CreateActorMovie(ActorMovie newEntry)
	{
		newEntry.Id = db.NextActorMovieId();
		db.ActorMovies.Add(newEntry);
		return await Task.FromResult(newEntry);
	}

	public async Task<ActorMovie?> ReadActorMovie(int id)
	{
		var result = db.ActorMovies.FirstOrDefault(x => x.Id == id);
		return await Task.FromResult(result);
	}

	public async Task<ActorMovie?> UpdateActorMovie(int id, ActorMovie newData)
	{
		var existing = db.ActorMovies.FirstOrDefault(x => x.Id == id);
		if (existing != null)
		{
			existing.ActorId = newData.ActorId;
			existing.MovieId = newData.MovieId;
			existing.Role = newData.Role;
		}
		return await Task.FromResult(existing);
	}

	public async Task<ActorMovie?> DeleteActorMovie(int id)
	{
		var existing = db.ActorMovies.FirstOrDefault(x => x.Id == id);
		if (existing != null)
		{
			db.ActorMovies.Remove(existing);
		}
		return await Task.FromResult(existing);
	}
}