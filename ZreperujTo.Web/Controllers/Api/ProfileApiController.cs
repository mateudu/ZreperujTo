using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.v3;
using ZreperujTo.Web.Data;
using ZreperujTo.Web.Helpers;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.FailModels;
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Controllers.Api
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("api/Profile")]
    public class ProfileApiController : Controller
    {
        private readonly IZreperujToService _serviceCore;

        public ProfileApiController(ZreperujToDbClient serviceCore)
        {
            _serviceCore = serviceCore;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("Info")]
        [ProducesResponseType(typeof(UserInfoReadModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserInfo()
        {
            string userId = User.Claims.FirstOrDefault(
                        x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!String.IsNullOrWhiteSpace(userId))
            {
                var obj = await _serviceCore.GetUserInfoDbModelAsync(userId);
                var result = new UserInfoReadModel
                {
                    Email = obj.Email,
                    Name = obj.Name,
                    MobileNumber = obj.MobileNumber,
                    Id = obj.UserId,
                    Company = obj.Company,
                    RatingSum = obj.RatingSum,
                    RatingCount = obj.RatingCount,
                    Ratings = obj.Ratings,
                    Badges = obj.Badges
                };
                return Ok(result);
            }
            else return NoContent();
        }

        [HttpGet("Info/Bids")]
        [ProducesResponseType(typeof(List<BidReadModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBids()
        {
            string userId = User.Claims.FirstOrDefault(
                        x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var obj = await _serviceCore.GetBidDbModelsAsync(userId);
            return Ok(obj);
        }

        [HttpGet("Info/Fails")]
        [ProducesResponseType(typeof(List<FailMetaModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFailsMeta()
        {
            string userId = User.Claims.FirstOrDefault(
                        x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var fails = await _serviceCore.GetUserFailsMetaAsync(userId);
            return Ok(fails);
        }
    }
}