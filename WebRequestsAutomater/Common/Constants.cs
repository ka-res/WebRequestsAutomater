using System.Collections.Generic;

namespace WebRequestsAutomater.Common
{
    public static class Constants
    {
        public const string BaseUrl = "TODO enter url";
        public const string LoginUrl = "TODO enter url";

        public const string EmailField = "email";
        public const string PasswordField = "password";
        public const string TokenField = "ci_csrf_token";

        public const string UsersFileName = "users.txt";
        public const string UniquesFileName = "uniques.txt";

        public static List<char> PossibleChoices = new List<char>
        {
            'v',
            'l',
            'x',
            'q'
        };

        public static List<char> PossibleBoolChoices = new List<char>
        {
            'y',
            'n'
        };
    }
}
