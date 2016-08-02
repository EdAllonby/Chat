using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient.ViewModels
{
    internal static class ChatWindowStringBuilder
    {
        public static string CreateUserListTitle(IList<string> usernames)
        {
            if (!usernames.Any())
            {
                return string.Empty;
            }

            var titleBuilder = new StringBuilder();

            titleBuilder.Append("Chat between ");

            titleBuilder.Append(UserListBuilder(usernames));

            return titleBuilder.ToString();
        }

        public static string CreateUsersTypingMessage(IList<string> usersTyping)
        {
            if (usersTyping.Count == 0)
            {
                return string.Empty;
            }

            var usersTypingBuilder = new StringBuilder();

            usersTypingBuilder.Append(UserListBuilder(usersTyping));

            usersTypingBuilder.Append(usersTyping.Count() == 1 ? " is typing..." : " are typing...");

            return usersTypingBuilder.ToString();
        }

        private static string UserListBuilder(ICollection<string> usernames)
        {
            if (usernames.Count == 0)
            {
                return string.Empty;
            }

            var userListBuilder = new StringBuilder();

            string firstUsername = usernames.First();
            string lastUsername = usernames.Last();

            foreach (string username in usernames)
            {
                if (!username.Equals(firstUsername))
                {
                    userListBuilder.Append(!username.Equals(lastUsername) ? ", " : " and ");
                }

                userListBuilder.Append(username);
            }

            return userListBuilder.ToString();
        }
    }
}