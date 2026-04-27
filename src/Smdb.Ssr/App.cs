namespace Smdb.Ssr;

using Shared.Http;

public class App : HttpServer
{
	public App()
	{
	}

	public override void Init()
	{
		router.Use(HttpUtils.StructuredLogging);
		router.Use(HttpUtils.CentralizedErrorHandling);
		router.Use(HttpUtils.AddResponseCorsHeaders);
		router.Use(HttpUtils.DefaultResponse);
		router.Use(HttpUtils.ParseRequestUrl);
		router.Use(HttpUtils.ParseRequestQueryString);
		router.UseParametrizedRouteMatching();

		// SSR routes will be added here
		// Movies, Actors, Users, etc. will be routed to Smdb.Api for SSR rendering
	}
}