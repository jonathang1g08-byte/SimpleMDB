namespace Smdb.Api.ActorMovie;

using Shared.Http;

public class ActorsMovieRouter : HttpRouter
{
	public ActorsMovieRouter(ActorsMovieApiController apiController, ActorsMovieSsrController ssrController)
	{
		UseParametrizedRouteMatching();
		MapGet("/", apiController.ReadActorMovies);
		MapPost("/", HttpUtils.ReadRequestBodyAsText, apiController.CreateActorMovie);
		MapGet("/:id", apiController.ReadActorMovie);
		MapPut("/:id", HttpUtils.ReadRequestBodyAsText, apiController.UpdateActorMovie);
		MapDelete("/:id", apiController.DeleteActorMovie);
		MapGet("/ssr", ssrController.RenderActorMovies);
	}
}