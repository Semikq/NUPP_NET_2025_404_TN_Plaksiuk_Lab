using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zoo.Infrastructure;
using Zoo.Infrastructure.Models;

namespace Zoo.MVC.Controllers
{
    public class ZooKeepersController : Controller
    {
        private readonly ZooContext _context;

        public ZooKeepersController(ZooContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ZooKeepers.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zooKeeperModel = await _context.ZooKeepers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zooKeeperModel == null)
            {
                return NotFound();
            }

            return View(zooKeeperModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,ExperienceYears,Shift")] ZooKeeperModel zooKeeperModel)
        {
            if (ModelState.IsValid)
            {
                zooKeeperModel.Id = Guid.NewGuid();
                _context.Add(zooKeeperModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zooKeeperModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zooKeeperModel = await _context.ZooKeepers.FindAsync(id);
            if (zooKeeperModel == null)
            {
                return NotFound();
            }
            return View(zooKeeperModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FullName,ExperienceYears,Shift")] ZooKeeperModel zooKeeperModel)
        {
            if (id != zooKeeperModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zooKeeperModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZooKeeperModelExists(zooKeeperModel.Id))
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
            return View(zooKeeperModel);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zooKeeperModel = await _context.ZooKeepers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zooKeeperModel == null)
            {
                return NotFound();
            }

            return View(zooKeeperModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var zooKeeperModel = await _context.ZooKeepers.FindAsync(id);
            if (zooKeeperModel != null)
            {
                _context.ZooKeepers.Remove(zooKeeperModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZooKeeperModelExists(Guid id)
        {
            return _context.ZooKeepers.Any(e => e.Id == id);
        }
    }
}