using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;

namespace ProgrammingForTheCloudPT2021.Controllers
{
    public class FileController : Controller
    { const string bucketName = "pfcpt2021bucket";
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        /*
         * uniform vs finegrained (you can assign or remove individual objects permissions)
         * 2 set of permissions
         *  - permissions which can be given to the whole bucket (e.g. allUsers -> Object Readers)
         *  - permissions which you can apply on the individual object only if it is finegrained
         * 
         * */


        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
           //uploads object
            var storage = StorageClient.Create();

            string newFilename = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);

            using (var fileStream = file.OpenReadStream())
            {
                storage.UploadObject(bucketName, newFilename, file.ContentType, fileStream);
            }

          //make the object public to be READ
            var storageObject = storage.GetObject(bucketName, newFilename);
            storageObject.Acl ??= new List<ObjectAccessControl>();

            var returnedObject = storage.UpdateObject(storageObject, new UpdateObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead });

            TempData["message"] = $"File uploaded. click <a href=\"{returnedObject.MediaLink}\">here</a> to download image";
            return View();
        }


        public IActionResult Index()
        {
            List<string> urlsToImages = new List<string>();
            var storage = StorageClient.Create();

            var storageObjects = storage.ListObjects(bucketName);
            foreach (var storageObject in storageObjects)
            {
           
                urlsToImages.Add(
                    $"https://storage.googleapis.com/{bucketName}/{storageObject.Name}"
                    );
            }

            return View( urlsToImages);
        }
    }
}
