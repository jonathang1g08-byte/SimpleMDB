namespace Smdb.Core.ActorMovie;

public class ActorMovie
{
	public int Id { get; set; }
	public int ActorId { get; set; }
	public int MovieId { get; set; }
	public string Role { get; set; }

	public ActorMovie()
	{
		Id = -1;
		ActorId = -1;
		MovieId = -1;
		Role = string.Empty;
	}

	public ActorMovie(int id, int actorId, int movieId, string role)
	{
		Id = id;
		ActorId = actorId;
		MovieId = movieId;
		Role = role;
	}
}