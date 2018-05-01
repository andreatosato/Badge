using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace TestLed
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int GPIO_PIN_GREEN = 17;
        const int GPIO_PIN_BLUE = 27;
        const int GPIO_PIN_RED = 22;
        GpioPin pinLed;

        public MainPage()
        {
            this.InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            pinLed = GpioController.GetDefault().OpenPin(GPIO_PIN_GREEN);
            pinLed.SetDriveMode(GpioPinDriveMode.Output);
            pinLed.Write(GpioPinValue.High);   
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            pinLed = GpioController.GetDefault().OpenPin(GPIO_PIN_BLUE);
            pinLed.SetDriveMode(GpioPinDriveMode.Output);
            pinLed.Write(GpioPinValue.High);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            pinLed = GpioController.GetDefault().OpenPin(GPIO_PIN_RED);
            pinLed.SetDriveMode(GpioPinDriveMode.Output);
            pinLed.Write(GpioPinValue.High);

        }
    }
}

