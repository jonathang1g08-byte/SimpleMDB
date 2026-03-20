namespace Smdb.Core.ActorMovie;

using Shared.Http;
using System.Net;

public class ActorsMovieService : IActorsMovieService
{
	private readonly IActorsMovieRepository repository;

	public ActorsMovieService(IActorsMovieRepository repository)
	{
		this.repository = repository;
	}

	public async Task<Result<PagedResult<ActorMovie>>> ReadActorMovies(int page, int size)
	{
		if (page < 1)
			return new Result<PagedResult<ActorMovie>>(new Exception("Page must be >= 1."), (int)HttpStatusCode.BadRequest);
		if (size < 1)
			return new Result<PagedResult<ActorMovie>>(new Exception("Page size must be >= 1."), (int)HttpStatusCode.BadRequest);

		var paged = await repository.ReadActorMovies(page, size);
		return paged == null
			? new Result<PagedResult<ActorMovie>>(new Exception($"Could not read actor-movies from page {page} size {size}."), (int)HttpStatusCode.NotFound)
			: new Result<PagedResult<ActorMovie>>(paged, (int)HttpStatusCode.OK);
	}

	public async Task<Result<ActorMovie>> CreateActorMovie(ActorMovie entry)
	{
		var validation = ValidateActorMovie(entry);
		if (validation != null) return validation;

		var created = await repository.CreateActorMovie(entry);
		return created == null
			? new Result<ActorMovie>(new Exception("Could not create actor-movie association."), (int)HttpStatusCode.NotFound)
			: new Result<ActorMovie>(created, (int)HttpStatusCode.Created);
	}

	public async Task<Result<ActorMovie>> ReadActorMovie(int id)
	{
		var item = await repository.ReadActorMovie(id);
		return item == null
			? new Result<ActorMovie>(new Exception($"Could not read actor-movie with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<ActorMovie>(item, (int)HttpStatusCode.OK);
	}

	public async Task<Result<ActorMovie>> UpdateActorMovie(int id, ActorMovie newData)
	{
		var validation = ValidateActorMovie(newData);
		if (validation != null) return validation;

		var updated = await repository.UpdateActorMovie(id, newData);
		return updated == null
			? new Result<ActorMovie>(new Exception($"Could not update actor-movie with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<ActorMovie>(updated, (int)HttpStatusCode.OK);
	}

	public async Task<Result<ActorMovie>> DeleteActorMovie(int id)
	{
		var deleted = await repository.DeleteActorMovie(id);
		return deleted == null
			? new Result<ActorMovie>(new Exception($"Could not delete actor-movie with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<ActorMovie>(deleted, (int)HttpStatusCode.OK);
	}

	private static Result<ActorMovie>? ValidateActorMovie(ActorMovie? model)
	{
		if (model is null)
			return new Result<ActorMovie>(new Exception("ActorMovie payload is required."), (int)HttpStatusCode.BadRequest);
		if (model.ActorId < 1 || model.MovieId < 1)
			return new Result<ActorMovie>(new Exception("ActorId and MovieId must be positive."), (int)HttpStatusCode.BadRequest);
		if (string.IsNullOrWhiteSpace(model.Role))
			return new Result<ActorMovie>(new Exception("Role is required."), (int)HttpStatusCode.BadRequest);
		return null;
	}
}