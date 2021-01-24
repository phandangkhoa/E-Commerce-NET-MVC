using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMDT.Controllers
{
    public class HttpErrorsController : Controller
    {
        // GET: HttpErrors
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;

            return View();
        }
    }
}