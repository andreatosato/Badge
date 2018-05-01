using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDReader.Entity
{
    public class DataBadge
    {
        public DateTime Orario { get; set; }
        public byte[] Id { get; set; }
        public string Posizione { get; set; }
        public string MachineName { get; set; }
        public string NomeBadge { get; set; }
        public int IdSwipe { get; set; }
    }
}
