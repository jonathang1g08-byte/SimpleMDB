namespace Smdb.Api.Users;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Shared.Http;
using Smdb.Core.Users;

public class AuthApiController
{
	private readonly IUsersService usersService;

	public AuthApiController(IUsersService usersService)
	{
		this.usersService = usersService;
	}

	public async Task Register(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var text = (string)props["req.text"]!;
		var user = JsonSerializer.Deserialize<User>(text, JsonSerializerOptions.Web);
		var result = await usersService.CreateUser(user!);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}

	public async Task Login(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var queryParams = req.QueryString;
		var username = queryParams["username"];

		if (string.IsNullOrEmpty(username))
		{
			var error = new Result<User>(new Exception("Username query parameter is required."), (int)HttpStatusCode.BadRequest);
			await JsonUtils.SendResultResponse(req, res, props, error);
			await next();
			return;
		}

		var result = await usersService.ReadUserByUsername(username);
		await JsonUtils.SendResultResponse(req, res, props, result);
		await next();
	}
}
