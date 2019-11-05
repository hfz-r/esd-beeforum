using System.Globalization;
using System.Linq;
using System.Text;

namespace beeforum.Infrastructure
{
    public static class Slug
    {
        public static string GenerateSlug(this string phrase)
        {
            var idn = new IdnMapping();
            var punyCode = idn.GetAscii(phrase);

            return punyCode;
        }

        public static string RemoveDiacritics(this string text)
        {
            var s = new string(text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            return s.Normalize(NormalizationForm.FormC);
        }
    }
}