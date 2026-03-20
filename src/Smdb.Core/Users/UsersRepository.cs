namespace Smdb.Core.Users;

using Shared.Http;
using Smdb.Core.Db;

public class UsersRepository : IUsersRepository
{
	private readonly MemoryDatabase db;

	public UsersRepository(MemoryDatabase db)
	{
		this.db = db;
	}

	public async Task<PagedResult<User>?> ReadUsers(int page, int size)
	{
		int totalCount = db.Users.Count;
		int start = Math.Clamp((page - 1) * size, 0, totalCount);
		int length = Math.Clamp(size, 0, totalCount - start);
		var values = db.Users.Slice(start, length);
		var result = new PagedResult<User>(totalCount, values);
		return await Task.FromResult(result);
	}

	public async Task<User?> CreateUser(User newUser)
	{
		newUser.Id = db.NextUserId();
		db.Users.Add(newUser);
		return await Task.FromResult(newUser);
	}

	public async Task<User?> ReadUser(int id)
	{
		var result = db.Users.FirstOrDefault(u => u.Id == id);
		return await Task.FromResult(result);
	}

	public async Task<User?> ReadUserByUsername(string username)
	{
		var result = db.Users.FirstOrDefault(u => u.Username == username);
		return await Task.FromResult(result);
	}

	public async Task<User?> UpdateUser(int id, User newData)
	{
		var existing = db.Users.FirstOrDefault(u => u.Id == id);
		if (existing != null)
		{
			existing.Username = newData.Username;
			existing.Email = newData.Email;
			existing.PasswordHash = newData.PasswordHash;
		}
		return await Task.FromResult(existing);
	}

	public async Task<User?> DeleteUser(int id)
	{
		var existing = db.Users.FirstOrDefault(u => u.Id == id);
		if (existing != null)
		{
			db.Users.Remove(existing);
		}
		return await Task.FromResult(existing);
	}
}
