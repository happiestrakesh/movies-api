using System.ComponentModel.DataAnnotations;

namespace Movies.Driver.SqlServer.Attributes
{
    public class ValidYearAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            short validYear;

            if (!short.TryParse((string?)value, out validYear))
            {
                // handle parse failure
                return false;
            }

            return validYear < DateTime.Now.Year; //Year smaller than today are valid (true)
        }
    }
}
