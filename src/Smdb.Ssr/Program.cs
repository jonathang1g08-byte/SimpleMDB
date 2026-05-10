
using Smdb.Ssr.Data;
using Smdb.Ssr.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// Dependency Injection
// ------------------------------
builder.Services.AddSingleton<IUserSsrRepository, UserSsrRepository>();
builder.Services.AddSingleton<IActorSsrRepository, ActorSsrRepository>();
builder.Services.AddSingleton<IMovieSsrRepository, MovieSsrRepository>();

builder.Services.AddSingleton<HtmlTemplates>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ------------------------------
// Routing
// ------------------------------
app.UseRouting();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
			name: "default",
			pattern: "{controller=UserSsr}/{action=Index}/{id?}");
});

app.Run();
