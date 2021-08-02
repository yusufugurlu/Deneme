using Deneme.Entity;
using Deneme.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Deneme.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //Burada newleme yapmamak gerekir onun yerine ORMBase de OT kavramı ile static bir yapı olarak alıyoruz.
            //CategoriesORM categories = new CategoriesORM();
          
            /*var Insert=  CategoriesORM.Current.Insert(new Categories()
               {
                   CategoryName="Kitap",
                   Description="Deneme"            
               });*/
            var categories1 = CategoriesORM.Current.Select();
            /*var delete = CategoriesORM.Current.Delete(new Categories()
            {
                CategoryID=9
            });*/

            var update = CategoriesORM.Current.Update(new Categories()
            {
                CategoryID=10,
                CategoryName = "Kitap123",
                Description = "Deneme123"
            });

          var d=  RegionORM.Current.Insert(new Region() {
                RegionDescription="Turkey",
                RegionID=5
            });
            var region= RegionORM.Current.Select();
            return View(categories1.Data);
        }
    }
}