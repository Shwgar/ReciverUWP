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
using System.Diagnostics;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using Polly;




// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ReciverUWP
{

    public class RootObject
    {
        public string deviceId { get; set; }
        public int messageId { get; set; }
        public bool tempAlert { get; set; }
        public int epocTime { get; set; }
        public string alertMessage { get; set; }
        public double temperature { get; set; }
        public double humidity { get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 


    public sealed partial class MainPage : Page
    {
        static string IotDevice2 = "HostName=IotHub-18.azure-devices.net;DeviceId=UwpAppDevice;SharedAccessKey=uyQG9HA5kXTC5dgUQb4Cut2iVPaZ0cxaqrnF6DhaopQ=";
        public string alertMessage = "";
        public string deviceID = "";
        public int dateTime = 0;
        public double temperature = 0;
        public double humidity = 0;
        public bool alertstatus = false;
        DateTime nowTime;

        public MainPage()
        {
        
            this.InitializeComponent();
            var policy = Policy.Handle<Microsoft.Azure.Devices.Client.Exceptions.IotHubException>().Retry();
            policy.Execute(() => ReceiveC2dAsync());

        }
        
        public async void ReceiveC2dAsync()
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(IotDevice2, TransportType.Mqtt);

            while (true)
            { /*
                  Message receivedMessage = await deviceClient.ReceiveAsync();
                  if (receivedMessage == null) continue;
                  //string recived = Encoding.UTF8.GetString(receivedMessage.GetBytes());
                  //Console.WriteLine("Mottaget meddelande: {0}", Encoding.UTF8.GetString(receivedMessage.GetBytes()));
                  Debug.WriteLine("Mottaget meddelande: {0}", Encoding.UTF8.GetString(receivedMessage.GetBytes()));
                 // inputText = Encoding.UTF8.GetString(receivedMessage.GetBytes());
                  await deviceClient.CompleteAsync(receivedMessage);
                  */



                Message receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage == null) continue;

                var messagejson = Encoding.UTF8.GetString(receivedMessage.GetBytes());
                
                await deviceClient.CompleteAsync(receivedMessage);

                RootObject json = JsonConvert.DeserializeObject<RootObject>(messagejson);

                alertMessage = json.alertMessage;
                dateTime = json.epocTime;
                deviceID = json.deviceId;
                alertstatus = json.tempAlert;
                temperature = json.temperature;
                humidity = json.humidity;
                Debug.WriteLine(alertMessage);
                nowTime = new DateTime(1970, 1, 1,1,1,1).AddSeconds(dateTime);
                TextUpdate();
             }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CenterTextBlock.Text = alertMessage;
        }

        private void TextUpdate()
        {
            CenterTextBlock.Text = alertMessage;
            DateTimeTextBlock.Text = nowTime.ToString();
          //  DateTimeTextBlock.Text = dateTime.ToString();
            DeviceTextBlock.Text = deviceID;
            TemperaturTextBlock.Text = temperature.ToString("0.0") + " C°";
            HumidityTextBlock.Text = humidity.ToString("0.0");
            AlertStatusTextBlock.Text = alertstatus.ToString();
            BackgroundImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///warningback.png"));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BackgroundImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///warningbackStandard.png"));
        alertMessage = "";
        deviceID = "";
        dateTime = 0;
        temperature = 0;
        humidity = 0;
        alertstatus = false;
    }
    }

 
}
