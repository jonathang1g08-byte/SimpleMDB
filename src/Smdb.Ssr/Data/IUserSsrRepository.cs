using Smdb.Ssr.Models;

namespace Smdb.Ssr.Data
{
	public interface IUserSsrRepository
	{
		List<User> ReadAll();
		User? Read(int id);
		void Create(User user);
		void Update(User user);
		void Delete(int id);
	}
}
