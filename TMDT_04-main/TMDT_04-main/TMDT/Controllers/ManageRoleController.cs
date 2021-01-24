using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using TMDT.Models.ViewModel;

namespace TMDT.Controllers
{
  
    [AccessDeniedAuthorize(Roles = "ADMIN")]
    public class ManageRoleController : Controller
    {
        private static ApplicationDbContext context = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        // GET: ManageRole

        public ActionResult Index()
        {
            var usersWithRoles = (from user in context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new UserInRoleViewModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });
            ViewBag.Roles = context.Roles.AsEnumerable();

            return View(usersWithRoles);


        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {

            string roleName = formCollection["roleName"];
            string userId = formCollection["userId"];

            string currentRole = UserManager.GetRoles(userId).First();

            var roles = context.Roles.AsEnumerable();
            if (currentRole != null)
            {
                UserManager.RemoveFromRole(userId, currentRole);
            }
            // userManager.RemoveFromRole(userId, roleName);
            UserManager.AddToRole(userId, roleName);
            return RedirectToAction(nameof(Index));
        }


        private bool HasPassword(string id)
        {
            var user = UserManager.FindById(id);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            
            var u = UserManager.FindById(id);
            var model = new UserVM
            {
                Id = id,
                Email = u.Email,
                Password = u.PasswordHash,
                Phone = u.PhoneNumber,
                Address = u.Address,
                Sex = u.Sex
            };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> EditUser(string id ,string phone, string address, string sex)
        {
            var user = UserManager.FindById(id);
            user.PhoneNumber = phone;
            user.Address = address;
            user.Sex = sex;

             await UserManager.UpdateAsync(user);

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpGet]
        
        public ActionResult ChangePassword(string id)
        {
          
            var model = new UserVM
            {
                Id = id,
             
              
           
            };
            return View(model);
        }
        // POST: /Manage/ChangePassword
        [HttpPost]
       
        public async Task<ActionResult> ChangePassword(string id, string newPassword)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = UserManager.FindById(id);
            await UserManager.RemovePasswordAsync(id);
            await UserManager.AddPasswordAsync(id, newPassword);
            return RedirectToAction(nameof(Index));
        }


    }
}