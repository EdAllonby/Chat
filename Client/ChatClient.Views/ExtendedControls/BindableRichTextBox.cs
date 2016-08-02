using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ChatClient.Views.ExtendedControls
{
    /// <summary>
    /// A bindable RichTextBox control for use with the MVVM pattern.
    /// </summary>
    public class BindableRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(FlowDocument), typeof(BindableRichTextBox), new FrameworkPropertyMetadata(null, OnDocumentChanged));

        public new FlowDocument Document
        {
            get { return (FlowDocument) GetValue(DocumentProperty); }

            set { SetValue(DocumentProperty, value); }
        }

        private static void OnDocumentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var rtb = (RichTextBox) obj;
            rtb.Document = (FlowDocument) args.NewValue;
        }
    }
}