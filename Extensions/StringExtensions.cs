using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mel.Live.Extensions
{
    public static class StringExtensions
    {
        public static string BindTo<T>(this string body, T model) where T : class
        {
            Regex regex = new Regex(@"{([a-zA-Z]+[0-9]*)}");

            var matches = regex.Matches(body).Cast<Match>()
                .OrderByDescending(i => i.Index);

            foreach (Match match in matches)
            {
                var fullMatch = match.Groups[0];

                var propName = match.Groups[1].Value;

                object value = string.Empty;

                try
                {
                    // use reflection to get property
                    // Note: if you need to use fields use GetField
                    var prop = typeof(T).GetProperty(propName);
                    value = prop.GetValue(model);

                    if (value == null) value = string.Empty;
                }
                catch (Exception ex)
                {
                    Core.Log.Debug($"A binding error occured while binding type of ({typeof(T)}) to {body}\n{ex}");
                    return body;
                }

                // remove substring with pattern
                // use remove instead of replace, since 
                // you may have several the same string
                // and insert what required
                body = body.Remove(fullMatch.Index, fullMatch.Length)
                    .Insert(fullMatch.Index, value.ToString());
            }

            return body;
        }

        public static bool IsEmailAddress(this string value) => new EmailAddressAttribute().IsValid(value);

        /// <summary>
        /// Creates a URL And SEO friendly slug
        /// </summary>
        /// <param name="text">Text to slugify</param>
        /// <param name="maxLength">Max length of slug</param>
        /// <returns>URL and SEO friendly string</returns>
        /// <remarks>
        /// Cloned from (https://www.johanbostrom.se/blog/how-to-create-a-url-and-seo-friendly-string-in-csharp-text-to-slug-generator)
        /// thanks to JOHAN BOSTRÖM for making this available.
        /// Javascript version (https://gist.github.com/hagemann/382adfc57adbd5af078dc93feef01fe1)
        /// </remarks>
        public static string UrlFriendly(this string text, int maxLength = 0)
        {
            // Return empty value if text is null
            if (text == null) return "";
            var normalizedString = text
                // Make lowercase
                .ToLowerInvariant()
                // Normalize the text
                .Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            var stringLength = normalizedString.Length;
            var prevdash = false;
            var trueLength = 0;
            char c;
            for (int i = 0; i < stringLength; i++)
            {
                c = normalizedString[i];
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    // Check if the character is a letter or a digit if the character is a
                    // international character remap it to an ascii valid character
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (c < 128)
                            stringBuilder.Append(c);
                        else
                            stringBuilder.Append(RemapInternationalCharToAscii(c));
                        prevdash = false;
                        trueLength = stringBuilder.Length;
                        break;
                    // Check if the character is to be replaced by a hyphen but only if the last character wasn't
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.OtherPunctuation:
                    case UnicodeCategory.MathSymbol:
                        if (!prevdash)
                        {
                            stringBuilder.Append('-');
                            prevdash = true;
                            trueLength = stringBuilder.Length;
                        }
                        break;
                }
                // If we are at max length, stop parsing
                if (maxLength > 0 && trueLength >= maxLength)
                    break;
            }
            // Trim excess hyphens
            var result = stringBuilder.ToString().Trim('-');
            // Remove any excess character to meet maxlength criteria
            return maxLength <= 0 || result.Length <= maxLength ? result : result.Substring(0, maxLength);
        }

        /// <summary>
        /// Remaps international characters to ascii compatible ones
        /// based of: https://meta.stackexchange.com/questions/7435/non-us-ascii-characters-dropped-from-full-profile-url/7696#7696
        /// </summary>
        /// <param name="c">Charcter to remap</param>
        /// <returns>Remapped character</returns>
        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }

    }
}
