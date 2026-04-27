namespace Smdb.Api.Home;

using Shared.Http;

public class HomeRouter : HttpRouter
{
	public HomeRouter(HomeSsrController homeSsrController)
	{
		MapGet("/", homeSsrController.RenderLandingPage);
	}
}