namespace Smdb.Api;

using Shared.Http;
using Smdb.Api.Movies;
using Smdb.Core.Movies;
public class App : HttpServer
{
	public override void Init()
	{
		var db = new MemoryDatabase();
		var movieRepo = new MemoryMovieRepository(db);
		var movieServ = new DefaultMovieService(movieRepo);
		var movieCtrl = new MoviesController(movieServ);
		var movieRouter = new MoviesRouter(movieCtrl);
		var apiRouter = new HttpRouter();
		router.Use(HttpUtils.StructuredLogging);
		router.Use(HttpUtils.CentralizedErrorHandling);
		router.Use(HttpUtils.AddResponseCorsHeaders);
		router.Use(HttpUtils.DefaultResponse);
		router.Use(HttpUtils.ParseRequestUrl);
		router.Use(HttpUtils.ParseRequestQueryString);
		router.UseParametrizedRouteMatching();
		router.UseRouter("/api/v1", apiRouter);
		apiRouter.UseRouter("/movies", movieRouter);
	}
	// <-- Rest of the code below goes here.
}