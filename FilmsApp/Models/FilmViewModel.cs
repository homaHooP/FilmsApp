namespace FilmsApp.Models
{
    public record FilmViewModel(Film Film, string PosterUrl, string? VideoUrl = null);
}
