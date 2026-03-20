namespace Smdb.Api.Users;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Shared.Http;
using Smdb.Core.Users;

public class UsersApiController
{
	private readonly IUsersService usersService;

	public UsersApiController(IUsersService usersService)
	{
		this.usersService = usersService;
	}

	public async Task ReadUsers(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;
		var result = await usersService.ReadUsers(page, size);
		await JsonUtils.SendPagedResultResponse(req, res, props, result, page, size);
		await next();
	}

	public async Task ReadUser(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var result = await usersService.ReadUser(id);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	public async Task UpdateUser(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var text = (string)props["req.text"]!;
		var user = JsonSerializer.Deserialize<User>(text, JsonSerializerOptions.Web);
		var result = await usersService.UpdateUser(id, user!);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	public async Task DeleteUser(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["req.params"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var result = await usersService.DeleteUser(id);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}
}
