using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Json;
using Shared.Http;
using Smdb.Core.Movies;

namespace Smdb.Ssr.Movies;

public class MoviesSsrController
{
	public async Task RenderMovies(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		try
		{
			int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
			int size = int.TryParse(req.QueryString["size"], out int s) ? s : 6;

			// Fetch from API
			using var client = new HttpClient();
			var response = await client.GetAsync($"http://localhost:8080/api/v1/movies?page={page}&size={size}");
			var json = await response.Content.ReadAsStringAsync();
			var moviesData = JsonSerializer.Deserialize<JsonElement>(json, JsonSerializerOptions.Web);

			var moviesHtml = new StringBuilder();
			if (moviesData.ValueKind == JsonValueKind.Object && moviesData.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
			{
				foreach (var movie in data.EnumerateArray())
				{
					var id = movie.GetProperty("id").GetInt32();
					var title = movie.GetProperty("title").GetString();
					var year = movie.GetProperty("year").GetInt32();
					var description = movie.GetProperty("description").GetString();

					moviesHtml.Append($@"
			<div class='movie-card'>
				<h3>{title}</h3>
				<p class='year'>Year: {year}</p>
				<p class='description'>{description}</p>
				<a href='/Movies/view.html?id={id}' class='btn'>View</a>
			</div>");
				}
			}

< html lang = 'en' >
< head >
	< meta charset = 'UTF-8' >
	< meta name = 'viewport' content = 'width=device-width, initial-scale=1.0' >
	< title > SimpleMDB - Movies </ title >
	< link rel = 'stylesheet' href = '/styles/main.css' />
	< style >
		.movies - grid { { display: grid; grid - template - columns: repeat(auto - fill, minmax(300px, 1fr)); gap: 2rem; padding: 2rem; } }
		.movie - card {
				{
				background: #161b22; border: 1px solid #30363d; border-radius: 8px; padding: 1.5rem; }}
		.movie - card h3 {
						{
						color: #58a6ff; margin-bottom: 0.5rem; }}
		.movie - card.year {
								{
								color: #8b949e; font-size: 0.9rem; }}
		.movie - card.description {
										{
										color: #c9d1d9; margin: 0.5rem 0; }}
		.movie - card.btn {
												{
												display: inline - block; background: #238636; color: white; padding: 0.5rem 1rem; text-decoration: none; border-radius: 4px; margin-top: 0.5rem; }}
		.movie - card.btn:hover {
														{
														background: #2ea043; }}
	</ style >
</ head >
< body style = 'background: #0d1117; color: #c9d1d9; margin: 0; padding: 0; font-family: Arial, sans-serif;' >
	< header style = 'background: #161b22; padding: 2rem; border-bottom: 1px solid #30363d;' >
		< h1 style = 'color: #58a6ff; margin: 0;' > Movies </ h1 >
		< nav style = 'margin-top: 1rem;' >
			< a href = '/api/v1/ssr/home' style = 'color: #58a6ff; text-decoration: none; margin-right: 1rem;' > Home </ a >
			< a href = '/Movies/index.html' style = 'color: #58a6ff; text-decoration: none; margin-right: 1rem;' > Movies </ a >
			< a href = '/Actors/index.html' style = 'color: #58a6ff; text-decoration: none; margin-right: 1rem;' > Actors </ a >
			< a href = '/Users/index.html' style = 'color: #58a6ff; text-decoration: none;' > Users </ a >
		</ nav >
	</ header >
	< main style = 'max-width: 1400px; margin: 0 auto;' >
		< div class='movies-grid'>
			{moviesHtml
}
		</div>
	</main>
</body>
</html>";

			res.StatusCode = (int) HttpStatusCode.OK;
res.ContentEncoding = Encoding.UTF8;
			res.ContentType = "text/html; charset=utf-8";
			await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(html));
		}
		catch (Exception ex)
		{
	res.StatusCode = (int)HttpStatusCode.InternalServerError;
	await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes($"Error: {ex.Message}"));
}
await next();
	}
}
