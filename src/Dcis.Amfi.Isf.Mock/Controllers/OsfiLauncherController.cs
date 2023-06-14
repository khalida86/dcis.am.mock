using Dcis.Amfi.Isf.Mock.Constants;
using Dcis.Amfi.Isf.Mock.Helpers;
using Dcis.Amfi.Isf.Mock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Dcis.Amfi.Isf.Mock.Controllers
{
    public class OsfiLauncherController : BaseController
    {
        private readonly ILogger<OsfiLauncherController> _logger;

        public OsfiLauncherController(ILogger<OsfiLauncherController> logger, IMemoryCache memoryCache, IConfiguration configuration) : base(memoryCache, configuration)
        {
            _logger = logger;
        } 

        public IActionResult Index()
        {
            _logger.LogInformation("Get list of user ...");
            ViewData[ViewDataKeys.Environment] = _environment;
            ViewData[ViewDataKeys.LoginUser] = Request.Cookies["_email_"];
            IList<User>? users = _memoryCache.GetUsers(Request.Host.Value);
            _logger.LogInformation(JsonConvert.SerializeObject(users, Formatting.Indented));

            return View(users);
        }

        [HttpPost]
        public IActionResult LinkedEntities(string selectUser)
        {
            ViewData[ViewDataKeys.Environment] = _environment;
            ViewData[ViewDataKeys.SelectedUser] = _memoryCache.GetUser(selectUser, Request.Host.Value);
            ViewData[ViewDataKeys.OpenAMFIinNewTab] = _configuration.GetValue(ViewDataKeys.OpenAMFIinNewTab, false);

            _logger.LogInformation("Get list of entity ...");
            _logger.LogInformation($"Selected user: {selectUser}");
            IList<Entity>? entities = _memoryCache.GetEntities();
            _logger.LogInformation(JsonConvert.SerializeObject(entities, Formatting.Indented));

            return View(entities);
        }
    }
}
