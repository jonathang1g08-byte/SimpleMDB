namespace Smdb.Csr;

using Shared.Http;
using Smdb.Csr.Users;
using Smdb.Csr.ActorMovie;
using Smdb.Csr.Actors;

public class App : HttpServer
{
	public App()
	{
	}
	public override void Init()
	{
		var usersDb = new MemoryDatabase();
		var usersRepo = new UsersRepository(usersDb);
		var usersServ = new UsersService(usersRepo);
		var usersApiCtrl = new UsersApiController(usersServ);
		var usersSsrCtrl = new UsersSsrController(usersServ);
		var usersRouter = new UsersRouter(usersApiCtrl, usersSsrCtrl);

		var amDb = new ActorMovieMemoryDatabase();
		var amRepo = new ActorsMovieRepository(amDb);
		var amServ = new ActorsMovieService(amRepo);
		var amApiCtrl = new ActorsMovieApiController(amServ);
		var amSsrCtrl = new ActorsMovieSsrController(amServ);
		var amRouter = new ActorsMovieRouter(amApiCtrl, amSsrCtrl);

		var actorDb = new ActorMemoryDatabase();
		var actorRepo = new ActorsRepository(actorDb);
		var actorServ = new ActorsService(actorRepo);
		var actorApiCtrl = new ActorsApiController(actorServ);
		var actorSsrCtrl = new ActorsSsrController(actorServ);
		var actorRouter = new ActorsRouter(actorApiCtrl, actorSsrCtrl);

		router.Use(HttpUtils.StructuredLogging);
		router.Use(HttpUtils.CentralizedErrorHandling);
		router.Use(HttpUtils.AddResponseCorsHeaders);
		router.Use(HttpUtils.DefaultResponse);
		router.Use(HttpUtils.ParseRequestUrl);
		router.Use(HttpUtils.ParseRequestQueryString);
		router.UseParametrizedRouteMatching();
		router.UseRouter("/api/v1/users", usersRouter);
		router.UseRouter("/api/v1/actors", actorRouter);
		router.UseRouter("/api/v1/actormovie", amRouter);
		router.Use(HttpUtils.ServeStaticFiles);
		router.UseSimpleRouteMatching();
		router.MapGet("/", async (req, res, props, next) =>
		{ res.Redirect("/index.html"); await next(); });
		router.MapGet("/movies", async (req, res, props, next) =>
		{ res.Redirect("/movies/index.html"); await next(); });
	}
}