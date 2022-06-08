using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisStackExchange.Models;
using RedisStackExchange.Services;
using StackExchange.Redis;

namespace RedisStackExchange.Controllers
{
    public class RedisStackController : Controller
    {
        private readonly RedisServices _redis;
        private readonly IDatabase db;
        public RedisStackController(RedisServices redis)
        {
            _redis = redis;
            db = _redis.GetDb(0);
        }
        public IActionResult Index()
        { 
            return View();
        }

        public IActionResult StringType()
        {
            db.StringSet("name", "uğur", TimeSpan.FromSeconds(100));
            db.StringSet("katılımcı", 1);
            RedisTest redisTest = new RedisTest() { ID = 1, Name = "Uğur", SurName = "Çalbay" };

            var jsonObject = JsonConvert.SerializeObject(redisTest);
            db.StringSet("RedisClass", jsonObject);
            return View();
        }

        public IActionResult StringTypeShow()
        {
            var value = db.StringGet("name");
            var katılımcı =  db.StringIncrement("katılımcı", 10);
            katılımcı = db.StringDecrementAsync("katılımcı", 1).Result; // async methodan sonuç döndüğü zaman result kullanılır.
            //db.StringDecrementAsync("katılımcı", 2).Wait(); // async methodan dönen sonucu kullanmak istemiyorsan wait kullanılır.
            var range = db.StringGetRange("name", 0, 2);
            var length = db.StringLength("name");

            var RedisClass = JsonConvert.DeserializeObject<RedisTest>(db.StringGet("RedisClass"));
             
            if (value.HasValue)
            {
                ViewData["name"] = value;
                ViewData["katılımcı"] = katılımcı;
                ViewData["range"] = range;
                ViewData["length"] = length;
                ViewData["redistest"] = RedisClass;
            }

            return View();
        }

        public IActionResult LinkedListType()
        {
            List<string> model = new List<string>();

            if (db.KeyExists("liste"))
            {
                db.ListRange("liste", 0, -1).ToList().ForEach(x => model.Add(x));
            }
             
            return View(model);
        }
        [HttpPost]
        public IActionResult LinkedListAdd(string data)
        {
            db.ListRightPush("liste", data);

            return RedirectToAction("LinkedListType");
        }
         
        public IActionResult LinkedListDelete(string data)
        {
            db.ListRemoveAsync("liste", data); 
             
            return RedirectToAction("LinkedListType");
        }
        public IActionResult LinkedListFirstDelete()
        {
            db.ListLeftPop("liste");

            return RedirectToAction("LinkedListType");
        }


    }
}
