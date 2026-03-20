namespace Smdb.Core.ActorMovie;

using Shared.Http;

public interface IActorsMovieRepository
{
	public Task<PagedResult<ActorMovie>?> ReadActorMovies(int page, int size);
	public Task<ActorMovie?> CreateActorMovie(ActorMovie newEntry);
	public Task<ActorMovie?> ReadActorMovie(int id);
	public Task<ActorMovie?> UpdateActorMovie(int id, ActorMovie newData);
	public Task<ActorMovie?> DeleteActorMovie(int id);
}