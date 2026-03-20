namespace Smdb.Api;

using Shared.Http;
using Smdb.Api.Movies;
using Smdb.Api.Actors;
using Smdb.Core.Movies;
using Smdb.Core.Actors;
using Smdb.Core.Db;
public class App : HttpServer
{
	public override void Init()
	{
		var db = new MemoryDatabase();
		var movieRepo = new MemoryMovieRepository(db);
		var movieServ = new DefaultMovieService(movieRepo);
		var movieCtrl = new MoviesController(movieServ);
		var movieRouter = new MoviesRouter(movieCtrl);

		var actorRepo = new ActorsRepository(db);
		var actorServ = new ActorsService(actorRepo);
		var actorApiCtrl = new ActorsApiController(actorServ);
		var actorSsrCtrl = new ActorsSsrController(actorServ);
		var actorRouter = new ActorsRouter(actorApiCtrl, actorSsrCtrl);

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
		apiRouter.UseRouter("/actors", actorRouter);
	}
	// <-- Rest of the code below goes here.
}