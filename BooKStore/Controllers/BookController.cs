using BooKStore.Models.Domain;
using BooKStore.Models.DTO;
using BooKStore.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BooKStore.Controllers
{
    public class BookController : Controller
    {
        private DataBaseContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IBookService bookService;
        private readonly IAuthorService authorService;
        private readonly IGenreService genreService;
        private readonly IPublisherService publisherService;
        public BookController(DataBaseContext _db,IBookService bookService, IGenreService genreService, IPublisherService publisherService, IAuthorService authorService,IWebHostEnvironment _webHostEnvironment)
        {
            this.bookService = bookService;
            this.genreService = genreService;
            this.publisherService = publisherService;
            this.authorService = authorService;
            this._webHostEnvironment = _webHostEnvironment;
            this._db = _db;
        }
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> Add()
        {
            var model = new Book();
            model.AuthorList = authorService.GetAll().Select(a => new SelectListItem { Text = a.AuthorName, Value = a.Id.ToString() }).ToList();
            model.PublisherList = publisherService.GetAll().Select(a => new SelectListItem { Text = a.PublisherName, Value = a.Id.ToString() }).ToList();
            model.GenreList = genreService.GetAll().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Book model)
        {
            model.AuthorList = authorService.GetAll().Select(a => new SelectListItem { Text = a.AuthorName, Value = a.Id.ToString(), Selected = a.Id == model.AuthorId }).ToList();
            model.PublisherList = publisherService.GetAll().Select(a => new SelectListItem { Text = a.PublisherName, Value = a.Id.ToString(), Selected = a.Id == model.PubhlisherId }).ToList();
            model.GenreList = genreService.GetAll().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = a.Id == model.GenreId }).ToList();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ImageUpload != null)
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/books");
                string imageName = Guid.NewGuid().ToString() + "_" + model.ImageUpload.FileName;

                string filePath = Path.Combine(uploadsDir, imageName);

                FileStream fs = new FileStream(filePath, FileMode.Create);
                 model.ImageUpload.CopyTo(fs);
                fs.Close();

                model.Image = imageName;
            }
            var result = bookService.Add(model);
            if (result)
            {
                TempData["msg"] = "Added Successfully";
                return RedirectToAction("GetAll");
            }
            TempData["Error"] = "Error has occured on server side";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id)
        {
            var model = bookService.FindById(id);
            model.AuthorList = authorService.GetAll().Select(a => new SelectListItem { Text = a.AuthorName, Value = a.Id.ToString(), Selected = a.Id == model.AuthorId }).ToList();
            model.PublisherList = publisherService.GetAll().Select(a => new SelectListItem { Text = a.PublisherName, Value = a.Id.ToString(), Selected = a.Id == model.PubhlisherId }).ToList();
            model.GenreList = genreService.GetAll().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = a.Id == model.GenreId }).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Book model)
        {
            model.AuthorList = authorService.GetAll().Select(a => new SelectListItem { Text = a.AuthorName, Value = a.Id.ToString(), Selected = a.Id == model.AuthorId }).ToList();
            model.PublisherList = publisherService.GetAll().Select(a => new SelectListItem { Text = a.PublisherName, Value = a.Id.ToString(), Selected = a.Id == model.PubhlisherId }).ToList();
            model.GenreList = genreService.GetAll().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = a.Id == model.GenreId }).ToList();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ImageUpload != null)
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/books");
                string imageName = Guid.NewGuid().ToString() + "_" + model.ImageUpload.FileName;

                string filePath = Path.Combine(uploadsDir, imageName);

                FileStream fs = new FileStream(filePath, FileMode.Create);
                model.ImageUpload.CopyTo(fs);
                fs.Close();

                model.Image = imageName;
            }
            var result = bookService.Update(model);
            if (result)
            {
                TempData["msg"] = "Updated Successfully";
                return RedirectToAction("GetAll");
            }
            TempData["Error"] = "Error has occured on server side";
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            Book book = await _db.Book.FindAsync(id);

            if (!string.Equals(book.Image, "noimage.png"))
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/books");
                string oldImagePath = Path.Combine(uploadsDir, book.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            var result = bookService.Delete(id);
            TempData["msg"] = "Deleted Successfully";
            return RedirectToAction("GetAll");
        }

        //public IActionResult GetAll()
        //{
        //    var result = bookService.GetAll();
        //    return View(result);
        //}


        public async Task<IActionResult> GetAll(string search = "", string Sortcolumn = "Title", string Iconclass = "fa-sort-asc", int PageNo = 1)
        {
            ViewBag.search = search;
            List<Book> books =bookService.GetAll().Where(temp => temp.Title.Contains(search)).ToList();

            /*Sorting*/
            ViewBag.SortColumn = Sortcolumn;
            ViewBag.IconClass = Iconclass;
            if (ViewBag.SortColumn == "GenreName")
            {
                if (ViewBag.IconClass == "fa-sort-asc")
                    books = books.OrderBy(temp => temp.GenreName).ToList();
                else
                    books = books.OrderByDescending(temp => temp.GenreName).ToList();
            }
            else if (ViewBag.SortColumn == "Title")
            {
                if (ViewBag.IconClass == "fa-sort-asc")
                    books = books.OrderBy(temp => temp.Title).ToList();
                else
                    books = books.OrderByDescending(temp => temp.Title).ToList();
            }
            else if (ViewBag.SortColumn == "Isbn")
            {
                if (ViewBag.IconClass == "fa-sort-asc")
                    books = books.OrderBy(temp => temp.Isbn).ToList();
                else
                    books = books.OrderByDescending(temp => temp.Isbn).ToList();
            }
            else if (ViewBag.SortColumn == "TotalPages")
            {
                if (ViewBag.IconClass == "fa-sort-asc")
                    books = books.OrderBy(temp => temp.TotalPages).ToList();
                else
                    books = books.OrderByDescending(temp => temp.TotalPages).ToList();
            }
            else if (ViewBag.SortColumn == "AuthorName")
            {
                if (ViewBag.IconClass == "fa-sort-asc")
                    books = books.OrderBy(temp => temp.AuthorName).ToList();
                else
                    books = books.OrderByDescending(temp => temp.AuthorName).ToList();
            }
            else if (ViewBag.SortColumn == "PublisherName")
            {
                if (ViewBag.IconClass == "fa-sort-asc")
                    books = books.OrderBy(temp => temp.PublisherName).ToList();
                else
                    books = books.OrderByDescending(temp => temp.PublisherName).ToList();
            }
            else if (ViewBag.SortColumn == "Image")
            {
                if (ViewBag.IconClass == "fa-sort-asc")
                    books = books.OrderBy(temp => temp.Image).ToList();
                else
                    books = books.OrderByDescending(temp => temp.Image).ToList();
            }
            else if (ViewBag.SortColumn == "Price")
            {
                if (ViewBag.IconClass == "fa-sort-asc")
                    books = books.OrderBy(temp => temp.Price).ToList();
                else
                    books = books.OrderByDescending(temp => temp.Price).ToList();
            }

            /* Paging */
            int NoOfRecordsPerPage = 5;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(books.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            books = books.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(books);
        }
       
        public async Task<IActionResult> Books(string genre = "", int p = 1)
        {
            int pageSize = 3;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.genre = genre;

            if (genre == "")
            {
                ViewBag.TotalPages = (int)Math.Ceiling((decimal)_db.Book.Count() / pageSize);
                return View( _db.Book.OrderByDescending(p => p.Id).Skip((p - 1) * pageSize).Take(pageSize).ToList());
            }
            Genre genres = await _db.Genre.Where(c => c.Name == genre).FirstOrDefaultAsync();
            if (genres == null) return RedirectToAction("Books");

            var booksBygenre =_db.Book.Where(p => p.Id == genres.Id);
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)booksBygenre.Count() / pageSize);

            return View(booksBygenre.OrderByDescending(p => p.Id).Skip((p - 1) * pageSize).Take(pageSize).ToListAsync());
        }

    }
}
