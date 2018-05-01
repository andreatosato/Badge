using Badge.EF.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Badge.Practise.Initialize
{
    public class PopulateSwipe
    {
        public Swipe Populate(DateTime orario, EF.Entity.PopulateBadge b, string posPersona, Machine m)
        {
            Swipe swipe = new Swipe();
            swipe.Orario = orario ;
            swipe.Badge = b;
            swipe.PosPersona = posPersona;
            swipe.Machine = m;
            return swipe;
        }
    }
}
