using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace InMemoryApp.Controllers
{
    public class DistributedCacheRedisController : Controller
    {
        private IDistributedCache _distributedCache;
        public DistributedCacheRedisController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public IActionResult Index()
        {
            _distributedCache.SetString("name", "uğur", new DistributedCacheEntryOptions { AbsoluteExpiration = DateTime.Now.AddSeconds(60) });

            return View();
        }
        public IActionResult Show()
        {
            ViewData["Name"] = _distributedCache.GetString("name"); 

            return View();
        }
        public IActionResult Remove()
        {
            _distributedCache.Remove("name");

            return View();
        }



    }
}
