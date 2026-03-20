namespace Smdb.Api.Actors;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Shared.Http;
using Smdb.Core.Actors;

public class ActorsSsrController
{
	private readonly IActorsService actorService;

	public ActorsSsrController(IActorsService actorService)
	{
		this.actorService = actorService;
	}

	public async Task RenderActors(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;
		var result = await actorService.ReadActors(page, size);

		if (result.IsError)
		{
			res.StatusCode = result.StatusCode;
			await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(result.Error?.Message ?? "Error"));
			await next();
			return;
		}

		var body = new StringBuilder();
		body.AppendLine("<html><body><h1>Actors</h1><ul>");
		foreach (var actor in result.Payload!.Values)
		{
			body.AppendLine($"<li>{actor.Id}: {actor.Name} ({actor.BirthYear}) - {actor.Biography}</li>");
		}
		body.AppendLine("</ul></body></html>");

		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentType = "text/html";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(body.ToString()));
		await next();
	}
}