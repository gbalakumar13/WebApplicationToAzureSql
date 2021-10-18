using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetAppSqlDb.Models;
using System.Diagnostics;
using System.IO;

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
        public ActionResult Upload(HttpPostedFileBase uploadedImage)
        {
            try
            {
                if (uploadedImage.ContentLength > 0)
                {
                    string imageFileName = Path.GetFileName(uploadedImage.FileName);
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Some Error was thrown while uploading file to blob storage";
            }

            return View();
        }
    }
}
