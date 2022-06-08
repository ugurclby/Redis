using InMemoryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

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
            // Basit Tiplerin Cache atılması
            //_distributedCache.SetString("name", "uğur", new DistributedCacheEntryOptions { AbsoluteExpiration = DateTime.Now.AddSeconds(60) });
            //await _distributedCache.SetStringAsync("surname", "çalbay");

            //Complex tiplerin cache atılması
            Product product = new Product { Id=1,Name="Kalem",Price=23};

            string jsonData = JsonConvert.SerializeObject(product);
            _distributedCache.SetString("product:1", jsonData);

            Product productbinary = new Product { Id = 2, Name = "Silgi", Price = 25 };
            string jsonBinary = JsonConvert.SerializeObject(productbinary);

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonBinary);
            _distributedCache.Set("product:2", byteArray);


            return View();
        }
        public IActionResult Show()
        {
            //ViewData["Name"] = _distributedCache.GetString("name");
            //ViewData["SurName"] = _distributedCache.GetString("surname"); 
            var sonuc = JsonConvert.DeserializeObject<Product>(_distributedCache.GetString("product:1"));
            ViewData["Product"] = sonuc;
            var sonuc2 = JsonConvert.DeserializeObject<Product> (Encoding.UTF8.GetString(_distributedCache.Get("product:2")));
            ViewData["Produc2"] = sonuc2;
            return View();
        }
        public IActionResult Remove()
        {
            //_distributedCache.Remove("name");
            //_distributedCache.Remove("surname");

            return View();
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/testresim.jpg");

            byte[] imageArray = System.IO.File.ReadAllBytes (path);
            _distributedCache.Set("image", imageArray);
            return View();
        }

        public IActionResult ImageUrl()
        {
            var image= _distributedCache.Get("image");

            return File(image,"image/jpg");
        }
    }
}
