using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {

        public IActionResult Index()
        {
            var context = new Entities();
            
            // Add Racks and Shelfs to viewbag to use it in dropdownlist

            ViewBag.Racks = context.Rack.Select(x => new SelectListItem { Value = x.RackId.ToString(), Text = x.Code });
            ViewBag.Shelves = context.Shelf.Select(x => new SelectListItem { Value = x.ShelfId.ToString(), Text = x.Code });
            return View();
        }


        public JsonResult GetData()
        {
            try
            {
                var context = new Entities();
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //Work whith custom filtering parameters
                //Apply only active filters
                int price = -1;
                var pricestring = Request.Form["price"].FirstOrDefault();
                if(!string.IsNullOrEmpty(pricestring))
                int.TryParse(pricestring, out price);

                int rackid = -1;
                var rackstring = Request.Form["rackid"].FirstOrDefault();
                if (!string.IsNullOrEmpty(rackstring))
                    int.TryParse(rackstring, out rackid);

                int shelfid = -1;
                var shelfstring = Request.Form["shelfid"].FirstOrDefault();
                if (!string.IsNullOrEmpty(shelfstring))
                    int.TryParse(shelfstring, out shelfid);

                var text = Request.Form["text"].FirstOrDefault();

                bool isavailable = Convert.ToBoolean(Request.Form["available"].FirstOrDefault());


                var customerData = (from book in context.Book
                                    select book).Where(x=>x.IsDeleted != true).Include(x => x.Shelf).AsQueryable();



                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.Code.Contains(searchValue) || m.Name.Contains(searchValue) || m.Author.Contains(searchValue));
                }


                //Custom filtering

                if (!string.IsNullOrEmpty(text))
                {
                    customerData = customerData.Where(m => m.Name.Contains(text) || m.Author.Contains(text));
                }

                if (price != -1)
                {
                    customerData = customerData.Where(x => x.Price == price);
                }

                if (rackid != -1)
                {
                    customerData = customerData.Where(x => x.Shelf.RackId == rackid);
                }

                if (shelfid != -1)
                {
                    customerData = customerData.Where(x => x.ShelfId == shelfid);
                }

                if (isavailable ==  true)
                {
                    customerData = customerData.Where(x => x.IsAvailable == true);
                }

                recordsTotal = customerData.Count();
                
                var data = customerData.Skip(skip).Take(pageSize).ToList().Select(x => new { bookId = x.BookId, code = x.Code, name = x.Name, author = x.Author, isAvailable = x.IsAvailable, price = (decimal)x.Price, shelf = x.Shelf.Code, shelfId = x.ShelfId });

                var totalPrice = data.Sum(x => x.price);

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, totalPrice = totalPrice });

            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public ActionResult Add([FromBody]Book data)
        {
            try
            {
                var context = new Entities();

                var book = new Book();
                
                //Check if book name or code already exist

                if (context.Book.Any(x => x.Code == data.Code))
                {
                    return Json(new { Message = "Book code already exist", Status = "error", Title = "Error" });
                }

                if (context.Book.Any(x => x.Name == data.Name))
                {
                    return Json(new { Message = "Name already exist", Status = "error", Title = "Error" });
                }

                //Insert a book record using dbcontext

                //book.Code = data.Code;
                //book.Name = data.Name;
                //book.Author = data.Author;
                //book.Price = data.Price;
                //book.IsAvailable = data.IsAvailable;
                //book.IsDeleted = false;
                //book.ShelfId = data.ShelfId;

                //context.Book.Add(book);
                //int result = context.SaveChanges(); 


                //Insert a book record using stored procedure with parameters

                var code = new SqlParameter("Code", data.Code);
                var name = new SqlParameter("Name", data.Name);
                var author = new SqlParameter("Author", data.Author);
                var price = new SqlParameter("Price", data.Price);
                var isAvailable = new SqlParameter("IsAvailable", data.IsAvailable);
                var shelfId = new SqlParameter("ShelfId", data.ShelfId);

                context.Database.ExecuteSqlCommand("exec AddBook @Code,@Name,@Author,@IsAvailable,@Price,@ShelfId", code, name, author, isAvailable, price,shelfId);

                return Json(new { Message = "Book added", Status = "success", Title = "" });
              
            }

            catch (Exception ex)
            {
                return Json(new { Message = "Error happened, contact system admin", Status = "error", Title = "Error" });
            }

        }


        [HttpPost]
        public ActionResult Update([FromBody]Book data)
        {
            try
            {
                var context = new Entities();

                var book = new Book();

                //Check if book record exist

                if (context.Book.Any(x => x.BookId == data.BookId))
                {
                    book = context.Book.Find(data.BookId);

                    //Check if book name or code already exist

                    if (context.Book.Any(x => x.Code == data.Code && x.BookId != data.BookId))
                    {
                        return Json(new { Message = "Book code already exist", Status = "error", Title = "Error" });
                    }

                    if (context.Book.Any(x => x.Name == data.Name && x.BookId != data.BookId))
                    {
                        return Json(new { Message = "Name already exist", Status = "error", Title = "Error" });
                    }

                    //Insert a book record using dbcontext 

                    //book.Code = data.Code;
                    //book.Name = data.Name;
                    //book.Author = data.Author;
                    //book.Price = data.Price;
                    //book.IsAvailable = data.IsAvailable;
                    //book.ShelfId = data.ShelfId;

                    //context.Entry(book).State = EntityState.Modified;
                    //context.SaveChanges();


                    //Update book record using stored procedure with parameters

                    var bookId = new SqlParameter("BookId", data.BookId);
                    var code = new SqlParameter("Code", data.Code);
                    var name = new SqlParameter("Name", data.Name);
                    var author = new SqlParameter("Author", data.Author);
                    var price = new SqlParameter("Price", data.Price);
                    var isAvailable = new SqlParameter("IsAvailable", data.IsAvailable);
                    var shelfId = new SqlParameter("ShelfId", data.ShelfId);

                    context.Database.ExecuteSqlCommand("exec UpdateBook @BookId,@Code,@Name,@Author,@IsAvailable,@Price,@ShelfId", bookId,code, name, author, isAvailable, price, shelfId);

                    return Json(new { Message = "Book updated", Status = "success", Title = "" });
                }
                else
                {
                    return Json(new { Message = "Book data not exist", Status = "error", Title = "Error" });
                }



            }

            catch (Exception ex)
            {
                return Json(new { Message = "Error happened, contact system admin", Status = "error", Title = "Error" });
            }

        }



        [HttpPost]
        public ActionResult Delete([FromBody]Book data)
        {
            try
            {
                var context = new Entities();

                var book = new Book();

                //Check if book record exist

                if (context.Book.Any(x => x.BookId == data.BookId))
                {

                    //Delete book record (Setting isDeleted flag to true) using DBContext

                    //book = context.Book.Find(data.BookId);
                    //book.IsDeleted = true;

                    //context.Entry(book).State = EntityState.Modified;
                    //context.SaveChanges();


                    //Delete book record (Setting isDeleted flag to true) using stored procedure with parameter

                    var bookId = new SqlParameter("BookId", data.BookId);
                    
                    context.Database.ExecuteSqlCommand("exec DeleteBook @BookId", bookId);


                    return Json(new { Message = "Book deleted", Status = "success", Title = "" });
                }
                else
                {
                    return Json(new { Message = "Book data not exist", Status = "error", Title = "Error" });
                }



            }

            catch (Exception ex)
            {
                return Json(new { Message = "Error happened, contact system admin", Status = "error", Title = "Error" });
            }

        }

    }
}
