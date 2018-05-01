using Badge.EF;
using Badge.EF.Entity;
using Badge.WebJob.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTHub.Server
{
    /// <summary>
    /// Ascolta i messaggi dai dispositivi
    /// </summary>
    public class AscoltatoreDispositivi
    {
        // Endopoint, di default, sul quale il server riceve i messaggi
        private const string EndPointServer = "messages/events";
        // Endopoint, di default, sul quale i dispositivi mandano al server l'ACK. 
        //private const string Feedback = "messages/servicebound/feedback";

        private MittenteServer _serverSender;
        public BadgeContext context;
        public EventHubClient HubClient { get; set; }
        private string _connectionString;
        //private MittenteServer _serverSender;

        public AscoltatoreDispositivi(string connectionString, string dbConnectionString)
        {
            _connectionString = connectionString;

            HubClient = EventHubClient.CreateFromConnectionString(connectionString, EndPointServer);
            _serverSender = new MittenteServer(_connectionString);
            DbContextOptionsBuilder<BadgeContext> option = new DbContextOptionsBuilder<BadgeContext>(new DbContextOptions<BadgeContext>());
            option.UseSqlServer(dbConnectionString);
            context = new BadgeContext(option.Options);
        }

        /// <summary>
        /// Ascolta i messaggi dei dispositivi
        /// </summary>
        /// <param name="partitionId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Ogni volta che arriva un payload di dati:
        /// 1) deserializzo l'oggetto
        /// 2) invoco un metodo registrato sul device (un particolare device Id)
        /// 3) invio il messaggio a un device (un particolare device Id). Il messaggio inviato ha richiesto esplicitamente la risposta dal device tramite ACK.
        /// </remarks>
        /// 

        public async Task RicezioneMessaggiDaDispositivi(string partitionId)
        {
            var eventHubReceiver = HubClient.GetDefaultConsumerGroup().CreateReceiver(partitionId, DateTime.UtcNow);
            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                DataBadge nuovoDatoRicevuto = JsonConvert.DeserializeObject<DataBadge>(data);

                try
                {
                    bool thereisperson = context.Badges
                    .Any(x => x.Array == nuovoDatoRicevuto.Id);
                    var property = eventData.Properties.FirstOrDefault(x => x.Key == "DeviceId");
                    if (!thereisperson)
                    {
                        Console.WriteLine("Errore non esiste la persona");
                        string messageToSend = JsonConvert.SerializeObject(null);
                        await _serverSender.InviaAsync(property.Value.ToString(), messageToSend);
                    }
                    else
                    {
                        Console.WriteLine("La persona è presente");
                   
                        PopulateBadge currentBadge = await context.Badges
                            .Where(x => x.Array == nuovoDatoRicevuto.Id)
                            .FirstOrDefaultAsync();
                        
                        //TODO associare una persona a un badge a una macchina.
                        Machine machine = await context.Machines.FirstOrDefaultAsync();
                       
                        Swipe swipe1 = new Swipe()
                        {
                            Badge = currentBadge,
                            Machine = machine,
                            NomeBadge = currentBadge.NomeBadge,
                            MachineName = machine.Name,
                            Orario = nuovoDatoRicevuto.Orario,
                            PosPersona = nuovoDatoRicevuto.Posizione
                        };
                        context.Swipe.Add(swipe1);
                        await context.SaveChangesAsync();

                        Console.WriteLine($"Dato scodato dal server: {nuovoDatoRicevuto}");

                        // Faccio scattare il metodo registrato dal device

                        Person currentPerson = await context.People.FindAsync(currentBadge.IdPerson);
                        currentPerson.Badge = null;
                        string messageToSend = JsonConvert.SerializeObject(currentPerson);
                        
                        await _serverSender.InviaAsync(property.Value.ToString(), messageToSend);                       
                    }
                }

                catch(Exception ex)
                {
                    //TODO: Log
                    Console.WriteLine(ex.Message);   
                }
  
            }
        }

    }
}
