using System;
namespace UtahCrashesCracked.Models
{
    public class Capitalize
    {
        string UppercaseFirst(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }
    }

}
