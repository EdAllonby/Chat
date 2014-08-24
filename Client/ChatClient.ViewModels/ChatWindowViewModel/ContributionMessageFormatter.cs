using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.ChatWindowViewModel
{
    /// <summary>
    /// Holds the logic of formatting a new <see cref="Contribution"/> message to be displayed by a control that can display FlowDocuments.
    /// </summary>
    public sealed class ContributionMessageFormatter
    {
        private readonly Brush receiverBackground = new SolidColorBrush(Color.FromArgb(50, 250, 100, 200));
        private readonly Brush senderBackground = new SolidColorBrush(Color.FromArgb(70, 135, 206, 250));
        private readonly int userId;
        private readonly IReadOnlyEntityRepository<User> userRepository;

        private DateTime lastMessageSentDate = new DateTime(1999, 01, 01);
        private int lastUserId;

        /// <summary>
        /// Create a new instance of <see cref="ContributionMessageFormatter"/> linked to the user's Id and its user repository.
        /// </summary>
        /// <param name="userId">The user Id of the client.</param>
        /// <param name="userRepository">The client's repository holding the known users.</param>
        public ContributionMessageFormatter(int userId, IReadOnlyEntityRepository<User> userRepository)
        {
            this.userId = userId;
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Formats a contribution to be viewable on the screen. To be used with a <see cref="FlowDocument"/>.
        /// </summary>
        /// <param name="contribution">The <see cref="Contribution"/> to format.</param>
        /// <returns>A <see cref="Paragraph"/> that is added to a <see cref="FlowDocument"/> to be sent to a view Control.</returns>
        public Paragraph FormatContribution(Contribution contribution)
        {
            Paragraph paragraph = FormatParagraph(contribution);
            Run user = FormatUser(contribution);

            Run message = FormatMessageBody(contribution);

            Run timeStamp = FormatTimeStamp(contribution);

            if (!IsSameSenderAsLastContribution(contribution.ContributorUserId))
            {
                paragraph.Inlines.Add(user);
                paragraph.Inlines.Add(new LineBreak());
            }

            paragraph.Inlines.Add(message);

            if (IsLastMessageOld(contribution))
            {
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(timeStamp);
            }

            lastUserId = contribution.ContributorUserId;
            lastMessageSentDate = contribution.MessageTimeStamp;

            return paragraph;
        }

        private bool IsContributor(int contributorUserId)
        {
            return userId == contributorUserId;
        }

        private bool IsSameSenderAsLastContribution(int newUserId)
        {
            return lastUserId == newUserId;
        }

        private Paragraph FormatParagraph(Contribution contribution)
        {
            TextAlignment alignment = IsContributor(contribution.ContributorUserId) ? TextAlignment.Right : TextAlignment.Left;
            Brush brush = IsContributor(contribution.ContributorUserId) ? senderBackground : receiverBackground;

            int marginSpacing = 50;
            int messageSpacing = 2;

            if (!IsSameSenderAsLastContribution(contribution.ContributorUserId))
            {
                messageSpacing = 20;
            }

            Thickness messageMargin = IsContributor(contribution.ContributorUserId)
                ? new Thickness(marginSpacing, messageSpacing, 0, 0)
                : new Thickness(0, messageSpacing, marginSpacing, 0);
            return new Paragraph
            {
                TextAlignment = alignment,
                Background = brush,
                Margin = messageMargin,
            };
        }

        private Run FormatUser(Contribution contribution)
        {
            return new Run(userRepository.FindEntityById(contribution.ContributorUserId).Username)
            {
                FontWeight = FontWeights.Bold
            };
        }

        private static Run FormatMessageBody(Contribution contribution)
        {
            return new Run(contribution.Message);
        }

        private static Run FormatTimeStamp(Contribution contribution)
        {
            return new Run(contribution.MessageTimeStamp.ToString("HH:mm:ss dd/MM/yyyy", new CultureInfo("en-GB")))
            {
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.DarkGray
            };
        }

        private bool IsLastMessageOld(Contribution newContribution)
        {
            const int TimeThresholdInSeconds = 10;
            TimeSpan duration = newContribution.MessageTimeStamp - lastMessageSentDate;

            return duration.TotalSeconds > TimeThresholdInSeconds;
        }
    }
}