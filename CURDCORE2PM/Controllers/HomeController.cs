using CURDCORE2PM.Dbcontent;
using CURDCORE2PM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace CURDCORE2PM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public object FormsAuthentication { get; private set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

    [Authorize]
        public IActionResult Index()
        {

            RcdbContext dbobj = new RcdbContext();

            var res = dbobj.Empdetails.ToList();
            List<empmodel> mobj = new List<empmodel>();
            foreach (var item in res)
            {
                mobj.Add(new empmodel
                {
                    Id=item.Id,
                    Name=item.Name,
                    Course=item.Course,
                    Gender=item.Gender,
                    Mobile=item.Mobile,
                    Email=item.Email
                });

            }
            return View(mobj);
        }
        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
          
            return View();
        }
        
        [HttpPost]
        [Authorize]
        public IActionResult Add(empmodel mobj)
        {
            Empdetail tb = new Empdetail();
            RcdbContext db = new RcdbContext();
            
            tb.Id = mobj.Id;
            tb.Name = mobj.Name;
            tb.Course = mobj.Course;
            tb.Gender = mobj.Gender;
            tb.Mobile = mobj.Mobile;
            tb.Email = mobj.Email;
            if (mobj.Id == 0)
            {
                db.Empdetails.Add(tb);
                db.SaveChanges();

            }
            else
            {
                db.Entry(tb).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();

            }

            return RedirectToAction("Index");
        }
        //LOgin method
        [HttpGet]
      
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult login(adminmodel adm)
        {
            RcdbContext db = new RcdbContext();
            var ures = db.Users.Where(m => m.Email == adm.Email).FirstOrDefault();
            if (ures == null)
            {
                TempData["msg"] = "Email not found";
            }
            else
            {
                if(ures.Email==adm.Email && ures.Password == adm.Password)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, ures.Name),
                                        new Claim(ClaimTypes.Email, ures.Email) };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties);


                    HttpContext.Session.SetString("Name", ures.Email);


                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["invalid"] = "invalid user name and password ";
                   return RedirectToAction("login");
                   
                }
             
            
               
            }
            return View("login");
        }

        [Authorize]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("login","Home");
        }
        [HttpGet]
        public IActionResult reg()
        {
            HttpContext.Session.SetString("name", "Rahul");
            return View();
        }
        [HttpPost]
        public IActionResult reg(adminmodel mobj)
        {
            RcdbContext db = new RcdbContext();
            User tb = new User();
            tb.Id = mobj.Id;
            tb.Name = mobj.Name;
            tb.Email = mobj.Email;
            tb.Password = mobj.Password;
            db.Users.Add(tb);
            db.SaveChanges();
            return RedirectToAction("login");
        }
        public IActionResult Privacy()
        {
            HttpContext.Session.SetString("name", "Rahul");
            return View();
        }
        [Authorize]
        public IActionResult Update(int id)
        {
            empmodel mobj = new empmodel();
            RcdbContext db = new RcdbContext();
            var res = db.Empdetails.Where(m => m.Id == id).FirstOrDefault();
            
            mobj.Id = res.Id;
            mobj.Name = res.Name;
            mobj.Course = res.Course;
            mobj.Gender = res.Gender;
            mobj.Mobile = res.Mobile;
            mobj.Email = res.Email;


            return View("Add",mobj);
        }
        [Authorize]
        public IActionResult Delete(int id)
        {
            RcdbContext db = new RcdbContext();
            var ditem = db.Empdetails.Where(m => m.Id == id).First();
            db.Empdetails.Remove(ditem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
