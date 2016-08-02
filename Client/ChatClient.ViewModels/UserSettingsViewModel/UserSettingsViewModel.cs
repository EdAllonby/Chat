using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using ChatClient.Services;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Properties;
using GongSolutions.Wpf.DragDrop;
using SharedClasses;

namespace ChatClient.ViewModels.UserSettingsViewModel
{
    public sealed class UserSettingsViewModel : ViewModel, IDropTarget
    {
        private readonly IClientService clientService;

        private Image avatar = Resources.DefaultDropImage;

        private bool isImageChangedSinceLastApply;

        public UserSettingsViewModel(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            clientService = serviceRegistry.GetService<IClientService>();
        }

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

        public ICommand ApplyAvatarCommand => new RelayCommand(SendAvatarRequest, CanSendAvatarRequest);

        public ICommand ApplyAvatarCommandAndClose => new RelayCommand(SendAvatarRequestAndClose);

        public ICommand CancelCommand => new RelayCommand(OnCloseUserSettingsRequest);

        public void DragOver(IDropInfo dropInfo)
        {
            string filename = ((DataObject) dropInfo.Data).GetFileDropList()[0];

            Image image;
            if (ImageUtilities.TryLoadImageFromFile(filename, out image))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// Handles when something is dropped onto the avatar preview box.
        /// </summary>
        /// <param name="dropInfo">The information of the drop.</param>
        public void Drop(IDropInfo dropInfo)
        {
            string imageLocation = ((DataObject) dropInfo.Data).GetFileDropList()[0];

            ApplyAvatarToPreviewBox(imageLocation);
        }

        /// <summary>
        /// Set the avatar preview box to recently selected image.
        /// <see cref="imageLocation" />  doesn't need to be an image, checks are made in this method.
        /// </summary>
        /// <param name="imageLocation">The location of the image in the file system.</param>
        public void ApplyAvatarToPreviewBox(string imageLocation)
        {
            Image image;
            if (ImageUtilities.TryLoadImageFromFile(imageLocation, out image))
            {
                Avatar = image;

                // Force buttons to enable
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public event EventHandler CloseUserSettingsWindowRequest;

        private void SendAvatarRequest()
        {
            if (isImageChangedSinceLastApply)
            {
                clientService.SendAvatarRequest(Avatar);
                isImageChangedSinceLastApply = false;
            }
        }

        private void SendAvatarRequestAndClose()
        {
            if (isImageChangedSinceLastApply)
            {
                clientService.SendAvatarRequest(Avatar);
            }

            OnCloseUserSettingsRequest();
        }

        private bool CanSendAvatarRequest()
        {
            return isImageChangedSinceLastApply;
        }

        private void OnCloseUserSettingsRequest()
        {
            EventHandler closeUserSettingsWindowRequestCopy = CloseUserSettingsWindowRequest;
            if (closeUserSettingsWindowRequestCopy != null)
            {
                closeUserSettingsWindowRequestCopy(this, EventArgs.Empty);
            }
        }
    }
}