using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Media.Capture;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MicrosoftCognitive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CameraCaptureUI captureUI = new CameraCaptureUI();
        StorageFile photo;
        IRandomAccessStream imageStream;


        const string APIKEY = "cf9141e94fbe4c40b7561b39f63075d8";
        EmotionServiceClient emotionServiceClient = new EmotionServiceClient(APIKEY);

        Emotion[] emotionResult;
        public MainPage()
        {
            this.InitializeComponent();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedAspectRatio = new Size(200, 200);
        }

        private async void takePhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if(photo == null)
                {
                    //User canacelled photo
                    return;
                }
                else
                {
                    imageStream = await photo.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                    await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

                    image.Source = bitmapSource;
                }
            }
            catch
            {
                output.Text = "Error taking photo";
            }
        }

        private async void getEmotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                emotionResult = await emotionServiceClient.RecognizeAsync(imageStream.AsStream());

                if(emotionResult != null)
                {
                    Scores score = emotionResult[0].Scores;
                    output.Text = "Your emotions are: \n" +
                        "Happiness: " + score.Happiness + "\n" +
                        "Sadness: " + score.Sadness + "\n" +
                        "Fear: " + score.Fear + "\n" +
                        "Anger: " + score.Anger + "\n" +
                        "Neutral: " + score.Neutral + "\n" +
                        "Disgust: " + score.Disgust + "\n" +
                        "Conempt" + score.Contempt + "\n";
                }
            }
            catch
            {
                output.Text = "Error returing emotion";
            }
        }
    }
}
