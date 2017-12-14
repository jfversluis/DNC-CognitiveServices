using System;
using System.Linq;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using Plugin.Media;
using Xamarin.Forms;

namespace DNCCognitiveServices
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Handle_Clicked_Vision(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert("Whoops", "The camera is not available. Are you running on a emulator?", "Let me check...");

                return;
            }

            var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions());

            if (photo != null)
            {
                PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });

                #error Enter your own Vision API key here
                var client = new VisionServiceClient("KEY-HERE",
                                                     "https://westeurope.api.cognitive.microsoft.com/vision/v1.0");

                var result = await client.DescribeAsync(photo.GetStream());

                var topCaption = result.Description.Captions.OrderBy(c => c.Confidence).FirstOrDefault();

                var caption = string.Empty;

                if (topCaption == null)
                    caption = "Oh no, I can't find any smart caption for this picture... Sorry!";
                else
                    caption = $" I think I see: {topCaption.Text}, I am {Math.Round(topCaption.Confidence*100, 2)}% sure";

                await DisplayAlert("Result", caption, "Wow, thanks!");
            }
        }

        private async void Handle_Clicked_Emotion(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert("Whoops", "The camera is not available. Are you running on a emulator?", "Let me check...");

                return;
            }

            var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions());

            if (photo != null)
            {
                PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });

                #error Enter your own Emotion API key here
                var client = new EmotionServiceClient("KEY-HERE",
                                                      "https://westus.api.cognitive.microsoft.com/emotion/v1.0");

                var result = await client.RecognizeAsync(photo.GetStream());

                var topEmotion = result.FirstOrDefault()?.Scores?.ToRankedList().FirstOrDefault();

                var caption = string.Empty;

                if (topEmotion == null || !topEmotion.HasValue)
                    caption = "Oh no, no face or emotion could be detected. Are you Vulcan?";
                else
                    caption = $"{topEmotion.Value.Key} is the emotion that comes to mind..";

                await DisplayAlert("Result", caption, "Wow, thanks!");
            }
        }
    }
}