namespace Smdb.Ssr;

using Shared.Http;
using Smdb.Ssr.Auth;
using Smdb.Ssr.Movies;
using Smdb.Ssr.Actors;
using Smdb.Ssr.Users;

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
		router.Use(HttpUtils.ServeStaticFiles);
		router.UseParametrizedRouteMatching();

		// Auth routes
		var authController = new AuthController();
		var authRouter = new HttpRouter();
		authRouter.UseParametrizedRouteMatching();
		authRouter.MapGet("", async (req, res, props, next) => { res.Redirect("/api/v1/ssr/login"); await next(); });
		authRouter.MapGet("/", authController.RenderLogin);
		authRouter.MapGet("/login", authController.RenderLogin);
		authRouter.MapPost("/login", HttpUtils.ReadRequestBodyAsForm, authController.ProcessLogin);
		authRouter.MapGet("/register", authController.RenderRegister);
		authRouter.MapPost("/register", HttpUtils.ReadRequestBodyAsForm, authController.ProcessRegister);
		authRouter.MapGet("/home", authController.RenderHome);
		router.UseRouter("/api/v1/ssr", authRouter);

		// SSR content routes (fetch data from API at port 8080 and render as HTML)
		var moviesSsrCtrl = new MoviesSsrController();
		router.MapGet("/Movies/index.html", moviesSsrCtrl.RenderMovies);

		var actorsSsrCtrl = new ActorsSsrController();
		router.MapGet("/Actors/index.html", actorsSsrCtrl.RenderActors);

		var usersSsrCtrl = new UsersSsrController();
		router.MapGet("/Users/index.html", usersSsrCtrl.RenderUsers);

		router.MapGet("/", async (req, res, props, next) =>
		{
			res.Redirect("/api/v1/ssr");
			await next();
		});
	}
}