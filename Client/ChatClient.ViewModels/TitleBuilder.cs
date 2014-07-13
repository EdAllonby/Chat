using System.Collections.Generic;
using System.Text;

namespace ChatClient.ViewModels
{
    internal static class TitleBuilder
    {
        public static string CreateUserList(IEnumerable<string> usernames)
        {
            var titleBuilder = new StringBuilder();

            foreach (string username in usernames)
            {
                titleBuilder.Append(username);
                titleBuilder.Append(" and ");
            }

            titleBuilder.Length = titleBuilder.Length - " and ".Length;

            return titleBuilder.ToString();
        }
    }
}