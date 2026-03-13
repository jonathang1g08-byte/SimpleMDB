namespace Smdb.Api.Movies;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Shared.Http;
using Smdb.Core.Movies;
public class MoviesController
{
	private IMovieService movieService;
	public MoviesController(IMovieService movieService)
	{
		this.movieService = movieService;
	}

	// curl -X GET "http://localhost:8080/api/v1/movies?page=1&size=10"
	public async Task ReadMovies(HttpListenerRequest req, HttpListenerResponse res,
	Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;
		var result = await movieService.ReadMovies(page, size);
		await JsonUtils.SendPagedResultResponse(req, res, props, result, page, size);
		await next();
	}

	// curl -X POST "http://localhost:8080/api/v1/movies" -H "Content-Type: application/json" -d "{ \"id\": -1, \"title\": \"Inception\", \"year\": 2010, \"description\": \"A skilled thief who enters dreams to steal secrets.\" }"
	public async Task CreateMovie(HttpListenerRequest req,
	HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var text = (string)props["req.text"]!;
		var movie = JsonSerializer.Deserialize<Movie>(text,
		JsonSerializerOptions.Web);
		var result = await movieService.CreateMovie(movie!);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	// curl -X GET "http://localhost:8080/api/v1/movies/1"
	public async Task ReadMovie(HttpListenerRequest req, HttpListenerResponse res,
	Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var result = await movieService.ReadMovie(id);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	// curl -X PUT "http://localhost:8080/api/v1/movies/1" -H "Content-Type: application/json" -d "{ \"title\": \"Joker 2\", \"year\": 2020, \"description\": \"A man that is a joke.\" }"
	public async Task UpdateMovie(HttpListenerRequest req,
	HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var text = (string)props["req.text"]!;
		var movie = JsonSerializer.Deserialize<Movie>(text,
		JsonSerializerOptions.Web);
		var result = await movieService.UpdateMovie(id, movie!);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	// curl -X DELETE http://localhost:8080/api/v1/movies/1
	public async Task DeleteMovie(HttpListenerRequest req,
	HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var result = await movieService.DeleteMovie(id);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}
	// <-- Rest of the code below goes here.
}