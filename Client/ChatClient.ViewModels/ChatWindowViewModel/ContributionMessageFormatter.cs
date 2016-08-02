using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ChatClient.ViewModels.Converter;
using SharedClasses.Domain;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;

namespace ChatClient.ViewModels.ChatWindowViewModel
{
    /// <summary>
    /// Holds the logic of formatting a new <see cref="TextContribution" /> message to be displayed by a control that can
    /// display FlowDocuments.
    /// </summary>
    public sealed class ContributionMessageFormatter
    {
        private readonly BitmapToBitmapSourceConverter bitmapConverter = new BitmapToBitmapSourceConverter();

        private readonly Brush receiverBackground = new SolidColorBrush(Color.FromArgb(50, 250, 100, 200));
        private readonly Brush senderBackground = new SolidColorBrush(Color.FromArgb(70, 135, 206, 250));
        private readonly int userId;
        private readonly IReadOnlyEntityRepository<User> userRepository;

        private DateTime lastMessageSentDate = new DateTime(1999, 01, 01);
        private int lastUserId;

        /// <summary>
        /// Create a new instance of <see cref="ContributionMessageFormatter" /> linked to the user's Id and its user repository.
        /// </summary>
        /// <param name="userId">The user Id of the client.</param>
        /// <param name="userRepository">The client's repository holding the known users.</param>
        public ContributionMessageFormatter(int userId, IReadOnlyEntityRepository<User> userRepository)
        {
            this.userId = userId;
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Formats a contribution to be viewable on the screen. To be used with a <see cref="FlowDocument" />.
        /// </summary>
        /// <param name="contribution">The <see cref="TextContribution" /> to format.</param>
        /// <returns>A <see cref="Paragraph" /> that is added to a <see cref="FlowDocument" /> to be sent to a view Control.</returns>
        public Paragraph FormatContribution(IContribution contribution)
        {
            Paragraph paragraph = FormatParagraph(contribution);
            Run user = FormatUser(contribution);

            // Here will decide whether the contribution is text or image
            var textContribution = contribution as TextContribution;

            Run message = null;

            InlineUIContainer test = null;

            if (textContribution != null)
            {
                message = FormatMessageBody(textContribution);
            }
            else
            {
                test = FormatMessageBody((ImageContribution) contribution);
            }

            Run timeStamp = FormatTimeStamp(contribution);

            if (!IsSameSenderAsLastContribution(contribution.ContributorUserId))
            {
                paragraph.Inlines.Add(user);
                paragraph.Inlines.Add(new LineBreak());
            }

            if (message != null)
            {
                paragraph.Inlines.Add(message);
            }
            if (test != null)
            {
                paragraph.Inlines.Add(test);
            }

            if (IsLastMessageOld(contribution))
            {
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(timeStamp);
            }

            lastUserId = contribution.ContributorUserId;
            lastMessageSentDate = contribution.ContributionTimeStamp;

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

        private Paragraph FormatParagraph(IContribution contribution)
        {
            TextAlignment alignment = IsContributor(contribution.ContributorUserId) ? TextAlignment.Right : TextAlignment.Left;
            Brush brush = IsContributor(contribution.ContributorUserId) ? senderBackground : receiverBackground;

            var marginSpacing = 50;
            var messageSpacing = 2;

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
                Margin = messageMargin
            };
        }

        private Run FormatUser(IContribution contribution)
        {
            return new Run(userRepository.FindEntityById(contribution.ContributorUserId).Username)
            {
                FontWeight = FontWeights.Bold
            };
        }

        private static Run FormatMessageBody(TextContribution contribution)
        {
            return new Run(contribution.Message);
        }

        private InlineUIContainer FormatMessageBody(ImageContribution contribution)
        {
            var image = new Image { Source = bitmapConverter.Convert((Bitmap) contribution.Image) };
            return new InlineUIContainer(image);
        }

        private static Run FormatTimeStamp(IContribution contribution)
        {
            return new Run(contribution.ContributionTimeStamp.ToString("HH:mm:ss dd/MM/yyyy", new CultureInfo("en-GB")))
            {
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.DarkGray
            };
        }

        private bool IsLastMessageOld(IContribution newContribution)
        {
            const int TimeThresholdInSeconds = 10;
            TimeSpan duration = newContribution.ContributionTimeStamp - lastMessageSentDate;

            return duration.TotalSeconds > TimeThresholdInSeconds;
        }
    }
}