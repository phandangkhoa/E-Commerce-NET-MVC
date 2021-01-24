using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TMDT.Mail;
using TMDT.Models;
using TMDT.Payment;

namespace TMDT.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        public ActionResult Index()
        {

            return View(db.Order.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            var orderDetail = (from m in db.OrderDetail where m.OrderID == id select m).ToList();
            var userrOrderId = orderDetail.FirstOrDefault().Order.UserId.ToString();
            var userOrderDetail = db.Users.FirstOrDefault(x => x.Id == userrOrderId);

            ViewBag.name = orderDetail.FirstOrDefault().Order.NameRec;
            ViewBag.email = userOrderDetail.Email;
            ViewBag.address = orderDetail.FirstOrDefault().Order.AddressOrder;
            ViewBag.phone = orderDetail.FirstOrDefault().Order.PhoneOrder;

            return View(orderDetail);
        }

        public ActionResult DetailOfUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            var orderDetail = (from m in db.OrderDetail where m.OrderID == id select m).ToList();
            var userrOrderId = orderDetail.FirstOrDefault().Order.UserId.ToString();
            var userOrderDetail = db.Users.FirstOrDefault(x => x.Id == userrOrderId);

            ViewBag.name = orderDetail.FirstOrDefault().Order.NameRec;
            ViewBag.email = userOrderDetail.Email;
            ViewBag.address = orderDetail.FirstOrDefault().Order.AddressOrder;
            ViewBag.phone = orderDetail.FirstOrDefault().Order.PhoneOrder;

            return View(orderDetail);
        }
        public ActionResult ListOrderOfUser()
        {
            string currentUserId = User.Identity.GetUserId();
            var listOrder = (from m in db.Order where m.UserId == currentUserId select m).ToList();
            return View(listOrder);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string userName, string phoneNumber, string addressShip, int payment)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                var currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

                Order order = new Order();
                order.PhoneOrder = phoneNumber;
                order.UserId = currentUser.Id;
                order.AddressOrder = addressShip;
                order.NameRec = userName;

                switch (payment)
                {
                    case 0:
                        {
                            order.Payment = "Thanh toán khi nhận hàng";
                            break;
                        }
                    case 1:
                        {
                            order.Payment = "Thanh toán qua Momo";
                            break;
                        }
                    case 2:
                        {
                            order.Payment = "Thanh toán qua PayPal";
                            break;
                        }
                }

                order.Status = StatusEnum.WAIT;
                DateTime currentDate = DateTime.Now;
                order.OrderDate = currentDate;
                db.Order.Add(order);
                float total = 0;
                Cart cart = Session["Cart"] as Cart;
                foreach (var item in cart.Items)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderID = order.OrderID;
                    orderDetail.BookID = item._shopping_product.BookID;
                    orderDetail.UnitPriceSale = item._shopping_product.BookPrice;
                    orderDetail.Quantity = item._shopping_quantity;
                    item._shopping_product.InStock -= item._shopping_quantity;
                    total += item._shopping_quantity * item._shopping_product.BookPrice;
                    db.OrderDetail.Add(orderDetail);
                }
                db.SaveChanges();

                switch (payment)
                {
                    case 0:
                        {
                            string nameItem = "";
                            var i = 1;
                            foreach (var item in cart.Items)
                            {
                                nameItem += "<tr>" +
                                                "<td>" + i + "</td>" +
                                                "<td>" + item._shopping_product.BookName + "</td>" +
                                                "<td>" + item._shopping_quantity + "</td>" +
                                                "<td>" + item._shopping_quantity * item._shopping_product.BookPrice + "$</td>" +
                                            "</tr>";
                                i++;

                            }

                            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                            new MailHelper().SendMail(userName, "Đơn hàng mới từ Shop TMDT04", nameItem, order, total);
                            new MailHelper().SendMail(toEmail, "Đơn hàng mới từ Shop TMDT04", nameItem, order, total);
                            cart.ClearCart();
                            break;
                        }
                    case 1:
                        {
                            return RedirectToAction(nameof(ThanhToanMomo));
                        }
                    case 2:
                        {
                            return RedirectToAction("PaymentWithPaypal", "Paypal");
                        }
                }

            }
            return RedirectToAction("Index", "Books");

        }
        #region thanhtoan Momo
        public ActionResult ThanhToanMomo()
        {
            Cart cart = Session["Cart"] as Cart;
            //momo payment VND
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOVMY620201214";
            string accessKey = "YJ3tiYJsEj7DiR8h";
            string serectkey = "UVkryZe6tEZfWSrnKvQapJjgBzKROh9V";
            string orderInfo = "Đơn hàng từ SHOP TMD04";
            string returnUrl = "https://localhost:44354/Orders/ReturnUrl";
            string notifyurl = "https://localhost:44354/Orders/NotifyUrl";

            string amount = cart.Items.Sum(n => n._shopping_product.BookPrice * n._shopping_quantity).ToString();
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "payment";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            log.Debug("rawHash = " + rawHash);


            MomoPayment crypto = new MomoPayment();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);
            log.Debug("Signature = " + signature);

            //build body json request
            JObject message = new JObject
                                            {
                                                { "partnerCode", partnerCode },
                                                { "accessKey", accessKey },
                                                { "requestId", requestId },
                                                { "amount", amount },
                                                { "orderId", orderid },
                                                { "orderInfo", orderInfo },
                                                { "returnUrl", returnUrl },
                                                { "notifyUrl", notifyurl },
                                                { "extraData", extraData },
                                                { "requestType", "captureMoMoWallet" },
                                                { "signature", signature }

                                            };
            log.Debug("Json request to MoMo: " + message.ToString());
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);
            log.Debug("Return from MoMo: " + jmessage.ToString());

            return Redirect(jmessage.GetValue("payUrl").ToString());

        }

        public ActionResult ReturnUrl()
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MomoPayment crypto = new MomoPayment();
            string serectkey = "UVkryZe6tEZfWSrnKvQapJjgBzKROh9V";
            string signature = crypto.signSHA256(param, serectkey);
            if (signature != Request["signature"].ToString())
            {
                ViewBag.message = "Thông tin Request không hợp lệ";

            }

            if (Request.QueryString["errorCode"].Equals("-2") || Request.QueryString["errorCode"].Equals("0"))
            {

                ViewBag.message = "Thanh toán thành công";

            }
            else
            {
                ViewBag.message = "Thanh toán thất bại";
            }
            return View();
        }
        public JsonResult NotifyUrl()
        {
            string param = ""; // Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = "partner_code=" + Request["partner_code"] +
                "&access_key=" + Request["access_key"] +
                "&amount=" + Request["amount"] +
                "&order_id=" + Request["order_id"] +
                "&order_info=" + Request["order_info"] +
                "&order_type=" + Request["order_type"] +
                "&transaction_id=" + Request["transaction_id"] +
                "&message=" + Request["message"] +
                "&respone_time=" + Request["respone_time"] +
                "&status_code=" + Request["status_code"];
            param = Server.UrlDecode(param);
            MomoPayment crypto = new MomoPayment();

            string serectkey = "UVkryZe6tEZfWSrnKvQapJjgBzKROh9V";
            string signature = crypto.signSHA256(param, serectkey);

            //k dc cap nhat trang thai don, khi  status trong db != status WAIT
            if (signature != Request["signature"].ToString())
            {
                //cap nhat that bai vi chu ky k hop le
                // 
            }

            else
            {
                //success
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        [AccessDeniedAuthorize(Roles = "BAN_HANG, ADMIN")]
        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        [AccessDeniedAuthorize(Roles = "BAN_HANG, ADMIN")]
        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,UserId,NameRec,AddressOrder,PhoneOrder,OrderDate,Status,Payment")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }
        [AccessDeniedAuthorize(Roles = "BAN_HANG, ADMIN")]
        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        [AccessDeniedAuthorize(Roles = "BAN_HANG, ADMIN")]
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Order.Find(id);
            db.Order.Remove(order);
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
    }
}
