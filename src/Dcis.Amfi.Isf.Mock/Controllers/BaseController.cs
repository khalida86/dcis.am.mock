using Dcis.Amfi.Isf.Mock.Constants;
using Dcis.Amfi.Isf.Mock.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dcis.Amfi.Isf.Mock.Controllers
{
    public class BaseController : Controller
    {
        protected readonly MemCache _memoryCache;
        protected readonly IConfiguration _configuration;
        protected readonly string _environment;

        protected BaseController(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _configuration = configuration;
            _memoryCache = new MemCache(memoryCache);
            _environment = _configuration.GetValue(ViewDataKeys.Environment, "local");
        }

    }
}
