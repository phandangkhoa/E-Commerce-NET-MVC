using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TMDT.Models;
namespace TMDT.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        public ActionResult Index(int? page)
        {

            var book = db.Book.Include(b => b.Author).Include(b => b.Category).Include(b => b.Provider).Include(b => b.Publisher);
            //san pham moi nhap 
            var latestbooks = db.Book.Take(4).OrderByDescending(x => x.PublisherDate).ToList(); //lấy 10 sp mới dc them cai lệnh đó là tăng hay giảm gì á quên oi :))) lệnh take la set bao nhiu sp hiển thị
            ViewData["latestbooks"] = latestbooks;
            return View(book.ToList().ToPagedList(page ?? 1, 10));
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            var products = db.Book.Where(x => x.AuthorID == book.AuthorID && x.BookID != id && x.InStock > 0).ToList().ToPagedList(pageNumber, pageSize); //cùng mã tác giả
            //Đếm view và lưu vào csdl
            //var viewCount = db.Book.FirstOrDefault(x => x.BookID == id);
            //if (viewCount.View == null)
            //{
            //    viewCount.View = 0;
            //    viewCount.View += 1;
            //}
            //else
            //{
            //    viewCount.View += 1;
            //}
            //db.Entry(viewCount).State = System.Data.Entity.EntityState.Modified;
            //db.SaveChanges();

            ViewData["books"] = products;
            var review = db.Comments.Where(x => x.BookID == id).ToList();
            var countReview = db.Comments.Where(x => x.BookID == id).Count();
            ViewBag.countReview = countReview;
            ViewBag.review = review;
            return View(book);
        }
        [AccessDeniedAuthorize(Roles ="NV_KHO")]
        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.AuthorID = new SelectList(db.Author, "AuthorID", "AuthorName");
            ViewBag.CateID = new SelectList(db.Category, "CateID", "CateName");
            ViewBag.ProviderID = new SelectList(db.Provider, "ProviderID", "ProviderName");
            ViewBag.PublisherID = new SelectList(db.Publisher, "PublisherID", "PublisherName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                if (book.ImageUpLoad != null)
                {
                    string fileNameImg = Path.GetFileNameWithoutExtension(book.ImageUpLoad.FileName);
                    string extension = Path.GetExtension(book.ImageUpLoad.FileName);
                    fileNameImg = fileNameImg + extension;
                    book.Image = "~/Content/Images/" + fileNameImg;
                    book.ImageUpLoad.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileNameImg));
                }
                db.Book.Add(book);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AuthorID = new SelectList(db.Author, "AuthorID", "AuthorName", book.AuthorID);
            ViewBag.CateID = new SelectList(db.Category, "CateID", "CateName", book.CateID);
            ViewBag.ProviderID = new SelectList(db.Provider, "ProviderID", "ProviderName", book.ProviderID);
            ViewBag.PublisherID = new SelectList(db.Publisher, "PublisherID", "PublisherName", book.PublisherID);
            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorID = new SelectList(db.Author, "AuthorID", "AuthorName", book.AuthorID);
            ViewBag.CateID = new SelectList(db.Category, "CateID", "CateName", book.CateID);
            ViewBag.ProviderID = new SelectList(db.Provider, "ProviderID", "ProviderName", book.ProviderID);
            ViewBag.PublisherID = new SelectList(db.Publisher, "PublisherID", "PublisherName", book.PublisherID);
            return View(book);
        }
        [AccessDeniedAuthorize(Roles = "NV_KHO")]
        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookID,BookName,BookPrice,BookDescription,PublisherDate,Image,AuthorID,PublisherID,ProviderID,CateID,InStock")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorID = new SelectList(db.Author, "AuthorID", "AuthorName", book.AuthorID);
            ViewBag.CateID = new SelectList(db.Category, "CateID", "CateName", book.CateID);
            ViewBag.ProviderID = new SelectList(db.Provider, "ProviderID", "ProviderName", book.ProviderID);
            ViewBag.PublisherID = new SelectList(db.Publisher, "PublisherID", "PublisherName", book.PublisherID);
            return View(book);
        }
        [AccessDeniedAuthorize(Roles = "NV_KHO")]
        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }
 [AccessDeniedAuthorize(Roles = "NV_KHO")]
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Book.Find(id);
            db.Book.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SearchTenSach(string tenSach, int? page)
        {
            var tenSachs = from m in db.Book select m;

            if (!String.IsNullOrEmpty(tenSach))
            {
                tenSachs = tenSachs.Where(s => s.BookName.Contains(tenSach)).OrderBy(x => x.BookName);
            }
            else
            {
                return RedirectToAction("Index", "Books");
            }

            return View("Index", tenSachs.ToList().ToPagedList(page ?? 1, 10));

        }
        public ActionResult LoadSachTheoDanhMuc(string name, int? page)
        {
            var ten_loai = name;
            var tenDanhMuc = db.Book.Where(m => m.Category.CateName == ten_loai).OrderBy(x => x.BookName);
            return View("Index", tenDanhMuc.ToList().ToPagedList(page ?? 1, 10));

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Review([Bind(Include = "ID, Comment, UserID, BookID")] Comment review, int proid, string Comment)
        {
            //Đếm sản phẩm trong giỏ hàng           
            var user = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            review.Name = user.UserName;
            review.Avatar = user.Image.ToString().Replace("~", "");
            review.UserId = user.Id;
            review.BookID = proid;
            review.Time = DateTime.Now;
            review.Content = Comment;
            db.Comments.Add(review);
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

    }
}
