using ImageShareLikesEF.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ImageShareLikesEF.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ImageShareLikesEF.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _environment = environment;
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        public IActionResult Index()
        {
            var repo = new ImagesRepository(_connectionString);
            return View(repo.GetImages());
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(string title, IFormFile imageFile)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            string fullPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            using var stream = new FileStream(fullPath, FileMode.CreateNew);
            imageFile.CopyTo(stream);

            var image = new Image
            {
                Title = title,
                FileName = fileName,
                DateUploaded = DateTime.Now
            };
            var db = new ImagesRepository(_connectionString);
            db.Add(image);
            return RedirectToAction("Index");
        }

        public ActionResult ViewImage(int? id)
        {
            var db = new ImagesRepository(_connectionString);
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }
            var image = db.GetImage(id.Value);
            if (image == null)
            {
                return RedirectToAction("Index");
            }

            var vm = new ViewImageViewModel
            {
                Image = image
            };
            if (HttpContext.Session.GetString("likedids") != null)
            {
                var likedIds = HttpContext.Session.Get<List<int>>("likedids");
                //vm.CanLikeImage = !likedIds.Any(i => i == id);
                vm.CanLikeImage = likedIds.All(i => i != id);
            }
            else
            {
                vm.CanLikeImage = true;
            }
            return View(vm);
        }

        [HttpPost]
        public void LikeImage(int id)
        {
            var db = new ImagesRepository(_connectionString);
            db.AddLike(id);
            List<int> likedIds = HttpContext.Session.Get<List<int>>("likedids") ?? new List<int>();

            likedIds.Add(id);

            HttpContext.Session.Set("likedids", likedIds);
        }

        public ActionResult GetLikes(int id)
        {
            var db = new ImagesRepository(_connectionString);
            return Json(new { Likes = db.GetLikes(id) });
        }

    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}
