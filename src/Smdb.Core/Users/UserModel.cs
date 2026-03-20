namespace Smdb.Core.Users;

public class User
{
	public int Id { get; set; }
	public string Username { get; set; }
	public string Email { get; set; }
	public string PasswordHash { get; set; }
	public DateTime CreatedAt { get; set; }

	public User()
	{
		Id = -1;
		Username = string.Empty;
		Email = string.Empty;
		PasswordHash = string.Empty;
		CreatedAt = DateTime.UtcNow;
	}

	public User(int id, string username, string email, string passwordHash)
	{
		Id = id;
		Username = username;
		Email = email;
		PasswordHash = passwordHash;
		CreatedAt = DateTime.UtcNow;
	}
}
