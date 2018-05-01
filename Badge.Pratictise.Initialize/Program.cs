using System.Text;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Badge.EF.Entity;
using Badge.EF;

namespace Badge.Practise.Initialize
{
    class Program
    {
        static void Main(string[] args)
        {

            DbContextOptionsBuilder<BadgeContext> option = new DbContextOptionsBuilder<BadgeContext>(new DbContextOptions<BadgeContext>());
            option.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Badge;Trusted_Connection=True;MultipleActiveResultSets=true");
            BadgeContext db = new BadgeContext(option.Options);
            BadgeContextInitializer.Initialize(db);

            PopulatePerson p = new PopulatePerson();
            byte[] BadgeValueOne = { 144, 86, 10, 133, 73 };
            byte[] BadgeValueTwo = { 106, 60, 175, 41, 208 };
            Person persona1 = p.Populate("Andrea","Tosato","Sviluppatore");
            persona1.Uri = "https://roboval2018.blob.core.windows.net/images/1/profilo.JPG";
            Person persona2 = p.Populate("Mario", "Rossi", "Sistemista");
            db.People.Add(persona1);
            db.People.Add(persona2);
            db.SaveChanges();

            PopulateMachine m = new PopulateMachine();
            string IpMachine1 = "172.168.0.1";
            string IpMachine2 = "172.168.0.2";
            string MacAddress1 = "DD-5A-38-A2-D3-27";
            string MacAddress2 = "DF-7B-7A-22-A2-60";
            Machine machine1 = m.Populate(IpMachine1, MacAddress1);
            Machine machine2 = m.Populate(IpMachine2, MacAddress2);
            db.Machines.Add(machine1);
            db.Machines.Add(machine2);
            db.SaveChanges();

            PopulateBadge b = new PopulateBadge();
            string nomeBadge1 = "BadgeTosato";
            string nomeBadge2 = "BadgeRossi";
            EF.Entity.PopulateBadge badge1 = b.Populate(persona1, nomeBadge1, BadgeValueOne);
            EF.Entity.PopulateBadge badge2 = b.Populate(persona2, nomeBadge2, BadgeValueTwo);
            Console.WriteLine(badge1.ToString());
            PopulateBadge id = new PopulateBadge();
            id.idperson.Add(persona1.IdPerson);
            id.idperson.Add(persona2.IdPerson);
            db.Badges.Add(badge1);
            db.Badges.Add(badge2);
            db.SaveChanges();

            //PopulateSwipe s = new PopulateSwipe();
            //DateTime orario = DateTime.Now;
            //string pospersona = "Villafranca";
            //Swipe swipe1 = s.Populate(orario,badge1,pospersona,machine1);
            //Swipe swipe2 = s.Populate(orario, badge1, pospersona, machine1);
            //Swipe swipe3 = s.Populate(orario, badge1, pospersona, machine2);
            //Swipe swipe4 = s.Populate(orario, badge2, pospersona, machine2);
            //Swipe swipe5 = s.Populate(orario, badge2, pospersona, machine2);
            //Swipe swipe9 = s.Populate(orario, badge1, pospersona, machine1);
            //db.Swipe.Add(swipe1);
            //db.Swipe.Add(swipe2);
            //db.Swipe.Add(swipe3);
            //db.Swipe.Add(swipe4);
            //db.Swipe.Add(swipe5);
            //db.Swipe.Add(swipe9);
            //db.SaveChanges();

            Console.ReadKey();
        }
    }
}