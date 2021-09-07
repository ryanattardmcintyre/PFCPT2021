using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;
using ProgrammingForTheCloudPT2021.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammingForTheCloudPT2021.DataAccess.Repositories
{
    public class CacheAccess : ICacheAccess
    {
        private IDatabase db;
        public CacheAccess(IConfiguration config)
        {
            string connectionString = config.GetSection("cacheConnection").Value;
            db = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
        }

        public string FetchData(string key)
        {
            return db.StringGet(key);
        }

        public void SaveData(ItemDictionary item)
        {
            db.StringSet(item.Id, item.Value);
        }
    }
}
