using System;
using System.Collections.Generic;
using System.Text;

namespace WpfABPSimpleCodeGenerator
{
    public static class Helper
    {
        public static string ToEngString(string word)
        {
            string result = "";

            var p = word[0].ToString().ToLower()[0];
            var q = word[word.Length - 1];
            if (p == 'y' || p == 'Y')
            {
                word = word.Substring(word.Length - 1, 1);
                result = word + "es";
            }
            else if (p == 'o' || p == 'x' || p == 's' || (p == 'h' && q == 'c') || (p == 'h' && q == 's'))
            {
                result = word + "es";
            }
            else
            {
                result = word + "s";
            }

            return result;
        }
    }
}
