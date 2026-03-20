namespace Smdb.Core.Users;

using Shared.Http;
using System.Net;

public class UsersService : IUsersService
{
	private readonly IUsersRepository repository;

	public UsersService(IUsersRepository repository)
	{
		this.repository = repository;
	}

	public async Task<Result<PagedResult<User>>> ReadUsers(int page, int size)
	{
		if (page < 1)
			return new Result<PagedResult<User>>(new Exception("Page must be >= 1."), (int)HttpStatusCode.BadRequest);
		if (size < 1)
			return new Result<PagedResult<User>>(new Exception("Page size must be >= 1."), (int)HttpStatusCode.BadRequest);

		var paged = await repository.ReadUsers(page, size);
		return paged == null
			? new Result<PagedResult<User>>(new Exception($"Could not read users from page {page} size {size}."), (int)HttpStatusCode.NotFound)
			: new Result<PagedResult<User>>(paged, (int)HttpStatusCode.OK);
	}

	public async Task<Result<User>> CreateUser(User user)
	{
		var validation = ValidateUser(user);
		if (validation != null) return validation;

		var created = await repository.CreateUser(user);
		return created == null
			? new Result<User>(new Exception("Could not create user."), (int)HttpStatusCode.NotFound)
			: new Result<User>(created, (int)HttpStatusCode.Created);
	}

	public async Task<Result<User>> ReadUser(int id)
	{
		var user = await repository.ReadUser(id);
		return user == null
			? new Result<User>(new Exception($"Could not read user with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<User>(user, (int)HttpStatusCode.OK);
	}

	public async Task<Result<User>> ReadUserByUsername(string username)
	{
		var user = await repository.ReadUserByUsername(username);
		return user == null
			? new Result<User>(new Exception($"Could not read user with username {username}."), (int)HttpStatusCode.NotFound)
			: new Result<User>(user, (int)HttpStatusCode.OK);
	}

	public async Task<Result<User>> UpdateUser(int id, User newData)
	{
		var validation = ValidateUser(newData);
		if (validation != null) return validation;

		var updated = await repository.UpdateUser(id, newData);
		return updated == null
			? new Result<User>(new Exception($"Could not update user with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<User>(updated, (int)HttpStatusCode.OK);
	}

	public async Task<Result<User>> DeleteUser(int id)
	{
		var deleted = await repository.DeleteUser(id);
		return deleted == null
			? new Result<User>(new Exception($"Could not delete user with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<User>(deleted, (int)HttpStatusCode.OK);
	}

	private static Result<User>? ValidateUser(User? user)
	{
		if (user is null)
			return new Result<User>(new Exception("User payload is required."), (int)HttpStatusCode.BadRequest);
		if (string.IsNullOrWhiteSpace(user.Username))
			return new Result<User>(new Exception("Username is required."), (int)HttpStatusCode.BadRequest);
		if (user.Username.Length > 50)
			return new Result<User>(new Exception("Username cannot be longer than 50 characters."), (int)HttpStatusCode.BadRequest);
		if (string.IsNullOrWhiteSpace(user.Email))
			return new Result<User>(new Exception("Email is required."), (int)HttpStatusCode.BadRequest);
		if (user.Email.Length > 100)
			return new Result<User>(new Exception("Email cannot be longer than 100 characters."), (int)HttpStatusCode.BadRequest);
		if (string.IsNullOrWhiteSpace(user.PasswordHash))
			return new Result<User>(new Exception("PasswordHash is required."), (int)HttpStatusCode.BadRequest);
		return null;
	}
}
