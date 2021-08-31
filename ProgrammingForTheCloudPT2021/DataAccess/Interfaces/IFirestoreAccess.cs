using ProgrammingForTheCloudPT2021.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammingForTheCloudPT2021.DataAccess.Interfaces
{
    public interface IFirestoreAccess
    {
        Task AddItem(Item item);
      
        Task<List<Item>> GetItems();

        Task<Item> GetItem(string id);

        void DeleteItem(string id);

        void UpdateItem(Item item);

        Task<List<Item>> GetItems(string name);

    }
}
