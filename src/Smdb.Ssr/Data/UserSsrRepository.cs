using Smdb.Ssr.Models;

namespace Smdb.Ssr.Data
{
	public class UserSsrRepository : IUserSsrRepository
	{
		private readonly List<User> _users = new();
		private int _nextId = 1;

		public UserSsrRepository()
		{
			// Instructor-style mock usernames
			string[] usernames = new string[]
			{
								"Papo", "Pepo", "Popo", "Pipo",
								"Momo", "Momas", "Mama",
								"Lalo", "Lola", "Lala", "Lilo"
			};

			foreach (var username in usernames)
			{
				var user = new User
				{
					Id = _nextId++,
					Name = username,
					Email = $"{username.ToLower()}@example.com"
				};

				_users.Add(user);
			}
		}

		public List<User> ReadAll() => _users;

		public User? Read(int id) =>
				_users.FirstOrDefault(u => u.Id == id);

		public void Create(User user)
		{
			user.Id = _nextId++;
			_users.Add(user);
		}

		public void Update(User user)
		{
			var existing = Read(user.Id);
			if (existing == null) return;

			existing.Name = user.Name;
			existing.Email = user.Email;
		}

		public void Delete(int id)
		{
			var user = Read(id);
			if (user != null)
				_users.Remove(user);
		}
	}
}
