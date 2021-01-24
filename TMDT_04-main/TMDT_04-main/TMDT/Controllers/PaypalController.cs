
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class PaypalController : Controller
    {
        // GET: PayMent

        public ActionResult Index()
        {
            return View();
        }


        #region thanh toan bang tk paypal
        public ActionResult PaymentWithPaypal()
        {
            //getting the apiContext as earlier
            APIContext apiContext = PaypalConfigruation.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist

                    //it is returned by the create function call of the payment class

                    // Creating a payment

                    // baseURL is the url on which paypal sendsback the data.

                    // So we have provided URL of this controller only

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/PaymentWithPayPal?";

                    //guid we are generating for storing the paymentID received in session

                    //after calling the create function and it is used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url

                    //on which payer is redirected for paypal acccount payment

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("FailureView");
            }

            return View("SuccessView");
        }
        #endregion

        #region code dựng
        private PayPal.Api.Payment payment;

        private PayPal.Api.Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new PayPal.Api.Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private PayPal.Api.Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            float subTotal = 0;
            float toTal = 0;
            var itemList = new ItemList() { items = new List<Item>() };
            if (Session["Cart"] != null)
            {


                Cart cart = Session["Cart"] as Cart;
                foreach (var i in cart.Items)
                {
                    Item a = new Item();
                    a.name = i._shopping_product.BookName.ToString();
                    a.price = i._shopping_product.BookPrice.ToString();
                    a.quantity = i._shopping_quantity.ToString();
                    a.sku = "sku";
                    a.currency = "USD";
                    itemList.items.Add(a);
                    subTotal += i._shopping_quantity * i._shopping_product.BookPrice;
                }


                var payer = new Payer() { payment_method = "paypal" };

                // Configure Redirect Urls here with RedirectUrls object
                var redirUrls = new RedirectUrls()
                {
                    cancel_url = redirectUrl,
                    return_url = redirectUrl
                };

                // similar as we did for credit card, do here and create details object
                var details = new Details()
                {
                    tax = "1",
                    shipping = "1",
                    subtotal = subTotal.ToString()
                };
                // toTal = (subTotal / 23135) + 2;
                toTal = subTotal + 2;
                // similar as we did for credit card, do here and create amount object
                var amount = new Amount()
                {
                    currency = "USD",
                    total = toTal.ToString(), // Total must be equal to sum of shipping, tax and subtotal.
                    details = details
                };

                var transactionList = new List<Transaction>();

                transactionList.Add(new Transaction()
                {
                    description = "Vận chuyển về Việt Nam",
                    invoice_number = Guid.NewGuid().ToString(),
                    amount = amount,
                    item_list = itemList
                });

                this.payment = new PayPal.Api.Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls
                };
            }
            // Create a payment using a APIContext
            return this.payment.Create(apiContext);

        }
        #endregion
    }
}