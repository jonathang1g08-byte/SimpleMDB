namespace Smdb.Api.Home;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Shared.Http;
using Smdb.Core.Movies;

public class HomeSsrController
{
	private readonly IMovieService movieService;

	public HomeSsrController(IMovieService movieService)
	{
		this.movieService = movieService;
	}

	public async Task RenderLandingPage(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var moviesResult = await movieService.ReadMovies(1, 6);
		var pagedMovies = moviesResult.Payload!;
		var movies = pagedMovies.Values;

		var moviesHtml = new System.Text.StringBuilder();
		foreach (var movie in movies)
		{
			moviesHtml.Append($@"
			<div class='movie-card'>
				<h3>{movie.Title}</h3>
				<p class='year'>Year: {movie.Year}</p>
				<p class='description'>{movie.Description}</p>
			</div>");
		}

		var html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
	<meta charset='UTF-8'>
	<meta name='viewport' content='width=device-width, initial-scale=1.0'>
	<title>SimpleMDB - Movie Database</title>
	<style>
		* {{ margin: 0; padding: 0; box-sizing: border-box; }}
		body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #0d1117; color: #c9d1d9; }}
		header {{ background: linear-gradient(135deg, #1a1f2e 0%, #0d1117 100%); padding: 2rem; text-align: center; border-bottom: 1px solid #30363d; }}
		h1 {{ font-size: 3rem; color: #58a6ff; margin-bottom: 0.5rem; }}
		.subtitle {{ color: #8b949e; font-size: 1.2rem; }}
		nav {{ background: #161b22; padding: 1rem; display: flex; justify-content: center; gap: 2rem; border-bottom: 1px solid #30363d; }}
		nav a {{ color: #c9d1d9; text-decoration: none; padding: 0.5rem 1rem; border-radius: 6px; transition: background 0.2s; }}
		nav a:hover {{ background: #21262d; color: #58a6ff; }}
		.hero {{ text-align: center; padding: 4rem 2rem; background: linear-gradient(180deg, #0d1117 0%, #161b22 100%); }}
		.hero h2 {{ font-size: 2.5rem; margin-bottom: 1rem; }}
		.hero p {{ color: #8b949e; max-width: 600px; margin: 0 auto 2rem; }}
		.cta-button {{ display: inline-block; background: #238636; color: white; padding: 1rem 2rem; text-decoration: none; border-radius: 8px; font-weight: bold; transition: transform 0.2s; }}
		.cta-button:hover {{ transform: scale(1.05); }}
		.container {{ max-width: 1200px; margin: 0 auto; padding: 2rem; }}
		.section-title {{ text-align: center; margin-bottom: 2rem; font-size: 2rem; }}
		.movies-grid {{ display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 2rem; }}
		.movie-card {{ background: #161b22; border: 1px solid #30363d; border-radius: 8px; padding: 1.5rem; transition: transform 0.2s, border-color 0.2s; }}
		.movie-card:hover {{ transform: translateY(-5px); border-color: #58a6ff; }}
		.movie-card h3 {{ color: #58a6ff; margin-bottom: 0.5rem; }}
		.movie-card .year {{ color: #8b949e; font-size: 0.9rem; margin-bottom: 0.5rem; }}
		.movie-card .description {{ color: #c9d1d9; line-height: 1.6; }}
		footer {{ background: #161b22; padding: 2rem; text-align: center; border-top: 1px solid #30363d; margin-top: 4rem; }}
		footer p {{ color: #8b949e; }}
	</style>
</head>
<body>
	<header>
		<h1>🎬 SimpleMDB</h1>
		<p class='subtitle'>Your Personal Movie Database</p>
	</header>
	
	<nav>
		<a href='/'>Home</a>
		<a href='/api/v1/movies'>Movies</a>
		<a href='/api/v1/actors'>Actors</a>
		<a href='/api/v1/ssr/login'>Login</a>
		<a href='/api/v1/ssr/register'>Register</a>
	</nav>

	<section class='hero'>
		<h2>Welcome to SimpleMDB</h2>
		<p>Discover, manage, and explore your favorite movies and actors. Build your personal movie collection today!</p>
		<a href='/api/v1/movies' class='cta-button'>Browse All Movies</a>
	</section>

	<div class='container'>
		<h2 class='section-title'>Featured Movies</h2>
		<div class='movies-grid'>
			{moviesHtml}
		</div>
	</div>

	<footer>
		<p>© 2026 SimpleMDB. Built with ❤️</p>
	</footer>
</body>
</html>";

		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentType = "text/html";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(html));
		await next();
	}
}