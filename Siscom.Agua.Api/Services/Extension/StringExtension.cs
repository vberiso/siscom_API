using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Services.Extension
{
    public static class StringExtension
    {
        public static string RemoveLast(this string text, string character)
        {
            if (text.Length < 1) return text;
            return text.Remove(text.ToString().LastIndexOf(character), character.Length);
        }
    }
}
