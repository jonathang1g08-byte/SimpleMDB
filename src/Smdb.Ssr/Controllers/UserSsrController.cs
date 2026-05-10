using Microsoft.AspNetCore.Mvc;
using Smdb.Ssr.Data;
using Smdb.Ssr.Models;
using Smdb.Ssr.Services;

namespace Smdb.Ssr.Controllers
{
	public class UserSsrController : Controller
	{
		private readonly IUserSsrRepository _repo;
		private readonly HtmlTemplates _html;

		public UserSsrController(IUserSsrRepository repo, HtmlTemplates html)
		{
			_repo = repo;
			_html = html;
		}

		// GET: /User
		public IActionResult Index()
		{
			var users = _repo.ReadAll();
			var body = _html.UserTable(users);
			var page = _html.Page("Users", body);

			return Content(page, "text/html");
		}

		// GET: /User/Create
		public IActionResult Create()
		{
			var body = _html.UserForm();
			var page = _html.Page("Create User", body);

			return Content(page, "text/html");
		}

		// POST: /User/Create
		[HttpPost]
		public IActionResult Create(User user)
		{
			_repo.Create(user);
			return RedirectToAction("Index");
		}

		// GET: /User/Edit/5
		public IActionResult Edit(int id)
		{
			var user = _repo.Read(id);
			if (user == null)
				return Content("User not found", "text/html");

			var body = _html.UserForm(user);
			var page = _html.Page("Edit User", body);

			return Content(page, "text/html");
		}

		// POST: /User/Edit
		[HttpPost]
		public IActionResult Edit(User user)
		{
			_repo.Update(user);
			return RedirectToAction("Index");
		}

		// GET: /User/Delete/5
		public IActionResult Delete(int id)
		{
			_repo.Delete(id);
			return RedirectToAction("Index");
		}
	}
}
