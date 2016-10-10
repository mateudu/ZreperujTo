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
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Controllers.Api
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("api/Profile")]
    public class ProfileApiController : Controller
    {
        private readonly ZreperujToDbClient _zreperujDb;

        public ProfileApiController(ZreperujToDbClient zreperujDb)
        {
            _zreperujDb = zreperujDb;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("Info")]
        [ProducesResponseType(typeof(UserInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserInfo()
        {
            string userId = User.Claims.FirstOrDefault(
                        x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!String.IsNullOrWhiteSpace(userId))
            {
                var obj = await _zreperujDb.GetUserInfoDbModelAsync(userId);
                var result = new UserInfo
                {
                    Email = obj.Email,
                    Name = obj.Name,
                    MobileNumber = obj.MobileNumber,
                    UserId = obj.UserId,
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
    }
}