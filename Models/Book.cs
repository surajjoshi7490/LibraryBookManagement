using System.ComponentModel.DataAnnotations;

namespace LibraryBookManagement.Models
{
    public class Book
    {
        public Book()
        {
            Title = string.Empty;
            Author = string.Empty;
            ISBN = string.Empty;
            Genre = string.Empty;
            Detail = string.Empty;
            CreatedOn = DateTime.Now; // Default creation timestamp
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN must be exactly 13 digits.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Published Year is required.")]
        [Range(1000, 2100, ErrorMessage = "Enter a valid year.")]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public string Genre { get; set; }

        public string? Detail { get; set; }

        // ✅ Timestamps
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Display(Name = "Last Updated On")]
        public DateTime? UpdatedOn { get; set; }
    }
}
