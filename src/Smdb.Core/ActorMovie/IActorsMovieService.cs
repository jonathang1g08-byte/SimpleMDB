namespace Smdb.Core.ActorMovie;

using Shared.Http;

public interface IActorsMovieService
{
	public Task<Result<PagedResult<ActorMovie>>> ReadActorMovies(int page, int size);
	public Task<Result<ActorMovie>> CreateActorMovie(ActorMovie entry);
	public Task<Result<ActorMovie>> ReadActorMovie(int id);
	public Task<Result<ActorMovie>> UpdateActorMovie(int id, ActorMovie newData);
	public Task<Result<ActorMovie>> DeleteActorMovie(int id);
}