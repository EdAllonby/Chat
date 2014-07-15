using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Properties;
using GongSolutions.Wpf.DragDrop;

namespace ChatClient.ViewModels.UserSettingsViewModel
{
    public sealed class UserSettingsViewModel : ViewModel, IDropTarget
    {
        private Image avatar = Resources.DefaultDropImage;

        private bool isImageChangedSinceLastApply;

        public Image Avatar
        {
            get { return avatar; }
            set
            {
                avatar = value;

                isImageChangedSinceLastApply = true;
                
                OnPropertyChanged();
            }
        }

        public event EventHandler CloseUserSettingsWindowRequest;

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

        public ICommand ApplyAvatarCommand
        {
            get
            {
                return new RelayCommand(SendAvatarRequest, CanSendAvatarRequest);
            }
        }

        public ICommand ApplyAvatarCommandAndClose
        {
            get
            {
                return new RelayCommand(SendAvatarRequestAndClose);
            }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(OnCloseUserSettingsRequest); }
        }

        private void SendAvatarRequest()
        {
            if (isImageChangedSinceLastApply)
            {
                ClientService.SendAvatarRequest(Avatar);
                isImageChangedSinceLastApply = false;
            }
        }

        private void SendAvatarRequestAndClose()
        {
            if (isImageChangedSinceLastApply)
            {
                ClientService.SendAvatarRequest(Avatar);
            }

            OnCloseUserSettingsRequest();
        }

        private bool CanSendAvatarRequest()
        {
            return isImageChangedSinceLastApply;
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

        private void OnCloseUserSettingsRequest()
        {
            var closeUserSettingsWindowRequestCopy = CloseUserSettingsWindowRequest;
            if (closeUserSettingsWindowRequestCopy != null)
            {
                closeUserSettingsWindowRequestCopy(this, EventArgs.Empty);
            }
        }
    }
}