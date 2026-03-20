namespace Smdb.Api.Users;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Shared.Http;
using Smdb.Core.Users;

public class UsersSsrController
{
	private readonly IUsersService usersService;

	public UsersSsrController(IUsersService usersService)
	{
		this.usersService = usersService;
	}

	public async Task RenderUsers(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;
		var result = await usersService.ReadUsers(page, size);

		if (result.IsError)
		{
			res.StatusCode = result.StatusCode;
			await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(result.Error?.Message ?? "Error"));
			await next();
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("<html><body><h1>Users</h1><ul>");
		foreach (var user in result.Payload!.Values)
		{
			sb.AppendLine($"<li>{user.Id}: {user.Username} ({user.Email}) - Created: {user.CreatedAt:g}</li>");
		}
		sb.AppendLine("</ul></body></html>");

		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentType = "text/html";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(sb.ToString()));
		await next();
	}
}
