using Movies.Driver.SqlServer.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Movies.Driver.SqlServer.Modals
{
    public class Movie
    {
        [Key]
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
