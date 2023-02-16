using Microsoft.AspNetCore.Mvc;
using POC.WebPush.Notification.Web.Models;
using System.Diagnostics;
using WebPush;

namespace POC.WebPush.Notification.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration= configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string user, string endpoint, string p256dh, string auth) 
        {
            if(user == null) return BadRequest();

            if(Storage.GetUsersNames().Contains(user)) return BadRequest("Usuário já cadastrado");

            var subscription = new PushSubscription(endpoint, p256dh, auth);
            Storage.SaveSubscription(user, subscription);

            return View("Notify", Storage.GetUsersNames()); 
        }

        public IActionResult Notify() { return View(Storage.GetUsersNames()); }

        public IActionResult Notify(List<string> Users) { return View(Users); }

        [HttpPost]
        public IActionResult Notify(string message, string user) 
        {
            if(user == null) return BadRequest("Nenhum usuário foi selecionado");

            var subscription = Storage.GetSubscription(user);

            if (subscription == null) return BadRequest("Usuário não encontrado");

            var subject = _configuration.GetValue<string>("VAPID:subject");
            var publicKey = _configuration.GetValue<string>("VAPID:publicKey");
            var privateKey = _configuration.GetValue<string>("VAPID:privateKey");

            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
            var webPushClient = new WebPushClient();

            try
            {
                webPushClient.SendNotification(subscription, message, vapidDetails);
            }
            catch (Exception exception)
            {

                return BadRequest(exception);
            }

            return View(Storage.GetUsersNames());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class User 
    {
        public string Name { get; set; }
        public PushSubscription Subscription { get; set; }

        public User(string name, PushSubscription subscription)
        {
            Name= name;
            Subscription= subscription;
        }
    }

    public static class Storage 
    {
        public static List<User> Users { get; set; } = new List<User>();
        public static List<string> GetUsersNames() =>  Users.Select(x => x.Name).ToList();
        public static void SaveSubscription(string userName, PushSubscription subscription) =>
            Users.Add(new User(userName, subscription));
        public static PushSubscription GetSubscription(string userName)
        {
            return Users.SingleOrDefault(x => x.Name == userName).Subscription;
        }
    }
}