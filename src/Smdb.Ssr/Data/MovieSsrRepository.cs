using Smdb.Ssr.Models;

namespace Smdb.Ssr.Data
{
	public class MovieSsrRepository : IMovieSsrRepository
	{
		private readonly List<Movie> _movies = new();
		private readonly IActorSsrRepository _actorRepo;

		private readonly List<ActorMovie> _actorMovies = new();

		private int _nextId = 1;

		public MovieSsrRepository(IActorSsrRepository actorRepo)
		{
			_actorRepo = actorRepo;

			// Movies
			_movies.Add(new Movie { Id = 1, Title = "The Matrix", Year = 1999 });
			_movies.Add(new Movie { Id = 2, Title = "Mission: Impossible", Year = 1996 });
			_movies.Add(new Movie { Id = 3, Title = "Iron Man", Year = 2008 });
			_movies.Add(new Movie { Id = 4, Title = "Black Widow", Year = 2021 });
			_movies.Add(new Movie { Id = 5, Title = "John Wick", Year = 2014 });
			_movies.Add(new Movie { Id = 6, Title = "Pirates of the Caribbean", Year = 2003 });
			_movies.Add(new Movie { Id = 7, Title = "La La Land", Year = 2016 });
			_movies.Add(new Movie { Id = 8, Title = "Deadpool", Year = 2016 });
			_movies.Add(new Movie { Id = 9, Title = "Wonder Woman", Year = 2017 });
			_movies.Add(new Movie { Id = 10, Title = "Blade Runner", Year = 1982 });

			// ActorMovie join table (1 actor per movie)
			_actorMovies.Add(new ActorMovie { MovieId = 1, ActorId = 1 });  // Keanu Reeves
			_actorMovies.Add(new ActorMovie { MovieId = 2, ActorId = 2 });  // Tom Cruise
			_actorMovies.Add(new ActorMovie { MovieId = 3, ActorId = 3 });  // Robert Downey Jr.
			_actorMovies.Add(new ActorMovie { MovieId = 4, ActorId = 4 });  // Scarlett Johansson
			_actorMovies.Add(new ActorMovie { MovieId = 5, ActorId = 1 });  // Keanu Reeves
			_actorMovies.Add(new ActorMovie { MovieId = 6, ActorId = 5 });  // Johnny Depp
			_actorMovies.Add(new ActorMovie { MovieId = 7, ActorId = 6 });  // Emma Stone
			_actorMovies.Add(new ActorMovie { MovieId = 8, ActorId = 7 });  // Ryan Reynolds
			_actorMovies.Add(new ActorMovie { MovieId = 9, ActorId = 8 });  // Gal Gadot
			_actorMovies.Add(new ActorMovie { MovieId = 10, ActorId = 9 }); // Christian Bale
		}


		public List<Movie> ReadAll()
		{
			foreach (var movie in _movies)
			{
				var actorIds = _actorMovies
						.Where(am => am.MovieId == movie.Id)
						.Select(am => am.ActorId)
						.ToList();

				movie.Actors = _actorRepo.ReadAll()
						.Where(a => actorIds.Contains(a.Id))
						.ToList();
			}

			return _movies;
		}

		public Movie? Read(int id)
		{
			var movie = _movies.FirstOrDefault(m => m.Id == id);
			if (movie == null) return null;

			var actorIds = _actorMovies
					.Where(am => am.MovieId == id)
					.Select(am => am.ActorId)
					.ToList();

			movie.Actors = _actorRepo.ReadAll()
					.Where(a => actorIds.Contains(a.Id))
					.ToList();

			return movie;
		}


		public void Create(Movie movie, int[] actorIds)
		{
			movie.Id = _movies.Count == 0 ? 1 : _movies.Max(m => m.Id) + 1;
			_movies.Add(movie);

			foreach (var actorId in actorIds)
			{
				_actorMovies.Add(new ActorMovie
				{
					ActorId = actorId,
					MovieId = movie.Id
				});
			}
		}


		public void Update(Movie movie, int[] actorIds)
		{
			var existing = _movies.FirstOrDefault(m => m.Id == movie.Id);
			if (existing == null) return;

			existing.Title = movie.Title;
			existing.Year = movie.Year;

			// Remove old links
			_actorMovies.RemoveAll(am => am.MovieId == movie.Id);

			// Add new links
			foreach (var actorId in actorIds)
			{
				_actorMovies.Add(new ActorMovie
				{
					ActorId = actorId,
					MovieId = movie.Id
				});
			}
		}


		public void Delete(int id)
		{
			_movies.RemoveAll(m => m.Id == id);
			_actorMovies.RemoveAll(am => am.MovieId == id);
		}
	}
}