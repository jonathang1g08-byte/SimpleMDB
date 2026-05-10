namespace Smdb.Api;

using Shared.Http;
using Smdb.Api.Movies;
using Smdb.Api.Actors;
using Smdb.Api.ActorMovie;
using Smdb.Api.Users;
using Smdb.Api.Home;
using Smdb.Core.Movies;
using Smdb.Core.Actors;
using Smdb.Core.ActorMovie;
using Smdb.Core.Users;
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

		var homeSsrCtrl = new HomeSsrController(movieServ);
		var homeRouter = new HomeRouter(homeSsrCtrl);

		var actorRepo = new ActorsRepository(db);
		var actorServ = new ActorsService(actorRepo);
		var actorApiCtrl = new ActorsApiController(actorServ);
		var actorSsrCtrl = new ActorsSsrController(actorServ);
		var actorRouter = new ActorsRouter(actorApiCtrl, actorSsrCtrl);

		var actorMovieRepo = new ActorsMovieRepository(db);
		var actorMovieServ = new ActorsMovieService(actorMovieRepo);
		var actorMovieApiCtrl = new ActorsMovieApiController(actorMovieServ);
		var actorMovieSsrCtrl = new ActorsMovieSsrController(actorMovieServ);
		var actorMovieRouter = new ActorsMovieRouter(actorMovieApiCtrl, actorMovieSsrCtrl);

		var usersRepo = new UsersRepository(db);
		var usersServ = new UsersService(usersRepo);
		var usersApiCtrl = new UsersApiController(usersServ);
		var usersSsrCtrl = new UsersSsrController(usersServ);
		var usersRouter = new UsersRouter(usersApiCtrl, usersSsrCtrl);

		var apiRouter = new HttpRouter();
		router.Use(HttpUtils.StructuredLogging);
		router.Use(HttpUtils.CentralizedErrorHandling);
		router.Use(HttpUtils.AddResponseCorsHeaders);
		router.Use(HttpUtils.DefaultResponse);
		router.Use(HttpUtils.ParseRequestUrl);
		router.Use(HttpUtils.ParseRequestQueryString);
		router.UseParametrizedRouteMatching();
		router.UseRouter("/", homeRouter);
		router.UseRouter("/api/v1", apiRouter);
		apiRouter.UseRouter("/movies", movieRouter);
		apiRouter.UseRouter("/actors", actorRouter);
		apiRouter.UseRouter("/actormovie", actorMovieRouter);
		apiRouter.UseRouter("/users", usersRouter);
	}
	// <-- Rest of the code below goes here.
}