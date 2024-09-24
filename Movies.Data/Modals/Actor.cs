using Movies.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Movies.Data.Modals
{
    public class Actor
    {
        public string? Id { get; set; }

        [DisplayName("First Name")]
        [Required(AllowEmptyStrings = false)]
        public string? FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(AllowEmptyStrings = false)]
        public string? LastName { get; set; }

        [Display(Name = "Birth day")]
        [ValidDate(ErrorMessage = "Birth date not valid")]
        public string? BirthDay { get; set; }

        public List<string>? Filmography { get; set; }
    }
}
