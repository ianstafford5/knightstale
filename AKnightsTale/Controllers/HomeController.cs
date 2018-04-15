using AKnightsTale.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AKnightsTale.Controllers
{
    public class HomeController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();
      
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Leaderboard()
        {
            var model =
                from s in db.Scores
                orderby s.Score descending
                select s;

            return View(model);
        }

        [Authorize]
        public ActionResult GameStates()
        {
            try
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                
                var model =
                    from gs in db.GameStates
                    where gs.Username == user.UserName
                    orderby gs.DateTime descending
                    select gs;

                return View(model);
                
            }
            catch (Exception e)
            {

            }

            return View();
        }

        public ActionResult DeleteGameState(int id)
        {
            GameStateModel gameStateModel = db.GameStates.Find(id);
            db.GameStates.Remove(gameStateModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult MyScores()
        {
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            var model =
                from s in db.Scores
                where s.Username == user.UserName
                orderby s.Score descending
                select s;

            return View(model);
        }
    }
}