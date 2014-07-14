using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Properties;
using GongSolutions.Wpf.DragDrop;

namespace ChatClient.ViewModels.Test
{
    public sealed class UserSettingsViewModel : ViewModel, IDropTarget
    {
        private Image avatar = Resources.DefaultDropImage;

        public Image Avatar
        {
            get { return avatar; }
            set
            {
                avatar = value;
                OnPropertyChanged();
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            string filename = ((DataObject) dropInfo.Data).GetFileDropList()[0];

            Image image;
            if (TryLoadImageFromFile(filename, out image))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            string filename = ((DataObject) dropInfo.Data).GetFileDropList()[0];

            Image image;
            if (TryLoadImageFromFile(filename, out image))
            {
                Avatar = image;

                // Force buttons to enable
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand SendAvatarRequestCommand
        {
            get { return new RelayCommand(SendAvatarRequest, CanSendAvatarRequest);}
        }

        private bool CanSendAvatarRequest()
        {
            return Avatar != null;
        }

        private void SendAvatarRequest()
        {
            ClientService.SendAvatarRequest(Avatar);
        }

        private static bool TryLoadImageFromFile(string filename, out Image image)
        {
            image = null;

            using (FileStream fileStream = File.OpenRead(filename))
            {
                if (fileStream.IsJpegImage() || fileStream.IsPngImage())
                {
                    Bitmap bitmap = new Bitmap(fileStream);
                    Bitmap scaledBitmap = new Bitmap(bitmap, 300, 300);
                    image = scaledBitmap;
                    return true;
                }

                return false;
            }
        }
    }
}