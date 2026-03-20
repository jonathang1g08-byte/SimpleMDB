namespace Smdb.Core.Actors;

using Shared.Http;
using System.Net;

public class ActorsService : IActorsService
{
	private readonly IActorsRepository repository;

	public ActorsService(IActorsRepository repository)
	{
		this.repository = repository;
	}

	public async Task<Result<PagedResult<Actor>>> ReadActors(int page, int size)
	{
		if (page < 1)
		{
			return new Result<PagedResult<Actor>>(new Exception("Page must be >= 1."), (int)HttpStatusCode.BadRequest);
		}
		if (size < 1)
		{
			return new Result<PagedResult<Actor>>(new Exception("Page size must be >= 1."), (int)HttpStatusCode.BadRequest);
		}

		var pagedResult = await repository.ReadActors(page, size);
		var result = pagedResult == null
			? new Result<PagedResult<Actor>>(new Exception($"Could not read actors from page {page} and size {size}."), (int)HttpStatusCode.NotFound)
			: new Result<PagedResult<Actor>>(pagedResult, (int)HttpStatusCode.OK);
		return result;
	}

	public async Task<Result<Actor>> CreateActor(Actor actor)
	{
		var validationError = ValidateActor(actor);
		if (validationError != null) { return validationError; }

		var created = await repository.CreateActor(actor);
		var result = created == null
			? new Result<Actor>(new Exception($"Could not create actor {actor}."), (int)HttpStatusCode.NotFound)
			: new Result<Actor>(created, (int)HttpStatusCode.Created);
		return result;
	}

	public async Task<Result<Actor>> ReadActor(int id)
	{
		var actor = await repository.ReadActor(id);
		var result = actor == null
			? new Result<Actor>(new Exception($"Could not read actor with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<Actor>(actor, (int)HttpStatusCode.OK);
		return result;
	}

	public async Task<Result<Actor>> UpdateActor(int id, Actor newData)
	{
		var validationError = ValidateActor(newData);
		if (validationError != null) { return validationError; }

		var updated = await repository.UpdateActor(id, newData);
		var result = updated == null
			? new Result<Actor>(new Exception($"Could not update actor {newData.Name} with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<Actor>(updated, (int)HttpStatusCode.OK);
		return result;
	}

	public async Task<Result<Actor>> DeleteActor(int id)
	{
		var deleted = await repository.DeleteActor(id);
		var result = deleted == null
			? new Result<Actor>(new Exception($"Could not delete actor with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<Actor>(deleted, (int)HttpStatusCode.OK);
		return result;
	}

	private static Result<Actor>? ValidateActor(Actor? actorData)
	{
		if (actorData is null)
		{
			return new Result<Actor>(new Exception("Actor payload is required."), (int)HttpStatusCode.BadRequest);
		}
		if (string.IsNullOrWhiteSpace(actorData.Name))
		{
			return new Result<Actor>(new Exception("Name is required and cannot be empty."), (int)HttpStatusCode.BadRequest);
		}
		if (actorData.Name.Length > 256)
		{
			return new Result<Actor>(new Exception("Name cannot be longer than 256 characters."), (int)HttpStatusCode.BadRequest);
		}
		if (actorData.BirthYear < 1800 || actorData.BirthYear > DateTime.UtcNow.Year)
		{
			return new Result<Actor>(new Exception($"BirthYear must be between 1800 and {DateTime.UtcNow.Year}."), (int)HttpStatusCode.BadRequest);
		}
		return null;
	}
}