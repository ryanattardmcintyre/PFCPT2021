using ProgrammingForTheCloudPT2021.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammingForTheCloudPT2021.DataAccess.Interfaces
{
    public interface ICacheAccess
    {
        void SaveData(ItemDictionary item);
        string FetchData(string key);
    }
}
