﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquineTracker.Models;
using EquineTracker.Helpers;

namespace EquineTracker.Controllers {
    public class ResultsController : Controller {
        private readonly BegEFCoreContext _context;

        public ResultsController(BegEFCoreContext context) {
            _context = context;
        }

        // Get: Results grouped together by Event
        public ViewResult GroupedResults() {
            var events = _context.Result
                    .GroupBy(e => e.EventId)
                    .Select(g => new { id = g.Key, count = g.Count() });
            List<ResultGroup> lRg = new List<ResultGroup>();
            foreach (var gRes in events) {
                Event e2 = _context.Event.Where(x => x.EventId == gRes.id).FirstOrDefault();
                ResultGroup rg = new ResultGroup() { _Event = e2, ResultCount = gRes.count };
                lRg.Add(rg);
            }
            return View(lRg);
        }

        // GET: Results
        public async Task<IActionResult> Index(int EventId) {
            var begEFCoreContext = _context.Result.Include(r => r.Event).Include(r => r.Horse).Where(e => e.EventId == EventId);
            return View(await begEFCoreContext.ToListAsync());
        }

        // GET: Results/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var result = await _context.Result
                .Include(r => r.Event)
                .Include(r => r.Horse)
                .SingleOrDefaultAsync(m => m.ResultId == id);
            if (result == null) {
                return NotFound();
            }

            return View(result);
        }

        // GET: Results/Create
        public IActionResult Create(int EventId) {
            ViewData["EventId"] = new SelectList(_context.Event.Where(e => e.EventId == EventId), "EventId", "Description");
            ViewData["HorseId"] = new SelectList(_context.Horse, "HorseId", "Name");
            return View();
        }

        // POST: Results/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResultId,EventId,HorseId,Class,Score,Notes")] Result result) {
            if (ModelState.IsValid) {
                _context.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Description", result.EventId);
            ViewData["HorseId"] = new SelectList(_context.Horse, "HorseId", "Name", result.HorseId);
            return View(result);
        }

        // GET: Results/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var result = await _context.Result.SingleOrDefaultAsync(m => m.ResultId == id);
            if (result == null) {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Description", result.EventId);
            ViewData["HorseId"] = new SelectList(_context.Horse, "HorseId", "Name", result.HorseId);
            return View(result);
        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResultId,EventId,HorseId,Class,Score,Notes")] Result result) {
            if (id != result.ResultId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(result);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!ResultExists(result.ResultId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Description", result.EventId);
            ViewData["HorseId"] = new SelectList(_context.Horse, "HorseId", "Name", result.HorseId);
            return View(result);
        }

        // GET: Results/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var result = await _context.Result
                .Include(r => r.Event)
                .Include(r => r.Horse)
                .SingleOrDefaultAsync(m => m.ResultId == id);
            if (result == null) {
                return NotFound();
            }

            return View(result);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var result = await _context.Result.SingleOrDefaultAsync(m => m.ResultId == id);
            _context.Result.Remove(result);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResultExists(int id) {
            return _context.Result.Any(e => e.ResultId == id);
        }
    }
}
