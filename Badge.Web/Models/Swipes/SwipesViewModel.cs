using Badge.Web.Models.Nuova_cartella;
using Badge.Web.Models.Swipes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Badge.Web.Models.Swipes
{
    public class SwipesViewModel
    {
        [key]
        public int IdSwipe { get; set; }

        [DataType(DataType.Text)]
        [Display( Name ="Posizione")]
        public string PosPersona { get; set; }

        public string selnomebadge;

        public DateTime Orario { get; set; }

        [Display(Name = "Machine name")]
        public string MachineName { get; set; }

        [Display(Name = "Nome badge")]
        public string NomeBadge { get; set; }

        public int IdPerson { get; set; }
    }
}
