namespace Smdb.Api.ActorMovie;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Shared.Http;
using Smdb.Core.ActorMovie;

public class ActorsMovieApiController
{
	private readonly IActorsMovieService service;

	public ActorsMovieApiController(IActorsMovieService service)
	{
		this.service = service;
	}

	public async Task ReadActorMovies(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;
		var result = await service.ReadActorMovies(page, size);
		await JsonUtils.SendPagedResultResponse(req, res, props, result, page, size);
		await next();
	}

	public async Task CreateActorMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var text = (string)props["req.text"]!;
		var entry = JsonSerializer.Deserialize<ActorMovie>(text, JsonSerializerOptions.Web);
		var result = await service.CreateActorMovie(entry!);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	public async Task ReadActorMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var result = await service.ReadActorMovie(id);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	public async Task UpdateActorMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var text = (string)props["req.text"]!;
		var entry = JsonSerializer.Deserialize<ActorMovie>(text, JsonSerializerOptions.Web);
		var result = await service.UpdateActorMovie(id, entry!);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	public async Task DeleteActorMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var result = await service.DeleteActorMovie(id);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}
}