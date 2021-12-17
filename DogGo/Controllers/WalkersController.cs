using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using DogGo.Models;
using DogGo.Repositories;
using DogGo.Controllers;
using DogGo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalkRepository _walkRepo;
        private readonly IOwnerRepository _ownerRepo;


        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkersController(IWalkerRepository walkerRepository, IWalkRepository walkRepository, IOwnerRepository ownerRepository)
        {
            _walkerRepo = walkerRepository;
            _walkRepo = walkRepository;
            _ownerRepo = ownerRepository;
        }


        // GET: WalkersController
        public ActionResult Index()
        {
            int ownerId = GetCurrentUserId();
            
            if(ownerId != 0)
            {
                Owner owner = _ownerRepo.GetOwnerById(ownerId);
                List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);

                return View(walkers);
            }
            else
            {
                List<Walker> walkers = _walkerRepo.GetAllWalkers();
                return View(walkers);
            }
        }

        // GET: WalkersController/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walk> walks = _walkRepo.GetWalksByWalkerId(id);
            WalkerProfileViewModel vm = new WalkerProfileViewModel
            {
                Walker = walker,
                Walks = walks
            };
            if (walker == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private int GetCurrentUserId()
        {
            try
            {
                string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.Parse(id);
            }
            catch (Exception ex)
            {
                return (0);
            }
        }

    }
}