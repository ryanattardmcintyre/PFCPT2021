using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammingForTheCloudPT2021.Models
{
    [FirestoreData]
    public class Item
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Url { get; set; }
        [FirestoreProperty]
        public double Price { get; set; }
    }
}
