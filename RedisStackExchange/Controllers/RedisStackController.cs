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


        public IActionResult HashListType() // Redis tarafında set sınıfına karşılık gelir.
        {
            HashSet<string> model = new HashSet<string>();

            if (db.KeyExists("hashliste"))
            {
                db.SetMembers("hashliste").ToList().ForEach(x=> model.Add(x));
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult HashListAdd(string data)
        {
            var sonuc = db.SetAdd("hashliste", data);
            db.KeyExpire("hashliste", TimeSpan.FromMinutes(1));

            return RedirectToAction("HashListType");
        }

        public IActionResult HashListDelete(string data)
        {
            db.SetRemove("hashliste", data);

            return RedirectToAction("HashListType");
        }


        public class SortedModel
        {
            public double Score { get; set; }
            public string Element { get; set; }
        }

        public IActionResult SortedSetType() 
        {
            HashSet<SortedModel> model = new HashSet<SortedModel>();

            if (db.KeyExists("sortedliste"))
            {
                var sonucWithScore = db.SortedSetRangeByScoreWithScores("sortedliste");
                var sonuc2 = db.SortedSetScan("sortedliste").ToList();

                db.SortedSetScan("sortedliste" ).ToList().ForEach(x => model.Add(new SortedModel() { Element=x.Element,Score=x.Score}));
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SortedSetAdd(string data,int score)
        {
            var sonuc = db.SortedSetAdd("sortedliste", data, score);
            db.KeyExpire("sortedliste", TimeSpan.FromMinutes(1));

            return RedirectToAction("SortedSetType");
        }

        public IActionResult SortedSetDelete(string data)
        {
            db.SortedSetRemove("sortedliste", data); 

            return RedirectToAction("SortedSetType");
        }


        public IActionResult HashType() // Dictionary tipine karşılık gelir
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();      

            if (db.KeyExists("hashliste"))
            {
                  db.HashGetAll("hashliste").ToList().ForEach(x => keyValuePairs.Add(x.Name, x.Value)); 
            }

            return View(keyValuePairs);
        }

        [HttpPost]
        public IActionResult HashTypeAdd(string key, string val)
        {
            var sonuc = db.HashSet("hashliste", key, val); 

            return RedirectToAction("HashType");
        }

        public IActionResult HashDelete(string data)
        {
            db.HashDelete("hashliste", data);

            return RedirectToAction("HashType");
        }


    }
}
