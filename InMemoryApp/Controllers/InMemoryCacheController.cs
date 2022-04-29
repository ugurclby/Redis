using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.WebEncoders.Testing;

namespace InMemoryApp.Controllers
{
    public class InMemoryCacheController : Controller
    {
        private  readonly  IMemoryCache _memoryCache;

        public InMemoryCacheController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            // TryGetValue zaman isminde key var mı diye bakıp bool döner ve değeri döner
            //if (!_memoryCache.TryGetValue("zaman",out string zamanValue)) // 
            //{
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            //}

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            //options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
            options.SlidingExpiration = TimeSpan.FromSeconds(10);
            options.Priority = CacheItemPriority.High; // Cache silinme önceliği
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set("callback", $"{key} > {value} >> {reason}");
            });
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(),options);
            return View();
        }
        public IActionResult Show()
        {
            //GetOrCreate zaman isminde key bakar yoksa oluşturur.
            //_memoryCache.GetOrCreate("zaman", entry =>
            //{
            //    return DateTime.Now.ToString();
            //});
            _memoryCache.TryGetValue("zaman", out string zamanValue);
            _memoryCache.TryGetValue("callback", out string reasonValue);
            ViewData["Zaman"] = zamanValue;
            ViewData["Callback"] = reasonValue;
            return View();
        }

        public T Test<T>(T testsinif)
        {
            return testsinif;
        } 
    }
}
