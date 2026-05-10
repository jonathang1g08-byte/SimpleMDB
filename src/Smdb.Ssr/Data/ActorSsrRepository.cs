using Smdb.Ssr.Models;

namespace Smdb.Ssr.Data
{
	public class ActorSsrRepository : IActorSsrRepository
	{
		public ActorSsrRepository()
		{
			_actors.Add(new Actor { Id = 1, Name = "Keanu Reeves", BirthYear = 1964 });
			_actors.Add(new Actor { Id = 2, Name = "Tom Cruise", BirthYear = 1962 });
			_actors.Add(new Actor { Id = 3, Name = "Robert Downey Jr.", BirthYear = 1965 });
			_actors.Add(new Actor { Id = 4, Name = "Scarlett Johansson", BirthYear = 1984 });
			_actors.Add(new Actor { Id = 5, Name = "Johnny Depp", BirthYear = 1963 });
			_actors.Add(new Actor { Id = 6, Name = "Emma Stone", BirthYear = 1988 });
			_actors.Add(new Actor { Id = 7, Name = "Ryan Reynolds", BirthYear = 1976 });
			_actors.Add(new Actor { Id = 8, Name = "Gal Gadot", BirthYear = 1985 });
			_actors.Add(new Actor { Id = 9, Name = "Christian Bale", BirthYear = 1974 });
			_actors.Add(new Actor { Id = 10, Name = "Harrison Ford", BirthYear = 1942 });
		}

		private readonly List<Actor> _actors = new();
		private int _nextId = 1;

		public List<Actor> ReadAll() => _actors;

		public Actor? Read(int id) =>
				_actors.FirstOrDefault(a => a.Id == id);

		public void Create(Actor actor)
		{
			actor.Id = _nextId++;
			_actors.Add(actor);
		}

		public void Update(Actor actor)
		{
			var existing = Read(actor.Id);
			if (existing == null) return;

			existing.Name = actor.Name;
			existing.BirthYear = actor.BirthYear;
		}

		public void Delete(int id)
		{
			var actor = Read(id);
			if (actor != null)
				_actors.Remove(actor);
		}
	}
}

