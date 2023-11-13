using BooKStore.Models.DTO;
using BooKStore.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BooKStore.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService service;
        public AuthorController(IAuthorService service)
        {
            this.service = service;
        }
        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Author model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = service.Add(model);
            if (result)
            {
                TempData["msg"] = "Added Successfully";
                return RedirectToAction("GetAll");
            }
            TempData["Error"] = "Error has occured on server side";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Update(int id)
        {
            var record = service.FindById(id);
            return View(record);
        }

        [HttpPost]
        public IActionResult Update(Author model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = service.Update(model);
            if (result)
            {
                TempData["msg"] = "Updated Successfully";
                return RedirectToAction("GetAll");
            }
            TempData["Error"] = "Error has occured on server side";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {

            var result = service.Delete(id);
            TempData["msg"] = "Deleted Successfully";
            return RedirectToAction("GetAll");
        }

        public IActionResult GetAll()
        {

            var data = service.GetAll();
            return View(data);
        }
    }
}
