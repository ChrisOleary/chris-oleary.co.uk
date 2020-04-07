using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpotifyFutureAlbums.Controllers
{
    public class MoshController : Controller
    {
        // GET: Mosh
        public ActionResult Index()
        {
            var calculator = new Calculator();
            try
            {
                calculator.Add(null);
                Console.WriteLine($"The calculators answer is : {calculator}");
            }
            catch (Exception)
            {
                Console.WriteLine($"The input is {calculator}");
            }

            return View(calculator);
        }

       public class Calculator
        {
            public int Add(params int[] numbers)
            {
                int sum = 0;
                foreach (var tits in numbers)
                {
                    sum += tits;
                }
                return sum;
            }
        }

    }
}