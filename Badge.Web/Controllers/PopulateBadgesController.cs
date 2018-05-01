using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Badge.EF;
using Badge.EF.Entity;
using Badge.Web.Models.Badges;
using Badge.Web.Models.Shared;
using AutoMapper;

namespace Badge.Web.Controllers
{
    public class PopulateBadgesController : Controller
    {
        private readonly BadgeContext _context;

        public PopulateBadgesController(BadgeContext context)
        {
            _context = context;
        }

        // GET: PopulateBadges
        public async Task<IActionResult> Index(int peopleid, int take = 6, int skip = 0)
        {
            PaginationViewModel<BadgesViewModel> result = new PaginationViewModel<BadgesViewModel>();

            int quantita = await _context.Badges.Where(x => x.IdPerson == peopleid).CountAsync();
            List<PopulateBadge> badges = new List<PopulateBadge>();
            List<BadgesViewModel> badge = new List<BadgesViewModel>();
            badges = await _context.Badges.Where(x => x.IdPerson == peopleid).Skip(skip).Take(take).ToListAsync();
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

            ViewBag.IdPerson = peopleid;
            return View(result);
        }

        // GET: PopulateBadges/Details/5
        public async Task<IActionResult> Details(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }

            var populateBadge = await _context.Badges
                .Include(p => p.Person)
                .SingleOrDefaultAsync(m => m.NomeBadge ==Id);
            if (populateBadge == null)
            {
                return NotFound();
            }

            return View(populateBadge);
        }

        // GET: PopulateBadges/Create
        public IActionResult Create(int idperson)
        {
            ViewBag.IdPerson = idperson;
            return View();
        }

        // POST: PopulateBadges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int idperson,BadgesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newswipes = Mapper.Map<PopulateBadge>(model);
                _context.Add(newswipes);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { peopleid = idperson });
            }
            return View(model);
        }

        // GET: PopulateBadges/Edit/5
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }

            var badge = await _context.Badges.SingleOrDefaultAsync(m => m.NomeBadge == Id);
            BadgesViewModel model = Mapper.Map<BadgesViewModel>(badge);
            ViewBag.IdPerson = Id;
            if (badge == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: PopulateBadges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, int idperson,BadgesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var badge = await _context.Badges.SingleAsync(x => x.NomeBadge == Id);
                Mapper.Map(model, badge);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { peopleid = idperson });
            }

            return View(model);
        }

        // GET: PopulateBadges/Delete/5
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }

            bool haveswipes = _context.Badges
                .Where(m => m.NomeBadge == Id)
                .Any(x => x.Swipes.Any());

            var populateBadge = await _context.Badges
                .Include(p => p.Person)
                .SingleOrDefaultAsync(m => m.NomeBadge == Id);
            if (populateBadge == null)
            {
                return NotFound();
            }

            populateBadge.CanDelete = !haveswipes;
            return View(populateBadge);
        }

        // POST: PopulateBadges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Id, int idperson)
        {
            var populateBadge = await _context.Badges.SingleOrDefaultAsync(m => m.NomeBadge == Id);
            _context.Badges.Remove(populateBadge);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { peopleid = idperson });
        }

        private bool PopulateBadgeExists(string Id)
        {
            return _context.Badges.Any(e => e.NomeBadge == Id);
        }
    }
}
