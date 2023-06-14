using Dcis.Amfi.Isf.Mock.Constants;
using Dcis.Amfi.Isf.Mock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Dcis.Amfi.Isf.Mock.Controllers
{
    public class IsfUidController : BaseController
    {
        private readonly ILogger<IsfUidController> _logger;

        public IsfUidController(ILogger<IsfUidController> logger, IMemoryCache memoryCache, IConfiguration configuration) : base(memoryCache, configuration)
        {
            _logger = logger;
        }

        // GET: IsfUidController
        public ActionResult Index()
        {
            _logger.LogInformation("Get list of user ...");
            ViewData[ViewDataKeys.Environment] = _environment;
            ViewData[ViewDataKeys.LoginUser] = Request.Cookies["_email_"];
            IList<User>? users = _memoryCache.GetUsers(Request.Host.Value);
            _logger.LogInformation(JsonConvert.SerializeObject(users, Formatting.Indented));

            return View(users);
        }

        // POST: IsfUidController/Generate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(string selectUser, string additionalClaims, string btnGenerate, User? user)
        {
            ViewData[ViewDataKeys.Environment] = _environment;

            if (!string.IsNullOrWhiteSpace(selectUser))
            {
                ModelState.Clear();
                user = _memoryCache.GetUser(selectUser, Request.Host.Value);
            }

            if (user != null && !string.IsNullOrWhiteSpace(additionalClaims))
            {
                user.Claims = JsonConvert.DeserializeObject<IList<UserClaim>>(additionalClaims);
            }

            if (!string.IsNullOrWhiteSpace(btnGenerate))
            {
                user?.GenerateUid(_configuration);
            }

            return View(user);
        }

    }
}
