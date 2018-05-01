using Badge.EF.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Badge.Practise.Initialize
{
    public class PopulateBadge
    {
        public List<int> idperson = new List<int>();
        public EF.Entity.PopulateBadge Populate(Person p, string nomeBadge, byte[] vettore)
        {
            EF.Entity.PopulateBadge badge = new EF.Entity.PopulateBadge();
            badge.NomeBadge = nomeBadge;
            badge.Array = vettore;
            badge.Person = p;
            return badge;
        }
    }
}
