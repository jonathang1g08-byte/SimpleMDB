using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Json;
using Shared.Http;
using Smdb.Core.Actors;

namespace Smdb.Ssr.Actors;

public class ActorsSsrController
{
	public async Task RenderActors(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		try
		{
			int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
			int size = int.TryParse(req.QueryString["size"], out int s) ? s : 6;

			// Fetch from API
			using var client = new HttpClient();
			var response = await client.GetAsync($"http://localhost:8080/api/v1/actors?page={page}&size={size}");
			var json = await response.Content.ReadAsStringAsync();
			var actorsData = JsonSerializer.Deserialize<JsonElement>(json, JsonSerializerOptions.Web);

			var actorsHtml = new StringBuilder();
			if (actorsData.ValueKind == JsonValueKind.Object && actorsData.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
			{
				foreach (var actor in data.EnumerateArray())
				{
					var id = actor.GetProperty("id").GetInt32();
					var name = actor.GetProperty("name").GetString();
					var birthYear = actor.GetProperty("birthYear").GetInt32();

					actorsHtml.Append($@"
			<div class='actor-card'>
				<h3>{name}</h3>
				<p class='birth-year'>Birth Year: {birthYear}</p>
				<a href='/Actors/view.html?id={id}' class='btn'>View</a>
			</div>");
				}
			}

			var html = $@"<!DOCTYPE html>
<html lang='en'>
<head>
	<meta charset='UTF-8'>
	<meta name='viewport' content='width=device-width, initial-scale=1.0'>
	<title>SimpleMDB - Actors</title>
	<link rel='stylesheet' href='/styles/main.css' />
	<style>
		.actors-grid {{ display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 2rem; padding: 2rem; }}
		.actor-card {{ background: #161b22; border: 1px solid #30363d; border-radius: 8px; padding: 1.5rem; }}
		.actor-card h3 {{ color: #58a6ff; margin-bottom: 0.5rem; }}
		.actor-card .birth-year {{ color: #8b949e; font-size: 0.9rem; }}
		.actor-card .btn {{ display: inline-block; background: #238636; color: white; padding: 0.5rem 1rem; text-decoration: none; border-radius: 4px; margin-top: 0.5rem; }}
		.actor-card .btn:hover {{ background: #2ea043; }}
	</style>
</head>
<body style='background: #0d1117; color: #c9d1d9; margin: 0; padding: 0; font-family: Arial, sans-serif;'>
	<header style='background: #161b22; padding: 2rem; border-bottom: 1px solid #30363d;'>
		<h1 style='color: #58a6ff; margin: 0;'>Actors</h1>
		<nav style='margin-top: 1rem;'>
			<a href='/api/v1/ssr/home' style='color: #58a6ff; text-decoration: none; margin-right: 1rem;'>Home</a>
			<a href='/Movies/index.html' style='color: #58a6ff; text-decoration: none; margin-right: 1rem;'>Movies</a>
			<a href='/Actors/index.html' style='color: #58a6ff; text-decoration: none; margin-right: 1rem;'>Actors</a>
			<a href='/Users/index.html' style='color: #58a6ff; text-decoration: none;'>Users</a>
		</nav>
	</header>
	<main style='max-width: 1400px; margin: 0 auto;'>
		<div class='actors-grid'>
			{actorsHtml}
		</div>
	</main>
</body>
</html>";

			res.StatusCode = (int)HttpStatusCode.OK;
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
