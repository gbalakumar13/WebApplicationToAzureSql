using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetAppSqlDb.Models;
using System.Diagnostics;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Configuration;
using Azure.Storage.Queues;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace DotNetAppSqlDb.Controllers
{
    public class TodosController : Controller
    {
        private MyDatabaseContext db = new MyDatabaseContext();

        // GET: Todos
        public ActionResult Index()
        {            
            Trace.WriteLine("GET /Todos/Index");
            return View(db.Todoes.ToList());
        }

        // GET: Todos/Details/5
        public ActionResult Details(int? id)
        {
            Trace.WriteLine("GET /Todos/Details/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            return View(todo);
        }

        // GET: Todos/Create
        public ActionResult Create()
        {
            Trace.WriteLine("GET /Todos/Create");
            return View(new Todo { CreatedDate = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Description,CreatedDate,Name,ImagePath,ThumbnailPath,ImageFile")] Todo todo)
        {
            try
            {
                Trace.WriteLine("POST /Todos/Create");
                if (ModelState.IsValid)
                {
                    //var temp = Request.Files.Count;
                    //HttpPostedFileBase file = Request.Files["ImageData"];
                    //int i = UploadImageInDataBase(file, todo);
                    if (Request.Files.AllKeys.Any())
                    {
                        // Get the uploaded image from the Files collection
                        var httpPostedFile = HttpContext.Request.Files[0];

                        if (httpPostedFile != null)
                        {
                        }
                    }
                    db.Todoes.Add(todo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(todo);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Some exception has occurred, Sorry! : " + ex.Message;
                return null;
            }
        }

        private int UploadImageInDataBase(HttpPostedFileBase file, Todo todo)
        {
            todo.ImagePath = null; // ConvertToBytes(file);
            //var Content = new Content
            //{
            //    Title = contentViewModel.Title,
            //    Description = contentViewModel.Description,
            //    Contents = contentViewModel.Contents,
            //    Image = contentViewModel.Image
            //};
            db.Todoes.Add(todo);
            int i = db.SaveChanges();
            return (i == 1) ? 1 : 0;            
        }
        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
        // GET: Todos/Edit/5
        public ActionResult Edit(int? id)
        {
            Trace.WriteLine("GET /Todos/Edit/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            return View(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Name,Description,CreatedDate")] Todo todo)
        {
            Trace.WriteLine("POST /Todos/Edit/" + todo.ID);
            if (ModelState.IsValid)
            {
                db.Entry(todo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(todo);
        }

        // GET: Todos/Delete/5
        public ActionResult Delete(int? id)
        {
            Trace.WriteLine("GET /Todos/Delete/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            return View(todo);
        }

        // POST: Todos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trace.WriteLine("POST /Todos/Delete/" + id);
            Todo todo = db.Todoes.Find(id);
            db.Todoes.Remove(todo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Upload(int id)
        {
            Trace.WriteLine("POST /Todos/Upload/" + id);
            Todo todo = db.Todoes.Find(id);            
            return View(todo);
        }
        [HttpPost, ActionName("Upload")]
        public ActionResult Upload([Bind(Include = "ID")] Todo todo, HttpPostedFileBase uploadedImage)
        {
            try
            {
                if (uploadedImage.ContentLength > 0)
                {
                    string imageFileName = Path.GetFileName(uploadedImage.FileName);
                    string folderPath = Path.Combine(Server.MapPath("~/UploadedImages"), imageFileName);
                    uploadedImage.SaveAs(folderPath);
                    string connectionString = ConfigurationManager.AppSettings["storageAccountConnectionString"];

                    BlobServiceClient blobSvcClient = new BlobServiceClient(connectionString);
                    BlobContainerClient container = CreateContainer(blobSvcClient, "products-container", true);
                    BlobClient uploadedBlob = UploadBlob(container, folderPath);
                    BlobContainerProperties props = container.GetProperties();
                    string allMetadata = string.Empty;
                    foreach (var item in props.Metadata)
                    {
                        allMetadata = allMetadata + item.Key + " - " + item.Value + "\n";
                    }
                    string finalMetadata = allMetadata;
                    //ListBlobs(container);
                    //ListBlobs(container);
                    //ListBlobsAsAnonimousUser("con2");
                    //LeaseDemo(con1);
                    //CreateServiceSASforBlobContainer(con1);
                    //BlobClient blobClient = con1.GetBlobClient("shoes-2.png");
                    //CreateServiceSASforBlob(blobClient);
                    //string policyName = "PolicyOne";
                    //CreateStoredAccessPolicy(con1, policyName);
                    //CreateServiceSASforBlob(blobClient, policyName);
                    //}

                    if (ModelState.IsValid)
                    {
                        Todo fromDb = db.Todoes.Find(todo.ID);
                        fromDb.ImagePath = uploadedBlob.Uri.AbsoluteUri;
                        db.Entry(fromDb).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    PostMessageInQueue(connectionString, todo.ID, uploadedBlob.Uri.AbsoluteUri);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                ViewBag.Message = "Some Error was thrown while uploading file to blob storage" + exception.Message;
            }

            return View(todo);
        }

        private static void PostMessageInQueue(string connectionString, int id, string uri)
        {
            CloudQueueClient cloudQueueClient = CloudStorageAccount.Parse(connectionString).CreateCloudQueueClient();
            CloudQueue thumbnailRequestQueue = cloudQueueClient.GetQueueReference("productsqueue");
            thumbnailRequestQueue.CreateIfNotExists();
            BlobInformation blobInfo = new BlobInformation() { Id = id, BlobUri = new Uri(uri) };
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(blobInfo));
            thumbnailRequestQueue.AddMessage(queueMessage);
        }

        private BlobContainerClient CreateContainer(BlobServiceClient blobSvcClient, string containerName, bool isPublic)
        {
            BlobContainerClient container = blobSvcClient.GetBlobContainerClient(containerName);
            if (!container.Exists())
            {
                container.Create();
                Console.WriteLine($"{containerName} is Created\n");
                if (isPublic)
                {
                    container.SetAccessPolicy(PublicAccessType.Blob);
                }
            }
            return container;
        }

        private BlobClient UploadBlob(BlobContainerClient container, string path)
        {
            FileInfo fi = new FileInfo(path);
            BlobClient blobClient = container.GetBlobClient(fi.Name);
            if (!blobClient.Exists())
            {
                Azure.Response<BlobContentInfo> temp = blobClient.Upload(path);
            }
            return blobClient;
        }


        public QueueClient CreateQueue(string storageConString , string queueName)
        {
            try
            {
                string connectionString = storageConString;
                QueueClient queueClient = new QueueClient(connectionString, queueName);
                queueClient.CreateIfNotExists();
                return queueClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n\n");
                Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
                return null;
            }
        }
    }
}
