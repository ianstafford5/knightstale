using AKnightsTale.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AKnightsTale.Controllers
{
    public class HomeController : Controller
    {
        public static int downloads = 0;
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

        public FileResult Download()
        {
            downloads++;
            Debug.WriteLine(downloads);
            var FileVirtualPath = "~/App_Data/uploads/AKnightsTale.zip";
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Dashboard()
        {
            var checkpoint0Count = db.GameStates.Where(gs => gs.Checkpoint == 0).Count();
            var checkpoint1Count = db.GameStates.Where(gs => gs.Checkpoint == 1).Count();
            var checkpoint2Count = db.GameStates.Where(gs => gs.Checkpoint == 2).Count();
            var checkpoint3Count = db.GameStates.Where(gs => gs.Checkpoint == 3).Count();
            var checkpoint4Count = db.GameStates.Where(gs => gs.Checkpoint == 4).Count();

            var userCount = db.Users.Count();

            List<int> scores = new List<int>();
            List<int> count = new List<int>();

            foreach (var line in db.Scores.GroupBy(info => info.Score)
                        .Select(group => new {
                            Score = group.Key,
                            Count = group.Count()
                        })
                        .OrderBy(x => x.Score))
            {
                scores.Add(line.Score);
                count.Add(line.Count);
                Debug.WriteLine("**********************SCORES********************");
                Debug.WriteLine("{0} {1}", line.Score, line.Count);
            }

            var lives = db.GameStates.Select(gs => gs.Lives).Sum();
            var coins = db.GameStates.Select(gs => gs.CoinCount).Sum();
            var gems = db.GameStates.Select(gs => gs.GemCount).Sum();

            ViewBag.Checkpoint0Count = checkpoint0Count;
            ViewBag.Checkpoint1Count = checkpoint1Count;
            ViewBag.Checkpoint2Count = checkpoint2Count;
            ViewBag.Checkpoint3Count = checkpoint3Count;
            ViewBag.Checkpoint4Count = checkpoint4Count;
            ViewBag.Scores = scores;
            ViewBag.Counts = count;
            ViewBag.Lives = lives-3;
            ViewBag.Coins = coins/10;
            ViewBag.Gems = gems;
            ViewBag.Downloads = downloads;
            ViewBag.Users = userCount;

            return View();
        }
    }
}