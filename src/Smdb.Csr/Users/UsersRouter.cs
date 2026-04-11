namespace Smdb.Csr.Users;

using Shared.Http;

public class UsersRouter : HttpRouter
{
	public UsersRouter(UsersApiController apiController, UsersSsrController ssrController)
	{
		UseParametrizedRouteMatching();
		MapGet("/", apiController.ReadUsers);
		MapPost("/", HttpUtils.ReadRequestBodyAsText, apiController.CreateUser);
		MapGet("/:id", apiController.ReadUser);
		MapPut("/:id", HttpUtils.ReadRequestBodyAsText, apiController.UpdateUser);
		MapDelete("/:id", apiController.DeleteUser);
		MapGet("/ssr", ssrController.RenderUsers);
	}
}
