using Microsoft.AspNetCore.Mvc;
using Smdb.Ssr.Data;
using Smdb.Ssr.Models;
using Smdb.Ssr.Services;

namespace Smdb.Ssr.Controllers
{
	public class ActorSsrController : Controller
	{
		private readonly IActorSsrRepository _repo;
		private readonly HtmlTemplates _html;

		public ActorSsrController(IActorSsrRepository repo, HtmlTemplates html)
		{
			_repo = repo;
			_html = html;
		}

		// GET: /Actor
		public IActionResult Index()
		{
			var actors = _repo.ReadAll();
			var body = _html.ActorTable(actors);
			var page = _html.Page("Actors", body);

			return Content(page, "text/html");
		}

		// GET: /Actor/Create
		public IActionResult Create()
		{
			var body = _html.ActorForm();
			var page = _html.Page("Create Actor", body);

			return Content(page, "text/html");
		}

		// POST: /Actor/Create
		[HttpPost]
		public IActionResult Create(Actor actor)
		{
			_repo.Create(actor);
			return RedirectToAction("Index");
		}

		// GET: /Actor/Edit/5
		public IActionResult Edit(int id)
		{
			var actor = _repo.Read(id);
			if (actor == null)
				return Content("Actor not found", "text/html");

			var body = _html.ActorForm(actor);
			var page = _html.Page("Edit Actor", body);

			return Content(page, "text/html");
		}

		// POST: /Actor/Edit
		[HttpPost]
		public IActionResult Edit(Actor actor)
		{
			_repo.Update(actor);
			return RedirectToAction("Index");
		}

		// GET: /Actor/Delete/5
		public IActionResult Delete(int id)
		{
			_repo.Delete(id);
			return RedirectToAction("Index");
		}
	}
}
