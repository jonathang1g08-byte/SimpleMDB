namespace Smdb.Core.Movies;

public class Movie
{
	public int Id { get; set; }
	public string Title { get; set; }
	public int Year { get; set; }
	public string Description { get; set; }
	public Movie(int id, string title, int year, string description)
	{
		Id = id;
		Title = title;
		Year = year;
		Description = description;
	}
	public override string ToString()
	{
		return $"Movie[Id={Id}, Title={Title}, Year={Year}, Description={Description}]";
	}
}