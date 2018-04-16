using AKnightsTale.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AKnightsTale.Controllers
{
    [RoutePrefix("game")]
    public class GameController : ApiController
    {

        [HttpGet]
        [Route("score")]
        public IHttpActionResult GetScores()               // GET
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    var scores = db.Scores.OrderByDescending(s => s.Score).ToList().Take(10);

                    // Puts Scores in Dictionary to be legible for unity JsonUtility function
                    int key = 1;
                    var toDict = scores.Select(s => new { id = "Score" + key++, score = s }).ToDictionary(a => a.id, a => a.score);

                    return Ok(toDict);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    return NotFound();
                }
            }
        }

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage RegisterAsync(RegisterViewModel model)
        {
            var modelStateList = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            Dictionary<string, string> errorList = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string[]> kvp in modelStateList)
            {
                Debug.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value[0]);
                if (kvp.Key.Length > 0)
                {
                    int i = kvp.Key.IndexOf(".") + 1;
                    string key = kvp.Key.Substring(i);
                    Debug.WriteLine(key);
                    errorList.Add(key, kvp.Value[0]);
                    //modelStateList.Remove(kvp.Key);
                }
                Debug.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value[0]);
            }

            ApplicationUser userEmail = null;
            ApplicationUser username = null;

            if (ModelState.IsValid)
            {
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                userEmail = userManager.FindByEmail(model.Email);
                username = userManager.FindByName(model.Username);
            }

            if (!errorList.ContainsKey("Email"))
            {
                if (userEmail != null)
                {
                    errorList.Add("Email", "Email is already taken!");
                }
            }
            

            if (!errorList.ContainsKey("Username"))
            {
                if (username != null)
                {
                    errorList.Add("Username", "Username is already taken!");
                }
            }

            //var errors = new ScoresController().MyDictionaryToJson(errorList);
            if (!errorList.ContainsKey("Password"))
            {
                bool containsInt = model.Password.Any(char.IsDigit);
                bool containsNon = model.Password.Any(ch => !Char.IsLetterOrDigit(ch));
                bool containsUpper = model.Password.Any(char.IsUpper);
                bool containsLower = model.Password.Any(char.IsLower);

                if (!containsInt)
                {
                    errorList.Add("Password", "Passwords must have at least one digit ('0'-'9').");
                }
                else if (!containsNon)
                {
                    errorList.Add("Password", "Passwords must have at least one non letter or digit character.");
                }
                else if (!containsUpper)
                {
                    errorList.Add("Password", "Passwords must have at least one uppercase ('A'-'Z').");
                }
                else if (!containsLower)
                {
                    errorList.Add("Password", "Passwords must have at least one lowercase ('a'-'z').");
                }
            }

            if (errorList.Count <= 0)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var newUser = new ApplicationUser() { Email = model.Email, UserName = model.Username };

                Debug.WriteLine("**************************Model Valid****************************");
                //Debug.WriteLine("Name: " + model.Email + "\nPassword: " + model.Password);
                var createUser = UserManager.Create(newUser, model.Password);

                if (createUser.Succeeded)
                {
                    Debug.WriteLine("**************************SUCCESS!!!****************************");
                    HttpResponseMessage okResponse = Request.CreateResponse(HttpStatusCode.OK);
                    return okResponse;
                }


            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, errorList);
            return response;
        }

        [HttpPost]
        [Route("login")]
        public async System.Threading.Tasks.Task<IHttpActionResult> LoginAsync(LoginViewModel model)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); ;
                    var user = await userManager.FindAsync(model.Username, model.Password);

                    if (user == null)
                    {
                        return BadRequest();
                    }

                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
        }

        [HttpPost]
        [Route("score")]
        public IHttpActionResult AddScore(SendScoreModel form)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    // add and save
                    if (ModelState.IsValid)
                    {
                        if (form.Valid)
                        {
                            ScoreModel score = new ScoreModel
                            {
                                Username = form.Username,
                                Score = form.Score
                            };
                            db.Scores.Add(score);
                            db.SaveChanges();
                            return Ok();
                        }
                        return BadRequest();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return BadRequest();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("highscore")]
        public IHttpActionResult HighScore(string user)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (user != null)
                        {
                            var sc = db.Scores.Where(s => s.Username == user).Max(s => s.Score);
                            if (sc != 0)
                            {
                                return Ok(sc);
                            }
                        }
                        return BadRequest();
                    }
                    return BadRequest();
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
        }

        [HttpPost]
        [Route("gamestate")]
        public IHttpActionResult AddGameState(GameStateModel model)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    var gameStates = db.GameStates.Where(g => g.Username == model.Username).OrderBy(g => g.DateTime).ToList();

                    if (gameStates.Count < 3)
                    {
                        db.GameStates.Add(model);
                        db.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        var oldGameState = gameStates[0];
                        var oldestGameState = db.GameStates.Where(g => g.ID == oldGameState.ID).First();
                        db.GameStates.Remove(oldestGameState);
                        db.GameStates.Add(model);
                        db.SaveChanges();
                        return Ok();
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
        }

        [HttpGet]
        [Route("gamestate")]
        public IHttpActionResult GetGameStates(string username)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    var gameStates = db.GameStates.Where(g => g.Username == username).OrderByDescending(g => g.DateTime).ToList();

                    // Puts GameStates in Dictionary to be legible for unity JsonUtility function
                    int key = 1;
                    var toDict = gameStates.Select(gs => new { id = "GameState" + key++, gamestate = gs }).ToDictionary(a => a.id, a => a.gamestate);

                    return Ok(toDict);
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
        }


        //[HttpDelete]
        //[Route("gamestate")]
        //public IHttpActionResult DeleteGameState(DeleteGameStateForm delete)
        //{
        //    using (ApplicationDbContext db = new ApplicationDbContext())
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var oldestGameState = db.GameStates.Where(g => g.ID == delete.ID).First();
        //                db.GameStates.Remove(oldestGameState);
        //                db.SaveChanges();
        //                return Ok();
        //            }
        //            return BadRequest();
        //        }
        //        catch (Exception e)
        //        {
        //            return BadRequest(e.ToString());
        //        }
        //    }
        //}

    }
}
