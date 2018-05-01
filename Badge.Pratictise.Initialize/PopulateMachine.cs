using Badge.EF.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Badge.Practise.Initialize
{
   public class PopulateMachine
    {
        public Machine Populate(string ipMachine, string MacAddress)
        {
            Machine m = new Machine();
            m.IpMachine = ipMachine;
            m.MacAddress = MacAddress;
            return m;
        }
    }
}
