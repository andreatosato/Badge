using Badge.EF.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Badge.Practise.Initialize
{
    public class PopulatePerson
    {
        public Person Populate(string nome, string cognome, string professione)
        {
            Person p = new Person();
            p.Nome = nome;
            p.Cognome = cognome;
            p.Professione = professione;
            return p;
        }
    }
}
