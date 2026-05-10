using Microsoft.AspNetCore.Mvc;
using Smdb.Ssr.Data;
using Smdb.Ssr.Models;
using Smdb.Ssr.Services;

namespace Smdb.Ssr.Controllers
{
	public class MovieSsrController : Controller
	{
		private readonly IMovieSsrRepository _repo;
		private readonly IActorSsrRepository _actorRepo;
		private readonly HtmlTemplates _html;

		public MovieSsrController(IMovieSsrRepository repo, IActorSsrRepository actorRepo, HtmlTemplates html)
		{
			_repo = repo;
			_actorRepo = actorRepo;
			_html = html;
		}

		// GET: /Movie
		public IActionResult Index()
		{
			var movies = _repo.ReadAll();
			var body = _html.MovieTable(movies);
			var page = _html.Page("Movies", body);

			return Content(page, "text/html");
		}

		// GET: /Movie/Create
		public IActionResult Create()
		{
			var actors = _actorRepo.ReadAll();
			var body = _html.MovieForm(null, actors);
			var page = _html.Page("Create Movie", body);

			return Content(page, "text/html");
		}

		// POST: /Movie/Create
		[HttpPost]
		public IActionResult Create(Movie movie, int[] actorIds)
		{
			_repo.Create(movie, actorIds);
			return RedirectToAction("Index");
		}

		// GET: /Movie/Edit/5
		public IActionResult Edit(int id)
		{
			var movie = _repo.Read(id);
			if (movie == null)
				return Content("Movie not found", "text/html");

			var actors = _actorRepo.ReadAll();
			var body = _html.MovieForm(movie, actors);
			var page = _html.Page("Edit Movie", body);

			return Content(page, "text/html");
		}

		// POST: /Movie/Edit
		[HttpPost]
		public IActionResult Edit(Movie movie, int[] actorIds)
		{
			_repo.Update(movie, actorIds);
			return RedirectToAction("Index");
		}

		// GET: /Movie/Delete/5
		public IActionResult Delete(int id)
		{
			_repo.Delete(id);
			return RedirectToAction("Index");
		}
	}
}
