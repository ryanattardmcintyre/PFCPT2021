using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProgrammingForTheCloudPT2021.Data;
using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;
using ProgrammingForTheCloudPT2021.Models;

namespace ProgrammingForTheCloudPT2021.Controllers
{
    public class ItemsController : Controller
    {
        private ApplicationDbContext _context;
        private IFirestoreAccess _fireStoreAccess;
        public ItemsController(ApplicationDbContext context, IFirestoreAccess fireStoreAccess)
        {
            _fireStoreAccess = fireStoreAccess;
            _context = context;
        }

        public async Task<IActionResult> Search(string keyword)
        {
            var list = await _fireStoreAccess.GetItems(keyword);

            return View("Index", list);
        }

        public async Task<IActionResult> Index()
        {
            //var list = _context.Items.ToList();
            //return View(list);
            var list = await _fireStoreAccess.GetItems();

            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Item i)
        {
            //i.Id = Guid.NewGuid().ToString();
            //_context.Items.Add(i);
            //_context.SaveChanges();

           Task t =  _fireStoreAccess.AddItem(i);
           t.Wait();


            return RedirectToAction("Index");
        }


        public IActionResult Delete(string id)
        {
            _fireStoreAccess.DeleteItem(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string id)
        {
            var item = await _fireStoreAccess.GetItem(id);
            return View(item);
        }

        public async Task<IActionResult> Update(string id)
        {
            var item = await _fireStoreAccess.GetItem(id);
            return View(item);
        }

        [HttpPost]
        public IActionResult Update(Item i)
        {
             _fireStoreAccess.UpdateItem(i);

            return RedirectToAction("Index");
        }

    }
}
