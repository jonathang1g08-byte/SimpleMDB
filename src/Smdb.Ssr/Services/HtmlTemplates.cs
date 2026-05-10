using Smdb.Ssr.Models;
using System.Text;

namespace Smdb.Ssr.Services
{
    public class HtmlTemplates
    {
        // ------------------------------
        // PAGE WRAPPER
        // ------------------------------
        public string Page(string title, string body)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>{title}</title>
</head>
<body>
    <h1>{title}</h1>

    <div>
        <a href=""/UserSsr"">Users</a> |
        <a href=""/ActorSsr"">Actors</a> |
        <a href=""/MovieSsr"">Movies</a>
    </div>

    <hr />

    {body}
</body>
</html>
";
        }

        // ------------------------------
        // USER TABLE
        // ------------------------------
        public string UserTable(List<User> users)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"<a href=""/UserSsr/Create"">Create User</a><br/><br/>");

            sb.AppendLine("<table border=\"1\">");
            sb.AppendLine("<tr><th>ID</th><th>Name</th><th>Email</th><th>Actions</th></tr>");

            foreach (var u in users)
            {
                sb.AppendLine($@"
<tr>
    <td>{u.Id}</td>
    <td>{u.Name}</td>
    <td>{u.Email}</td>
    <td>
        <a href=""/UserSsr/Edit/{u.Id}"">Edit</a> |
        <a href=""/UserSsr/Delete/{u.Id}"">Delete</a>
    </td>
</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        // ------------------------------
        // USER FORM
        // ------------------------------
        public string UserForm(User? user = null)
        {
            bool editing = user != null;

            return $@"
<form method=""post"">
    {(editing ? $@"<input type=""hidden"" name=""Id"" value=""{user!.Id}"" />" : "")}

    <label>Name:</label><br/>
    <input type=""text"" name=""Name"" value=""{user?.Name}"" /><br/><br/>

    <label>Email:</label><br/>
    <input type=""text"" name=""Email"" value=""{user?.Email}"" /><br/><br/>

    <button type=""submit"">{(editing ? "Update" : "Create")}</button>
</form>
";
        }

        // ------------------------------
        // ACTOR TABLE
        // ------------------------------
        public string ActorTable(List<Actor> actors)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"<a href=""/ActorSsr/Create"">Create Actor</a><br/><br/>");

            sb.AppendLine("<table border=\"1\">");
            sb.AppendLine("<tr><th>ID</th><th>Name</th><th>Birth Year</th><th>Actions</th></tr>");

            foreach (var a in actors)
            {
                sb.AppendLine($@"
<tr>
    <td>{a.Id}</td>
    <td>{a.Name}</td>
    <td>{a.BirthYear}</td>
    <td>
        <a href=""/ActorSsr/Edit/{a.Id}"">Edit</a> |
        <a href=""/ActorSsr/Delete/{a.Id}"">Delete</a>
    </td>
</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        // ------------------------------
        // ACTOR FORM
        // ------------------------------
        public string ActorForm(Actor? actor = null)
        {
            bool editing = actor != null;

            return $@"
<form method=""post"">
    {(editing ? $@"<input type=""hidden"" name=""Id"" value=""{actor!.Id}"" />" : "")}

    <label>Name:</label><br/>
    <input type=""text"" name=""Name"" value=""{actor?.Name}"" /><br/><br/>

    <label>Birth Year:</label><br/>
    <input type=""number"" name=""BirthYear"" value=""{actor?.BirthYear}"" /><br/><br/>

    <button type=""submit"">{(editing ? "Update" : "Create")}</button>
</form>
";
        }

        // ------------------------------
        // MOVIE TABLE
        // ------------------------------
        public string MovieTable(List<Movie> movies)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"<a href=""/MovieSsr/Create"">Create Movie</a><br/><br/>");

            sb.AppendLine("<table border=\"1\">");
            sb.AppendLine("<tr><th>ID</th><th>Title</th><th>Year</th><th>Actors</th><th>Actions</th></tr>");

            foreach (var m in movies)
            {
                string actorNames = string.Join(", ", m.Actors.Select(a => a.Name));

                sb.AppendLine($@"
<tr>
    <td>{m.Id}</td>
    <td>{m.Title}</td>
    <td>{m.Year}</td>
    <td>{actorNames}</td>
    <td>
        <a href=""/MovieSsr/Edit/{m.Id}"">Edit</a> |
        <a href=""/MovieSsr/Delete/{m.Id}"">Delete</a>
    </td>
</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        // ------------------------------
        // MOVIE FORM
        // ------------------------------
        public string MovieForm(Movie? movie, List<Actor> actors)
        {
            bool editing = movie != null;

            var sb = new StringBuilder();

            sb.AppendLine("<form method=\"post\">");

            if (editing)
                sb.AppendLine($@"<input type=""hidden"" name=""Id"" value=""{movie!.Id}"" />");

            sb.AppendLine(@"
<label>Title:</label><br/>
<input type=""text"" name=""Title"" value=""" + movie?.Title + @""" /><br/><br/>

<label>Year:</label><br/>
<input type=""number"" name=""Year"" value=""" + movie?.Year + @""" /><br/><br/>

<label>Actors:</label><br/>
");

            foreach (var actor in actors)
            {
                bool isChecked = movie?.Actors.Any(a => a.Id == actor.Id) ?? false;

                sb.AppendLine($@"
<input type=""checkbox"" name=""actorIds"" value=""{actor.Id}"" {(isChecked ? "checked" : "")} />
{actor.Name} ({actor.BirthYear})<br/>
");
            }

            sb.AppendLine("<br/><button type=\"submit\">" + (editing ? "Update" : "Create") + "</button>");
            sb.AppendLine("</form>");

            return sb.ToString();
        }
    }
}
