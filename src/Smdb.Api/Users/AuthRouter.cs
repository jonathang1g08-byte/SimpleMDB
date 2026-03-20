namespace Smdb.Api.Users;

using Shared.Http;

public class AuthRouter : HttpRouter
{
	public AuthRouter(AuthApiController apiController, AuthSsrController ssrController)
	{
		MapPost("/register", HttpUtils.ReadRequestBodyAsText, apiController.Register);
		MapGet("/login", apiController.Login);
		MapGet("/ssr/login", ssrController.RenderLoginForm);
		MapGet("/ssr/register", ssrController.RenderRegisterForm);
	}
}
