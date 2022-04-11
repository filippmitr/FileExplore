using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Explorer.Models;
using System.Web.Script.Serialization;

namespace Explorer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ExplorerEntities newContainer = new ExplorerEntities();
            bool exDb = newContainer.Database.CreateIfNotExists();
            if (exDb || !newContainer.Folders.Any())
            {
                 new CreateDB(AppDomain.CurrentDomain.BaseDirectory);
            }
            return View();
        }
    }
}