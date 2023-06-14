using Dcis.Amfi.Isf.Mock.Helpers;
using Dcis.Amfi.Isf.Mock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dcis.Amfi.Isf.Mock.Controllers
{
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IMemoryCache memoryCache, IConfiguration configuration) : base(memoryCache, configuration)
        {
            _logger = logger;
        }

        // GET api/v1/user/PE8NMZ4@localhost
        [HttpGet()]
        [Route("api/v1/user/{internalId}")]
        public User? Get(string internalId) => _memoryCache.GetUser(internalId, Request.Host.Value);

        // POST api/v1/user/generateJwt
        [HttpPost]
        [Route("api/v1/user/generateJwt")]
        public string GenerateJwtUid(User user) => JwtHelper.GenerateToken(user, _configuration);
    }
}
