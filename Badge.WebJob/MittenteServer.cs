using Badge.EF.Entity;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHub.Server
{
    /// <summary>
    /// Invia messaggi verso i dispositivi
    /// </summary>
    class MittenteServer
    {
        public ServiceClient _serviceClient;
        public MittenteServer(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
        }

        public async Task InviaAsync(string deviceId, string objectToSend)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(objectToSend));
            await _serviceClient.SendAsync(deviceId, commandMessage);
            Console.WriteLine($"Invio messaggio al device {objectToSend}");
        }

        /// <summary>
        /// Metodo non utilizzato:
        /// Richiama un metodo registrato sul device.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task InvocaMetodoSulDevice(string deviceId, Person person)
        {
            try
            {
                CloudToDeviceMethod method = new CloudToDeviceMethod("MetodoInvocatoDaServer");
                string personString = JsonConvert.SerializeObject(person);
                method.SetPayloadJson(personString);
                var result = await _serviceClient.InvokeDeviceMethodAsync(deviceId, method);

                string jsonPayload = result.GetPayloadAsJson();
                int status = result.Status;
                Console.WriteLine($"Risposta ricevuta dal server in data {DateTimeOffset.UtcNow} : {status} - {jsonPayload}");
            }
            catch(DeviceNotFoundException ex) // Se il protocollo di comunicazione è HTTP il dispositivo non può essere identificato come ONLINE
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Metodo non utilizzato:
        /// Serve per riceve una notifica di file caricato sul Blob
        /// </summary>
        /// <returns></returns>
        private async Task RiceviNotificaCaricamentoFileAsync()
        {
            var notificationReceiver = _serviceClient.GetFileNotificationReceiver();
            while (true)
            {
                var fileUploadNotification = await notificationReceiver.ReceiveAsync();
                if (fileUploadNotification == null) continue;
                                
                Console.WriteLine($"Received file upload noticiation: {fileUploadNotification.BlobName} - {fileUploadNotification.BlobUri}");
                await notificationReceiver.CompleteAsync(fileUploadNotification);
            }
        }
    }
}
