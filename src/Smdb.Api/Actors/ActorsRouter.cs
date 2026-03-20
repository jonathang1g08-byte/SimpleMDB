namespace Smdb.Api.Actors;

using Shared.Http;

public class ActorsRouter : HttpRouter
{
	public ActorsRouter(ActorsApiController apiController, ActorsSsrController ssrController)
	{
		UseParametrizedRouteMatching();
		MapGet("/", apiController.ReadActors);
		MapPost("/", HttpUtils.ReadRequestBodyAsText, apiController.CreateActor);
		MapGet("/:id", apiController.ReadActor);
		MapPut("/:id", HttpUtils.ReadRequestBodyAsText, apiController.UpdateActor);
		MapDelete("/:id", apiController.DeleteActor);
		MapGet("/ssr", ssrController.RenderActors);
	}
}