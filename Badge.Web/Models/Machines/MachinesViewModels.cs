using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Badge.Web.Models.Machines
{
    public class MachinesViewModel
    {
            [Key]
            public string Nome { get; set; }

            [StringLength(16)]
            //[RegularExpression("[0-9]+'.'")]
            public string IpMachine { get; set; }

            [StringLength(18)]
            //[RegularExpression("[A-F]+[0-9]+'-'")]
            public string MacAddress { get; set; }
    }
}
    

