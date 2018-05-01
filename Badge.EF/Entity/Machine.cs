using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Badge.EF.Entity
{
    [Table("Machines")]//Nome della Tabella nel Database

    public class Machine
    {
        public Machine()
        {
        }

        [Key]
        public string Name { get; set; }

        public string IpMachine { get; set; }
        public string MacAddress { get; set; }
        public List<Swipe> Swipes { get; set; } = new List<Swipe>();


    }
}
