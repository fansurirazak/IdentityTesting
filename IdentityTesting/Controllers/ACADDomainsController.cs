using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityTesting.Data;
using IdentityTesting.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityTesting.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ACADDomainsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ACADDomainsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ACADDomains
        public async Task<IActionResult> Index()
        {
              return _context.ACADDomains != null ? 
                          View(await _context.ACADDomains.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ACADDomains'  is null.");
        }

        // GET: ACADDomains/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ACADDomains == null)
            {
                return NotFound();
            }

            var aCADDomain = await _context.ACADDomains
                .FirstOrDefaultAsync(m => m.ACADDomainID == id);
            if (aCADDomain == null)
            {
                return NotFound();
            }

            return View(aCADDomain);
        }

        // GET: ACADDomains/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ACADDomains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ACADDomainID,ACADDomainName")] ACADDomain aCADDomain)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aCADDomain);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aCADDomain);
        }

        // GET: ACADDomains/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ACADDomains == null)
            {
                return NotFound();
            }

            var aCADDomain = await _context.ACADDomains.FindAsync(id);
            if (aCADDomain == null)
            {
                return NotFound();
            }
            return View(aCADDomain);
        }

        // POST: ACADDomains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ACADDomainID,ACADDomainName")] ACADDomain aCADDomain)
        {
            if (id != aCADDomain.ACADDomainID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aCADDomain);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ACADDomainExists(aCADDomain.ACADDomainID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aCADDomain);
        }

        // GET: ACADDomains/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ACADDomains == null)
            {
                return NotFound();
            }

            var aCADDomain = await _context.ACADDomains
                .FirstOrDefaultAsync(m => m.ACADDomainID == id);
            if (aCADDomain == null)
            {
                return NotFound();
            }

            return View(aCADDomain);
        }

        // POST: ACADDomains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ACADDomains == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ACADDomains'  is null.");
            }
            var aCADDomain = await _context.ACADDomains.FindAsync(id);
            if (aCADDomain != null)
            {
                _context.ACADDomains.Remove(aCADDomain);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ACADDomainExists(int id)
        {
          return (_context.ACADDomains?.Any(e => e.ACADDomainID == id)).GetValueOrDefault();
        }
    }
}
