using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using DogGo.Models;
using DogGo.Repositories;
using DogGo.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class DogsController : Controller
    {
        private readonly IDogRepository _dogRepo;
        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public DogsController(IDogRepository dogRepository)
        {
            _dogRepo = dogRepository;
        }


        // GET: DogController
        [Authorize]
        public ActionResult Index()
        {
            int ownerId = GetCurrentUserId();

            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);

            return View(dogs);
        }


        // GET: DogController/Details/5
        public ActionResult Details(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);
            return View(dog);
        }

        // GET: DogController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog dog)
        {
            try
            {
                // update the dogs OwnerId to the current user's Id
                dog.OwnerId = GetCurrentUserId();

                _dogRepo.AddDog(dog);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }


        // GET: DogController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {

            Dog dog = _dogRepo.GetDogById(id);

            if (dog == null || dog.OwnerId != GetCurrentUserId())
            {
                return NotFound();
            }

            return View(dog);
        }

        // POST: DogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog dog)
        {
            try
            {
                _dogRepo.UpdateDog(dog);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(dog);
    }
}

// GET: DogController/Delete/5
[Authorize]
public ActionResult Delete(int id)
{
    Dog dog = _dogRepo.GetDogById(id);

    if (dog == null || dog.OwnerId != GetCurrentUserId())
    {
        return StatusCode(404);
    }

    return View(dog);
}

// POST: DogController/Delete/5
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Delete(int id, Dog dog)
{
    try
    {
        _dogRepo.DeleteDog(id);
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        return View(dog);
    }
}

//GET: VALIDATE USER AFTER LOGIN TO SEE DOGS
private int GetCurrentUserId()
{
    string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
    return int.Parse(id);
}

    }
}
