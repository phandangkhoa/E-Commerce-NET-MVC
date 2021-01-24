using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class ShowImgCurrentUserController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ShowImgCurrentUser
        public PartialViewResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            if (currentUser !=null&&currentUser.Image!= null && currentUser.Image.StartsWith("~"))
            {
                currentUser.Image = currentUser.Image.Replace("~", "");
                ViewBag.Img = currentUser.Image;
            }
            else
            {
                ViewBag.Img = "/Content/Images/thumbnail.png";
            }
          
           
            return PartialView();
        }
    }
}