using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Badge.Web.Models.People
{
    public class PeopleViewModel
    {
        [Key]
        public int IdPerson { get; set; }

        [StringLength(60)]
        [DataType(DataType.Text)]
        [Display(Name = "Nome di battesimo")]
        public string Nome { get; set; }

        [StringLength(60)]
        public string Cognome { get; set; }

        [StringLength(40)]
        public string Professione { get; set; }

        public string Uri { get; set; }

        public IFormFile AvatarImage { get; set; }
        
        [Display(Name = "Numero badge")]
        public int CountBadge { get; set; }
    }
}
