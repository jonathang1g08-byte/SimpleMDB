namespace Smdb.Core.Actors;

public class Actor
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int BirthYear { get; set; }
	public string Biography { get; set; }

	public Actor()
	{
		Id = -1;
		Name = string.Empty;
		BirthYear = 0;
		Biography = string.Empty;
	}

	public Actor(int id, string name, int birthYear, string biography)
	{
		Id = id;
		Name = name;
		BirthYear = birthYear;
		Biography = biography;
	}
}
