using FilmsApp.Data;
using FilmsApp.Models;
using FilmsApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmsApp.Controllers
{
    public class FilmsController(AppDbContext db, BucketService bucket) : Controller
    {
        private static readonly TimeSpan UrlLifetime = TimeSpan.FromHours(3);

        public async Task<IActionResult> Details(Guid id)
        {
            var film = await db.Films.FindAsync(id);
            if (film == null) return NotFound();

            var posterUrl = await bucket.GetReadUrlAsync(film.s3PosterKey, UrlLifetime);
            var videoUrl = await bucket.GetReadUrlAsync(film.s3VideoKey, UrlLifetime);
            return View(new FilmViewModel(film, posterUrl, videoUrl));
        }

        public IActionResult Create() => View(new FilmCreateViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmCreateViewModel model)
        {
            if (model.Poster != null && !model.Poster.ContentType.StartsWith("image/"))
                ModelState.AddModelError(nameof(model.Poster), "Постер має бути зображенням");
            if (model.Video != null && !model.Video.ContentType.StartsWith("video/"))
                ModelState.AddModelError(nameof(model.Video), "Оберіть відеофайл");
            if (!ModelState.IsValid) return View(model);

            var id = Guid.NewGuid();
            var film = new Film
            {
                Id = id,
                Title = model.Title,
                Description = model.Description,
                Genre = model.Genre,
                s3PosterKey = $"poster/{id}{Path.GetExtension(model.Poster!.FileName).ToLowerInvariant()}",
                s3VideoKey = $"video/{id}{Path.GetExtension(model.Video!.FileName).ToLowerInvariant()}"
            };

            await bucket.UploadAsync(film.s3PosterKey, model.Poster);
            await bucket.UploadAsync(film.s3VideoKey, model.Video);

            db.Films.Add(film);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var film = await db.Films.FindAsync(id);
            if (film == null) return NotFound();

            await bucket.DeleteAsync(film.s3PosterKey);
            await bucket.DeleteAsync(film.s3VideoKey);

            db.Films.Remove(film);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
