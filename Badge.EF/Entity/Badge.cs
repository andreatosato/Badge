using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Badge.EF.Entity
{
    [Table("Badge")]//Nome della Tabella nel Database
    public class PopulateBadge
    {
        [Key]
        [Display(Name = "Nome Badge")]
        public string NomeBadge { get; set; }

        public bool CanDelete;

        public byte[] Array { get; set; }

        public List<Swipe> Swipes { get; set; } = new List<Swipe>();

        [Display(Name = "Id Persona")]
        public int IdPerson { get; set; }
        //public bool Idperson { get; set; }
        [ForeignKey("IdPerson")]
        public Person Person { get; set; }

        public override string ToString()
        {
            return $"NomeBadge: {NomeBadge}";
        }
    }
}
