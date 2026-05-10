using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Shared.Http;

namespace Smdb.Ssr.Auth;

public class AuthController
{
	public async Task RenderLogin(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		string html = HtmlTemplates.RenderLoginForm(
				"/api/v1/ssr/login",
				"Don't have an account?",
				"Register",
				"/api/v1/ssr/register"
		);

		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentEncoding = Encoding.UTF8;
		res.ContentType = "text/html; charset=utf-8";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(html));
		await next();
	}

	public async Task RenderRegister(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		string html = HtmlTemplates.RenderRegisterForm(
				"/api/v1/ssr/register",
				"Already have an account?",
				"Login",
				"/api/v1/ssr/login"
		);

		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentEncoding = Encoding.UTF8;
		res.ContentType = "text/html; charset=utf-8";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(html));
		await next();
	}

	public async Task ProcessLogin(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var form = (NameValueCollection?)props["req.form"];
		string email = form?["email"] ?? string.Empty;
		string password = form?["password"] ?? string.Empty;

		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			res.Redirect("/api/v1/ssr/login");
		}
		else
		{
			res.Redirect("/api/v1/ssr/home");
		}

		await next();
	}

	public async Task ProcessRegister(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var form = (NameValueCollection?)props["req.form"];
		string email = form?["email"] ?? string.Empty;
		string password = form?["password"] ?? string.Empty;

		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			res.Redirect("/api/v1/ssr/register");
		}
		else
		{
			res.Redirect("/api/v1/ssr/home");
		}

		await next();
	}

	public async Task RenderHome(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		string html = @"<!DOCTYPE html>
<html lang='en'>
<head>
	<meta charset='UTF-8'>
	<meta name='viewport' content='width=device-width, initial-scale=1.0'>
	<title>SimpleMDB - Home</title>
	<link rel='stylesheet' href='/styles/main.css' />
</head>
<body class='auth-page'>
	<div class='auth-container'>
		<h1 class='auth-title'>Welcome to SimpleMDB</h1>
		<p class='auth-description'>You are now logged in. Choose a page below:</p>
		<div style='display:flex;flex-direction:column;gap:1rem;margin-top:1.5rem;'>
				<a class='cta-button' href='/Movies/index.html'>Browse Movies</a>
				<a class='cta-button' href='/Actors/index.html'>Browse Actors</a>
		</div>
	</div>
</body>
</html>";

		res.StatusCode = (int)HttpStatusCode.OK;
		res.ContentEncoding = Encoding.UTF8;
		res.ContentType = "text/html; charset=utf-8";
		await res.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(html));
		await next();
	}
}
