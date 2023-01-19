using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HoloMetrix_API_Explorer
{
    public static class InputChecker
    {
        public static Regex int_Regex = new Regex("[^0-9]+");
        public static Regex float_Regex = new Regex("[^0-9.]+");

        public static bool IsValid(string text, Regex regex)
        {
            return regex.IsMatch(text);
        }
    }
}
