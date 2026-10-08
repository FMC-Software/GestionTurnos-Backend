using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace GestionTurnos.Application.Helpers
{
    public static class SlugHelper
    {
        public static string Slugify(string value)
        {
            var decomposed = value.Normalize(NormalizationForm.FormD);

            var withoutDiacritics = new StringBuilder();
            foreach (var c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    withoutDiacritics.Append(c);
            }

            var slug = withoutDiacritics.ToString()
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();

            slug = Regex.Replace(slug, "[^a-z0-9]+", "-");
            slug = Regex.Replace(slug, "-{2,}", "-");

            return slug.Trim('-');
        }
    }
}
