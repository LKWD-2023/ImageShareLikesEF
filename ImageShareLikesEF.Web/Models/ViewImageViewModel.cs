using ImageShareLikesEF.Data;

namespace ImageShareLikesEF.Web.Models
{
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public bool CanLikeImage { get; set; }
    }
}