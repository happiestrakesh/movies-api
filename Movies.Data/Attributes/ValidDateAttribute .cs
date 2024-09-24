using System;
using System.ComponentModel.DataAnnotations;

namespace Movies.Data.Attributes
{
    public class ValidDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            DateTime validDate;

            if (!DateTime.TryParse((string?)value, out validDate))
            {
                // handle parse failure
                return false;
            }

            DateTime d = Convert.ToDateTime(validDate);

            return d < DateTime.Now; //Dates smaller than today are valid (true)
        }
    }
}
