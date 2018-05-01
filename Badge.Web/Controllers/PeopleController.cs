using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Badge.Web.Models.Shared;
using Badge.Web.Models.People;
using AutoMapper;
using Badge.Web.Models;
using Badge.Web.Models.Badges;
using Badge.Web.Services;
using Badge.EF.Entity;
using Badge.EF;

namespace Badge.Web.Controllers
{
    public class PeopleController : Controller
    {
        private readonly BadgeContext _context;
        private readonly IUploadBlob _uploadBlobService;
        public PeopleController(BadgeContext context, IUploadBlob uploadBlobService)
        {
            _context = context;
            _uploadBlobService = uploadBlobService;
        }

        // GET: People
        public async Task<IActionResult> Index(int? id, int take = 6, int skip = 0)
        {
            PaginationViewModel<PeopleViewModel> result = new PaginationViewModel<PeopleViewModel>();
            int quantita = await _context.People.CountAsync();
            List<Person> person = new List<Person>();
            person = await _context.People.Skip(skip).Take(take).ToListAsync();
            result.Skip = skip;
            result.Count = quantita;
            int Count1 = 0;

            if (result.Count % 6 == 0)
            {
                Count1 = (result.Count / 6) - 1;
            }
            else
            {
                Count1 = (result.Count / 6);
            }
            
            result.Count = Count1;
            var personBadge = await _context.People
                   .Skip(skip)
                   .Take(take)
                   .Select(x => new { IdPerson = x.IdPerson, CountBadge = x.Badge.Count() })
                   .ToListAsync();

            foreach (var p in person)
            {
                PeopleViewModel pv = new PeopleViewModel()
                {
                    Cognome = p.Cognome,
                    Nome = p.Nome,
                    Professione = p.Professione,
                    Uri = p.Uri,
                    IdPerson = p.IdPerson
                };

                pv.CountBadge = personBadge.
                    FirstOrDefault(x => x.IdPerson == pv.IdPerson).
                    CountBadge;
                result.Data.Add(pv);
            }
            return View(result);
        }


        // GET: People
        public async Task<IActionResult> Badge_people (int peopleid, int take = 6, int skip = 0)
        {
            PaginationViewModel<BadgesViewModel> result = new PaginationViewModel<BadgesViewModel>();
            int quantita = await _context.Badges.Where(x => x.IdPerson == peopleid).CountAsync();
            List<PopulateBadge> badges = new List<PopulateBadge>();
            List<BadgesViewModel> badge = new List<BadgesViewModel>();
            badges = await _context.Badges.Where( x => x.IdPerson ==peopleid).Skip(skip).Take(take).ToListAsync();
            result.Skip = skip;
            result.Count = badges.Count();
            int Countgiri = 0;

            if (result.Count % 6 == 0)
            {
                Countgiri = (result.Count / 6) - 1;
            }
            else
            {
                Countgiri = (result.Count / 6);
            }
            
            result.Count = Countgiri;
            
            foreach (var p in badges)
            {
                BadgesViewModel pv = new BadgesViewModel()
                {
                    NomeBadge = p.NomeBadge,
                    IdPerson = p.IdPerson
                };

                result.Data.Add(pv);    
            }

            return View(result);
        }


        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .SingleOrDefaultAsync(m => m.IdPerson == id);

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PeopleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newperson = Mapper.Map<Person>(model);
                _context.Add(newperson);
                await _context.SaveChangesAsync();

                byte[] result = null;
                using (var memoryStream = new System.IO.MemoryStream())

                {
                    await model.AvatarImage.CopyToAsync(memoryStream);
                    result = memoryStream.ToArray();
                }

                newperson.Uri = await _uploadBlobService.UploadFile(result, $"{newperson.IdPerson}/{model.AvatarImage.FileName}");
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var person = await _context.People.SingleAsync(m => m.IdPerson == id);
            PeopleViewModel model = Mapper.Map<PeopleViewModel>(person);
            return View(model);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PeopleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var person = await _context.People.SingleAsync(x => x.IdPerson == id);
                Mapper.Map(model, person);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            bool haveBadge = _context.People
                .Where(m => m.IdPerson == id)
                .Any(x => x.Badge.Any());

            List<PopulateBadge> badge = new List<PopulateBadge>();
            
            var person = await _context.People
                .SingleOrDefaultAsync(m => m.IdPerson == id);
            if (person == null)
            {
                return NotFound();
            }
            person.CanDelete = !haveBadge;

            return View(person);
        }
        
        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.People.SingleOrDefaultAsync(m => m.IdPerson == id);
            _context.People.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.IdPerson == id);
        }
    }
}
