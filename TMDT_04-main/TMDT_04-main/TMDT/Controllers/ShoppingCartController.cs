using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        // GET: ShoppingCart
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        [Authorize(Roles = "User")]
        //public ActionResult AddtoCart(int id)
        //{
        //    var pro = _db.Book.SingleOrDefault(s => s.BookID == id);
        //    if (pro != null)
        //    {
        //        GetCart().Add(pro);
        //    }
        //    return Redirect(Request.UrlReferrer.ToString());

        //}

        public ActionResult AddtoCart(int id, int quantity)
        {
            var pro = _db.Book.SingleOrDefault(s => s.BookID == id);
            if (pro != null)
            {
                GetCart().Add(pro, quantity);
            }
            return Redirect(Request.UrlReferrer.ToString());

        }
        public ActionResult AddtoCart_Detail(int id, int quantity)
        {
            var pro = _db.Book.SingleOrDefault(s => s.BookID == id);
            if (pro != null)
            {
                GetCart().Add(pro, quantity);
            }
            return RedirectToAction("ShowToCart", "ShoppingCart");

        }


        public ActionResult ShowToCart()
        {
            if (Session["Cart"] == null)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return View();
            }
            Cart cart = Session["Cart"] as Cart;
            if (cart.Items.Count() == 0)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return View();
            }
            return View(cart);
        }

        public ActionResult Update_Quantity_Cart(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = Convert.ToInt32(form["ID_Product"]);
            int _quantity = int.Parse(form["Quantity"]);
            cart.Update_Quantity_Shopping(id_pro, _quantity);
            return RedirectToAction("ShowToCart", "ShoppingCart");
        }
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_Cart_Item(id);
            return Redirect(Request.UrlReferrer.ToString());
        }
        public PartialViewResult BagCart()
        {
            int total_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
                total_item = cart.Total_Quantity_In_Cart();
            ViewBag.QuantityCart = total_item;

            return PartialView("BagCart");

        }
        public PartialViewResult BagCartItem()
        {
            if (Session["Cart"] == null)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return PartialView();
            }
            Cart cart = Session["Cart"] as Cart;
            if (cart.Items.Count() == 0)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return PartialView();
            }
            return PartialView(cart);

        }
        public ActionResult ThanhToan()
        {
            if (Session["Cart"] == null)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return View();
            }
            Cart cart = Session["Cart"] as Cart;
            string currentUserId = User.Identity.GetUserId();
            var currentUser = _db.Users.FirstOrDefault(x => x.Id == currentUserId);
            ViewBag.CurrentPhone = currentUser.PhoneNumber;
            ViewBag.CurrentAddress = currentUser.Address;

            if (cart.Items.Count() == 0)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return View();
            }

            return View(cart);
        }


    }
}