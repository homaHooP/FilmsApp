using FilmsApp.Data;
using FilmsApp.Models;
using FilmsApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FilmsApp.Controllers
{
    public class HomeController(AppDbContext db, BucketService bucket) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var films = await db.Films.AsNoTracking().OrderBy(f => f.Title).ToListAsync();

            var cards = new List<FilmViewModel>();
            foreach (var film in films)
            {
                cards.Add(new FilmViewModel(film, await bucket.GetReadUrlAsync(film.s3PosterKey, TimeSpan.FromHours(3))));
            }
            return View(cards);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
