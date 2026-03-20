namespace Smdb.Core.Users;

using Shared.Http;

public interface IUsersRepository
{
	public Task<PagedResult<User>?> ReadUsers(int page, int size);
	public Task<User?> CreateUser(User newUser);
	public Task<User?> ReadUser(int id);
	public Task<User?> ReadUserByUsername(string username);
	public Task<User?> UpdateUser(int id, User newData);
	public Task<User?> DeleteUser(int id);
}