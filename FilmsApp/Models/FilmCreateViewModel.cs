using System.ComponentModel.DataAnnotations;

namespace FilmsApp.Models
{
    public class FilmCreateViewModel
    {
        [Required(ErrorMessage = "Вкажіть назву")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть опис")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть жанр")]
        [StringLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть файл постера")]
        public IFormFile? Poster { get; set; }

        [Required(ErrorMessage = "Оберіть відеофайл або трейлер")]
        public IFormFile? Video { get; set; }
    }
}
