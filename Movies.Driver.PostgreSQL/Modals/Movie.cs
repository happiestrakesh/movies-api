using Movies.Driver.PostgreSQL.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Movies.Driver.PostgreSQL.Modals
{
    public class Movie
    {
        public string? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? Title { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [ValidYear(ErrorMessage = "Year not valid")]
        public string? Year { get; set; }
        
        public string? Genre { get; set; }
        
        public List<string>? StarringActor { get; set; }
    }
}
