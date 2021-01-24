using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class PriceController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Price
        public ActionResult Index()
        {
            List<Price> ObjList = new List<Price>()
            {
                new Price { ID = 1, price = 33 },
                new Price { ID = 2, price = 66 },
                new Price { ID = 3, price = 99 },
                new Price { ID = 4, price = 388 }
            };
            ViewBag.o = ObjList;
            return View(ObjList);
        }

    }
}
