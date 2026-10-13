namespace FilmsApp.Models
{
    public class Film
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string s3PosterKey { get; set; } = string.Empty; 
        public string s3VideoKey { get; set; } = string.Empty;
    }
}
