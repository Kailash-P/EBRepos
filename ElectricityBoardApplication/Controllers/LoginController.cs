using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectricityBoardApplication.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult LoginPage()
        {
            return View();
        }

        public ViewResult HomePage(int loginID)
        {
            ViewBag.LoginID = loginID;
            return View();
        }

        public ViewResult QuickPayPage(int loginID)
        {
            ViewBag.LoginID = loginID;
            return View();
        }

        public ViewResult PaymentDetailPage(int loginID)
        {
            ViewBag.LoginID = loginID;
            return View();
        }

        public ViewResult EditProfilePage(int loginID)
        {
            ViewBag.LoginID = loginID;
            return View();
        }

        public ViewResult PreviousBillStatementPage(int loginID)
        {
            ViewBag.LoginID = loginID;
            return View();
        }

        public ViewResult AccountSummaryPage(int loginID)
        {
            ViewBag.LoginID = loginID;
            return View();
        }
    }
}