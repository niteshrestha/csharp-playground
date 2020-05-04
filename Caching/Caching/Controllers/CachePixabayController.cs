using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Caching.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Caching.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CachePixabayController : ControllerBase
    {
        private readonly ILogger<CachePixabayController> _logger;
        private readonly IDistributedCache _cache;

        public CachePixabayController(ILogger<CachePixabayController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string query)
        {
            var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
            var encodedCache = await _cache.GetAsync(query);
            object cacheEntry;
            if (encodedCache != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(encodedCache))
                {
                    cacheEntry = bf.Deserialize(ms);
                    return Ok(cacheEntry);
                }
            }

            var data = await GetData();
            byte[] encodedData;
            if (data != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, data);
                    encodedData = ms.ToArray();
                }
                await _cache.SetAsync(query, encodedData, cacheOptions);
            }
            return Ok(data);
        }

        private async Task<CacheResult> GetData()
        {
            await Task.Delay(2000);
            return new CacheResult()
            {
                CacheTime = DateTime.Now
            };
        }
    }
}
