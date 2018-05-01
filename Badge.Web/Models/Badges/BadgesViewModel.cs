using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Badge.Web.Models.Badges
{
    public class BadgesViewModel
    {
        [Key]
        [Display(Name = "Nome Badge")]
        public string NomeBadge { get; set; }

        [Display(Name = "Id Persona")]
        public int IdPerson { get; set; }
    }
}
    

