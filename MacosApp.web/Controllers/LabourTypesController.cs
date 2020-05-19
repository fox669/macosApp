using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MacosApp.Web.Data;
using MacosApp.Web.Data.Entities;

namespace MacosApp.Web.Controllers
{
    public class LabourTypesController : Controller
    {
        private readonly DataContext _context;

        public LabourTypesController(DataContext context)
        {
            _context = context;
        }

        // GET: LabourTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LabourTypes.ToListAsync());
        }

        // GET: LabourTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labourType = await _context.LabourTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (labourType == null)
            {
                return NotFound();
            }

            return View(labourType);
        }

        // GET: LabourTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LabourTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] LabourType labourType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(labourType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(labourType);
        }

        // GET: LabourTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labourType = await _context.LabourTypes.FindAsync(id);
            if (labourType == null)
            {
                return NotFound();
            }
            return View(labourType);
        }

        // POST: LabourTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] LabourType labourType)
        {
            if (id != labourType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(labourType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LabourTypeExists(labourType.Id))
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
            return View(labourType);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labourType = await _context.LabourTypes
                .Include(pt => pt.Labours)
                .FirstOrDefaultAsync(pt => pt.Id == id);
            if (labourType == null)
            {
                return NotFound();
            }

            if(labourType.Labours.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "The labour type can't be removed.");
                return RedirectToAction(nameof(Index));
            }

            _context.LabourTypes.Remove(labourType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LabourTypeExists(int id)
        {
            return _context.LabourTypes.Any(e => e.Id == id);
        }
    }
}
