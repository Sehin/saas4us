using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebRole1.Controllers
{
    public class StorageController : Controller
    {
        //
        // GET: /Storage/
        MyBlobStorageService myBlobStorageService = new MyBlobStorageService();
        public ActionResult Index()
        {
            return View();
        }

        public String PostFileInStorage(HttpPostedFileBase fileBase, String containerName)
        {
            try
            {
                if (fileBase.ContentLength > 0)
                {

                    CloudBlobContainer container = myBlobStorageService.getCloudBlobContainer(containerName);
                    CloudBlockBlob blob = container.GetBlockBlobReference(fileBase.FileName);
                    blob.UploadFromStream(fileBase.InputStream);
                    return blob.Uri.ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                return "Error";
            }
        }
        public class MyBlobStorageService
        {
            public CloudBlobContainer getCloudBlobContainer(String containerName)
            {
                String connectionString = ConfigurationManager.ConnectionStrings["AzureStorageAccount"].ConnectionString;
                CloudStorageAccount sa = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient bc = sa.CreateCloudBlobClient();
                CloudBlobContainer container = bc.GetContainerReference(containerName);
                return container;
            }
        }
    }
}
