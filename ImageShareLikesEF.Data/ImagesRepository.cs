using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ImageShareLikesEF.Data
{
    public class ImagesRepository
    {
        private string _connectionString;

        public ImagesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Image> GetImages()
        {
            using var context = new ImageLikesContext(_connectionString);
            return context.Images.OrderByDescending(i => i.DateUploaded).ToList();
        }

        public void Add(Image image)
        {
            using var context = new ImageLikesContext(_connectionString);
            context.Images.Add(image);
            context.SaveChanges();
        }

        public Image GetImage(int id)
        {
            using var context = new ImageLikesContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }

        public void AddLike(int id)
        {
            using var context = new ImageLikesContext(_connectionString);
            context.Database.ExecuteSqlInterpolated($"UPDATE Images SET Likes = Likes + 1 WHERE Id = {id}");
        }

        public int GetLikes(int id)
        {
            using var context = new ImageLikesContext(_connectionString);
            return context.Images.Where(i => i.Id == id).Select(i => i.Likes).FirstOrDefault();
        }
    }

}