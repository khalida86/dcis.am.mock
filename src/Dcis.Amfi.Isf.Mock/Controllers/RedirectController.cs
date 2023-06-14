using Dcis.Amfi.Isf.Mock.Helpers;
using Dcis.Amfi.Isf.Mock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dcis.Amfi.Isf.Mock.Controllers
{
    [ApiController]
    public class RedirectController : BaseController
    {
        private readonly ILogger<RedirectController> _logger;

        public RedirectController(ILogger<RedirectController> logger, IMemoryCache memoryCache, IConfiguration configuration) : base(memoryCache, configuration)
        {
            _logger = logger;
        }

        /// <summary>
        /// set sessionStorage/cookies for jwtToken, entity infocus and selected user for the AMFI mock interceptor to use
        /// and redirect to AMFI UI
        /// cookies are needed if reverse proxy(NGINX) is not used
        /// </summary>
        /// <param name="jwtToken">the jwt token</param>
        /// <param name="selectedUser">the selected user</param>
        /// <param name="entityInfocus">the entity infocus</param>
        /// <param name="redirectUrl">the redirect url</param>
        /// <returns></returns>
        private ContentResult generateRedirectContents(string? jwtToken, string? redirectUrl, string? selectedUser = "", string? entityInfocus = "")
        {
            StringBuilder content = new($"sessionStorage.setItem('Token', '{jwtToken}');");

            if (!string.IsNullOrWhiteSpace(selectedUser))
            {
                content.Append($"sessionStorage.setItem('_email_', '{selectedUser}'); document.cookie = '_email_={selectedUser}; path=/';");
            }

            if (!string.IsNullOrWhiteSpace(entityInfocus))
            {
                content.Append($"sessionStorage.setItem('_entity_infocus_', '{entityInfocus}'); document.cookie = '_entity_infocus_={entityInfocus}; path=/';");
            }

            content.Append($"window.location.href = '{redirectUrl}';");

            return Content($"<html><body><script>{content}</script></body></html>", "text/html");
        }

        /// <summary>
        /// Redirect to AMFI ui
        /// </summary>
        /// <param name="selectedEntity">the selected entity</param>
        /// <param name="selectedUser">the selected user</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("api/[controller]/ToAmfi")]
        public IActionResult ToAmfi([FromForm] string selectedEntity, [FromForm] string selectedUser)
        {
            _logger.LogInformation($"Selected user: {selectedUser}, entity: {selectedEntity}");

            // get user and the selected entity object
            var user = _memoryCache.GetUser(selectedUser, Request.Host.Value);
            var entity = _memoryCache.GetEntity(selectedEntity);
            // generate the user JwtToken
            var jwtToken = JwtHelper.GenerateToken(user, _configuration);
            // generate the AMFI UI landing url
            var redirectUrl = user.RedirectUrl
                                  .Replace("{entityType}", entity.ExternalIdType)
                                  .Replace("{entityId}", entity.ExternalIdValue);
            // generate the entityInfocus object
            var entityInfocus = JsonConvert.SerializeObject(new { scheme = entity?.ExternalIdType, value = entity?.ExternalIdValue });

            // set sessionStorage/cookies for jwtToken, entity infocus and selected user for the AMFI mock interceptor to use
            // and redirect to AMFI UI
            // cookies are needed if reverse proxy(NGINX) is not used
            return generateRedirectContents(jwtToken, redirectUrl, selectedUser, entityInfocus);
        }

        // POST api/<RedirectController>
        /// <summary>
        /// Redirect to Claim Authorisation landing page
        /// </summary>
        /// <param name="txtSelectedUser">the currently login user</param>
        /// <param name="txtClaimAuthUrl">the claim authorisation url</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("api/[controller]/ToClaimAuth")]
        public IActionResult ToClaimAuth([FromForm] string txtSelectedUser, [FromForm] string txtClaimAuthUrl) {
            _logger.LogInformation($"Selected user: {txtSelectedUser}, claimAuthUrl: {txtClaimAuthUrl}");

            // get user and the selected entity object
            var user = _memoryCache.GetUser(txtSelectedUser, Request.Host.Value);
            // generate the user JwtToken
            var jwtToken = JwtHelper.GenerateToken(user, _configuration);

            // set sessionStorage/cookies for jwtToken, entity infocus and selected user for the AMFI mock interceptor to use
            // and redirect to AMFI UI
            // cookies are needed if reverse proxy(NGINX) is not used
            return generateRedirectContents(jwtToken, txtClaimAuthUrl, txtSelectedUser);
        }

        // POST api/<RedirectController>
        /// <summary>
        /// Redirect to Url with uid Token
        /// </summary>
        /// <param name="user">the updated user</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("api/[controller]/ToUrl")]
        public IActionResult ToUrl([FromForm] User user)
        {
            _logger.LogInformation($"Selected user: {JsonConvert.SerializeObject(user, Formatting.Indented)}");

            // generate the user JwtToken
            var jwtToken = JwtHelper.GenerateToken(user, _configuration);

            // set sessionStorage/cookies for jwtToken, and redirect to URL
            // cookies are needed if reverse proxy(NGINX) is not used
            return generateRedirectContents(jwtToken, user.RedirectUrl, user.InternalIdentityId);
        }
    }
}
