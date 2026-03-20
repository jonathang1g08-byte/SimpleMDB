namespace Smdb.Api.ActorMovie;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Shared.Http;
using Smdb.Core.ActorMovie;

public class ActorsMovieSsrController
{
	private readonly IActorsMovieService service;

	public ActorsMovieSsrController(IActorsMovieService service)
	{
		this.service = service;
	}

	public async Task RenderActorMovies(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;
		var result = await service.ReadActorMovies(page, size);

		if (result.IsError)
		{
			res.StatusCode = result.StatusCode;
			await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(result.Error?.Message ?? "Error"));
			await next();
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("<html><body><h1>Actor-Movie Credits</h1><ul>");
		foreach (var entry in result.Payload!.Values)
		{
			sb.AppendLine($"<li>{entry.Id}: ActorId={entry.ActorId}, MovieId={entry.MovieId}, Role={entry.Role}</li>");
		}
		sb.AppendLine("</ul></body></html>");

		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentType = "text/html";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(sb.ToString()));
		await next();
	}
}