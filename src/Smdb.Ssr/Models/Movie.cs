namespace Smdb.Ssr.Models
{
	public class Movie
	{
		public int Id { get; set; }
		public string Title { get; set; } = "";
		public int Year { get; set; }

		public List<Actor> Actors { get; set; } = new();
	}
}
