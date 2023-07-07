using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTEL.Controllers
{
    public class MenusController : Controller
    {
        // GET: Menus
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}