using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using Rc522;
using RFIDReader.Entity;
using System;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Devices.Gpio;
using System.Threading.Tasks;
using Badge.EF.Entity;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RFIDReader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DeviceClient _client;
        private const int GPIO_PIN_BLUE = 27;
        private const int GPIO_PIN_RED = 22;
        private const int GPIO_PIN_GREEN = 17;
        private GpioPin _pinBlueLed;
        private GpioPin _pinRedLed;
        private GpioPin _pinGreenLed;
        //TODO inserire in un processo di provisioning
        private static string DeviceId = "RaspberryRoboval";

        public MainPage()
        {
            this.InitializeComponent();
            //TODO: Creare un file e leggerlo
            _client = DeviceClient.CreateFromConnectionString(
            $"HostName=roboval.azure-devices.net;DeviceId={DeviceId};SharedAccessKey=62W9nTfaUGjIHlRMV9STKDNcmeGwgOO6l7dKeP9JVqE="
            , Microsoft.Azure.Devices.Client.TransportType.Amqp);

            _pinBlueLed = GpioController.GetDefault().OpenPin(GPIO_PIN_BLUE);
            _pinRedLed = GpioController.GetDefault().OpenPin(GPIO_PIN_RED);
            _pinGreenLed = GpioController.GetDefault().OpenPin(GPIO_PIN_GREEN);

            Task read = Task.Run(() => 
            {
                Task t2 = RiceviDatiAsync();
                Task t1 = ReadAsync();
                
                Task.WaitAll(t1, t2);
            }); // Lancio il task in backgroud              
        }

        private async Task ReadAsync()
        {
            var mfrc = new Mfrc522();
            await mfrc.InitIOAsync();            
            await ResetAsync();

            while (true)
            {
                try
                {
                    if (mfrc.IsTagPresent())
                    {
                        Uid uid = mfrc.ReadUid();
                        if (uid.IsValid)
                        {

                            await CheckingAsync(uid);
                            DataBadge badge = new DataBadge
                            {
                                Orario = DateTime.Now,
                                Id = uid.FullUid,
                                Posizione = "Villafranca"
                            };

                            _pinBlueLed.SetDriveMode(GpioPinDriveMode.Output);
                            _pinBlueLed.Write(GpioPinValue.High);
                            
                            var messageString = JsonConvert.SerializeObject(badge);
                            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));
                            message.Properties.Add("DeviceId", DeviceId);
                            await _client.SendEventAsync(message);
                        }
                        else
                            await ErrorReadAsync();

                        mfrc.HaltTag();
                    }
                }
                catch (Exception ex)
                {
                    await _client.CloseAsync();
                    var e = ex;
                }
            }
        }

        public async Task RiceviDatiAsync()
        {
            while (true)
            {

                Microsoft.Azure.Devices.Client.Message persona = await _client.ReceiveAsync();

                // Se non arrivano messaggi nell'ultimo minuto (default value) il messaggio risulta NULL.
                if (persona == null) continue;

                // Le operazioni sul messaggio dovranno avere la logica di IDEMPOTENZA.
                // L'esecuzione dell'azione potrebbe fallire e il messaggio potrebbe essere ripristinato

                try
                {
                    string messaggio = Encoding.ASCII.GetString(persona.GetBytes());

                    _pinBlueLed.SetDriveMode(GpioPinDriveMode.Output);
                    _pinBlueLed.Write(GpioPinValue.Low);

                    if (messaggio == null)
                    {
                        _pinRedLed.SetDriveMode(GpioPinDriveMode.Output);
                        _pinRedLed.Write(GpioPinValue.High);
                        await ErrorReadAsync();
                    }
                    else
                    {
                        _pinGreenLed.SetDriveMode(GpioPinDriveMode.Output);
                        _pinGreenLed.Write(GpioPinValue.High);
                        
                        Person personaPresente = JsonConvert.DeserializeObject<Person>(messaggio);
                        await SuccesReadAsync(personaPresente);
                        if (!string.IsNullOrEmpty(personaPresente.Uri))
                        {
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                PersonImage.Source = new BitmapImage(new Uri(personaPresente.Uri));
                            });
                        }

                        await ResetAsync();
                    }
                    await _client.CompleteAsync(persona); // sblocco il messaggio e notifico che è stato ricevuto correttamente

                }
                catch (Exception)
                {
                    await _client.AbandonAsync(persona);
                }
                
                await ResetAsync();
            }
        }


        private async Task SuccesReadAsync(Person person)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Result.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
                Uid.Text = $"{person.Nome} {person.Cognome} - {person.Professione} ";
            });
        }

        private async Task CheckingAsync(Uid uid)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Result.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
                Uid.Text = $"{uid.FullUid[0]} - {uid.FullUid[1]} - {uid.FullUid[2]} - {uid.FullUid[3]} - {uid.FullUid[4]}";
            });
        }

        private async Task ErrorReadAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Task.Delay(5000);
                if (_pinRedLed.Read() == GpioPinValue.High)
                {
                    _pinRedLed.Write(GpioPinValue.Low);
                }
                Result.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
                Uid.Text = "Persona non riconosciuta";
            });
            await ResetAsync();
        }

        private async Task ResetAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            await ResetReadAsync();
        }

        private async Task ResetReadAsync()
        {
            if(_pinGreenLed.Read() == GpioPinValue.High)
            {
                _pinGreenLed.Write(GpioPinValue.Low);
            }
           
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Result.Fill = new SolidColorBrush(Windows.UI.Colors.LightGray);
                Uid.Text = "Passa il Badge";
                PersonImage.Source = null;
            });
        }
    }
}
