using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpotifyFutureAlbums.Controllers
{
    public class MoshIntermediateController : Controller
    {

        public class Person
        {
            public string Name;

            public void Introduce(string to)
            {
                Console.WriteLine("Hi, {0}, my name is {1}", to, Name);
            }
        }

        // GET: MoshIntermediate
        public ActionResult Index()
        {
            var person = new Person();
            person.Name = "Yo Momma";
            person.Introduce("Chris");



            return View();
        }
    }
}