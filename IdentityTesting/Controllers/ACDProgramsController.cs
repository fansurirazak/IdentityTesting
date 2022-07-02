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
    public class ACDProgramsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ACDProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ACDPrograms
        public async Task<IActionResult> Index()
        {
              return _context.ACDPrograms != null ? 
                          View(await _context.ACDPrograms.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ACDPrograms'  is null.");
        }

        // GET: ACDPrograms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ACDPrograms == null)
            {
                return NotFound();
            }

            var aCDProgram = await _context.ACDPrograms
                .FirstOrDefaultAsync(m => m.ACDProgramID == id);
            if (aCDProgram == null)
            {
                return NotFound();
            }

            return View(aCDProgram);
        }

        // GET: ACDPrograms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ACDPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ACDProgramID,ACDProgramName,ACDProgramCode")] ACDProgram aCDProgram)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aCDProgram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aCDProgram);
        }

        // GET: ACDPrograms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ACDPrograms == null)
            {
                return NotFound();
            }

            var aCDProgram = await _context.ACDPrograms.FindAsync(id);
            if (aCDProgram == null)
            {
                return NotFound();
            }
            return View(aCDProgram);
        }

        // POST: ACDPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ACDProgramID,ACDProgramName,ACDProgramCode")] ACDProgram aCDProgram)
        {
            if (id != aCDProgram.ACDProgramID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aCDProgram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ACDProgramExists(aCDProgram.ACDProgramID))
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
            return View(aCDProgram);
        }

        // GET: ACDPrograms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ACDPrograms == null)
            {
                return NotFound();
            }

            var aCDProgram = await _context.ACDPrograms
                .FirstOrDefaultAsync(m => m.ACDProgramID == id);
            if (aCDProgram == null)
            {
                return NotFound();
            }

            return View(aCDProgram);
        }

        // POST: ACDPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ACDPrograms == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ACDPrograms'  is null.");
            }
            var aCDProgram = await _context.ACDPrograms.FindAsync(id);
            if (aCDProgram != null)
            {
                _context.ACDPrograms.Remove(aCDProgram);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ACDProgramExists(int id)
        {
          return (_context.ACDPrograms?.Any(e => e.ACDProgramID == id)).GetValueOrDefault();
        }
    }
}
