using Smdb.Ssr.Models;

namespace Smdb.Ssr.Data
{
	public interface IMovieSsrRepository
	{
		List<Movie> ReadAll();
		Movie? Read(int id);
		void Create(Movie movie, int[] actorIds);
		void Update(Movie movie, int[] actorIds);
		void Delete(int id);
	}
}
