using Microsoft.Extensions.Configuration;
using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;
using ProgrammingForTheCloudPT2021.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Firestore;

namespace ProgrammingForTheCloudPT2021.DataAccess.Repositories
{
    public class FireStoreAccess : IFirestoreAccess
    {

        FirestoreDb db;
        public FireStoreAccess(IConfiguration config)
        {
            var projId = config.GetSection("ProjectId").Value;
            db = FirestoreDb.Create(projectId: projId);
        }
        public Task AddItem(Item item)
        {
            item.Id = Guid.NewGuid().ToString();
            DocumentReference docRef = db.Collection("items").Document(item.Id);
            //Dictionary<string, object> city = new Dictionary<string, object>
            //{
            //    { "Id", item.Id },
            //    { "Name",item.Name },
            //    { "Price", item.Price },
            //    { "Url", item.Url},
            //};



            return docRef.SetAsync(item);
        }

        public async void DeleteItem(string id)
        {
            DocumentReference cityRef = db.Collection("items").Document(id);
            await cityRef.DeleteAsync();
        }

        public async Task<Item> GetItem(string id)
        {
            var docRef = db.Collection("items").Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            var item = snapshot.ConvertTo<Item>();
            return item;
        }

        public async Task<List<Item>> GetItems()
        {
            List<Item> items = new List<Item>();
             
            Query allItemsQuery = db.Collection("items");
            QuerySnapshot allIemsQuerySnapshot =   await allItemsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allIemsQuerySnapshot.Documents)
            {

                //Dictionary<string, object> city = documentSnapshot.ToDictionary();
                //Item i = new Item();
                //i.Id = city["Id"].ToString();
                //i.Name = city["Name"].ToString();
                //i.Price = Convert.ToDouble(city["Price"].ToString());

                //items.Add(i);

                items.Add(documentSnapshot.ConvertTo<Item>());
            }

            return items;
        }

        public async void UpdateItem(Item item)
        {
            await db.Collection("items").Document(item.Id).SetAsync(item);
        }

         
        public async Task<List<Item>> GetItems(string name)
        {
        
            List<Item> items = new List<Item>();

            Query allItemsQuery = db.Collection("items").WhereGreaterThanOrEqualTo("Name", name); 
            QuerySnapshot allIemsQuerySnapshot = await allItemsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allIemsQuerySnapshot.Documents)
            {

                items.Add(documentSnapshot.ConvertTo<Item>());
            }

            return items;

        }


    }
}
