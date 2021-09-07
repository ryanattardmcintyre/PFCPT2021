using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgrammingForTheCloudPT2021.Data;
using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;
using ProgrammingForTheCloudPT2021.Models;

namespace ProgrammingForTheCloudPT2021.Controllers
{
    public class CacheController : Controller
    {
        private ApplicationDbContext _context;
        private ICacheAccess _cacheAccess;
        public CacheController(ApplicationDbContext context, ICacheAccess cacheAccess)
        {
            _context = context;
            _cacheAccess = cacheAccess;
        }

        public IActionResult Save()
        {
            //Test to check which is the faster: the cache or the relational database

            //1. preparing data in the relational database

            List<Item> items = new List<Item>();
            Item item = new Item();
            for (int i = 0; i < 2000; i++)
            {
                item.Id = Guid.NewGuid().ToString();
                item.Name = "Item " + i;
                item.Price = i;
                item.Url = "https://cdn.britannica.com/77/170477-050-1C747EE3/Laptop-computer.jpg";

                items.Add(item);
            }

            DateTime startForRelationalDb = DateTime.Now;
            _context.ItemsDictionary.Add(new ItemDictionary()
            {
                Id = Guid.NewGuid().ToString(),
                Value = JsonConvert.SerializeObject(items)

            });
            _context.SaveChanges();
            double secondsForRelationalDb = DateTime.Now.Subtract(startForRelationalDb).TotalSeconds;

            //2. preparing data in the cache database
            DateTime startForCache = DateTime.Now;
            _cacheAccess.SaveData(new ItemDictionary()
            {
                Id = Guid.NewGuid().ToString(),
                Value = JsonConvert.SerializeObject(items)

            });
            double secondsForCacheDb = DateTime.Now.Subtract(startForCache).TotalSeconds;

            return Content(
                $"Seconds for Relational db: {secondsForRelationalDb}<br/>Seconds for Cache db: {secondsForCacheDb}<br/>"
                );
        }


        public IActionResult Fetch()
        {

            //1. retrieve data from cache

            DateTime startForCache = DateTime.Now;
            List<Item> items1 = JsonConvert.DeserializeObject<List<Item>>(_cacheAccess.FetchData("1849503d-afe5-475b-879c-c43b0f391a3f"));
            double secondsForCacheDb = DateTime.Now.Subtract(startForCache).TotalSeconds;

            //2. retrieve data from db

            DateTime startForRelationalDb = DateTime.Now;
            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(_context.ItemsDictionary.First().Value);
            double secondsForRelationalDb = DateTime.Now.Subtract(startForRelationalDb).TotalSeconds;


            //3. compare the time taken for both operations

            return Content(
               $"Seconds for Relational db: {secondsForRelationalDb}<br/>Seconds for Cache db: {secondsForCacheDb}<br/>"
               );

        }
    }
}
