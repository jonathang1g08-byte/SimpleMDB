using Smdb.Ssr.Models;

namespace Smdb.Ssr.Data
{
	public interface IActorSsrRepository
	{
		List<Actor> ReadAll();
		Actor? Read(int id);
		void Create(Actor actor);
		void Update(Actor actor);
		void Delete(int id);
	}
}
