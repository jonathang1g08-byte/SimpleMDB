namespace Smdb.Api.Users;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Shared.Http;
using Smdb.Core.Users;

public class AuthSsrController
{
	private readonly IUsersService usersService;

	public AuthSsrController(IUsersService usersService)
	{
		this.usersService = usersService;
	}

	public async Task RenderLoginForm(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var html = @"
<html>
<head><title>Login</title></head>
<body>
	<h1>User Login</h1>
	<form method='GET' action='/api/v1/auth/login'>
		<label>Username: <input type='text' name='username' required /></label>
		<button type='submit'>Login</button>
	</form>
</body>
</html>
";
		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentType = "text/html";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(html));
		await next();
	}

	public async Task RenderRegisterForm(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var html = @"
<html>
<head><title>Register</title></head>
<body>
	<h1>User Registration</h1>
	<form method='POST' action='/api/v1/auth/register'>
		<label>Username: <input type='text' name='username' required /></label>
		<label>Email: <input type='email' name='email' required /></label>
		<label>Password: <input type='password' name='passwordHash' required /></label>
		<button type='submit'>Register</button>
	</form>
</body>
</html>
";
		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentType = "text/html";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(html));
		await next();
	}
}
