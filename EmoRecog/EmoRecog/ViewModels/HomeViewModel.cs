﻿using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using System.Threading.Tasks;
using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;

namespace EmoRecog.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public Command TakePhotoCMD { get; set; }
        public Command PickPhotoCMD { get; set; }
        public Command TakeVideoCMD { get; set; }
        public Command PickVideoCMD { get; set; }
        public ImageSource imageSource { get; set; }
        public AlertConfig alert { get; set; }
        public HomeViewModel()
        {
            TakePhotoCMD = new Command(async () => await TakeAPhoto());
            PickPhotoCMD = new Command(async () => await PickPhoto());
            TakeVideoCMD = new Command(async () => await TakeAVideo());
            PickVideoCMD = new Command(async () => await PickAVideo());
            alert = new AlertConfig();
        }

        private async Task Send(MediaFile file)
        {
            UserDialogs.Instance.ShowLoading("Sending");
            await Task.Run(async () =>
            {
                await Networking.SendPhoto(file.GetStream());
                UserDialogs.Instance.HideLoading();
                alert.Title = "HeyThe";
                alert.Message = "The Photo has been Sent";
                UserDialogs.Instance.Alert(alert);
            });
        }

        private async Task Connect()
        {
            UserDialogs.Instance.ShowLoading("Connecting");
            string msg = await Networking.Connect();
            UserDialogs.Instance.HideLoading();
            alert.Title = "Connected To";
            alert.Message = msg;
            UserDialogs.Instance.Alert(alert);
        }

        private async Task PickAVideo()
        {
            if (!CrossMedia.Current.IsPickVideoSupported)
            {
                alert.Title = "No VIDEOS Not Supported";
                alert.Message = ":( Permission not granted to VIDEOS.";
                UserDialogs.Instance.Alert(alert);
                return;
            }
            await Connect();
            var file = await CrossMedia.Current.PickVideoAsync();

            if (file == null)
                return;

            file.GetStream();
            alert.Title = "Video Selected";
            alert.Message = "Location: " + file.Path;
            UserDialogs.Instance.Alert(alert);
            await Send(file);
            file.Dispose();
        }

        private async Task TakeAVideo()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                alert.Title = "No Camera";
                alert.Message = ":( No camera available.";
                UserDialogs.Instance.Alert(alert);
                return;
            }
            await Connect();
            var file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
            {
                Name = "video.mp4",
                Directory = "DefaultVideos",
                Quality = VideoQuality.High,
                AllowCropping = true
            });
            if (file == null)
                return;
            alert.Title = "Video Recoreded and Selected";
            alert.Message = "Location: " + file.Path;
            UserDialogs.Instance.Alert(alert);
            await Send(file);
            file.Dispose();
        }

        private async Task PickPhoto()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                alert.Title = "No Photos Not Supporte";
                alert.Message = ":( Permission not granted to photos.";
                UserDialogs.Instance.Alert(alert);
                return;
            }
            await Connect();
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                MaxWidthHeight = 2000
            });
            if (file == null)
                return;
            await Send(file);
            imageSource = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
            OnPropertyChanged(nameof(imageSource));
        }

        private async Task TakeAPhoto()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                alert.Title = "No Camera";
                alert.Message = ":( No camera available.";
                UserDialogs.Instance.Alert(alert);
                return;
            }
            await Connect();
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "EmoRecog",
                SaveToAlbum = true,
                PhotoSize = PhotoSize.Full,
                MaxWidthHeight = 2000,
                DefaultCamera = CameraDevice.Front
            });
            if (file == null)
                return;
            await Send(file);
            imageSource = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
            OnPropertyChanged(nameof(imageSource));
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged Implementation
    }
}
