using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZreperujTo.Web.Helpers;
using ZreperujTo.Web.Models;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.DbModels;
using ZreperujTo.Web.Models.FailModels;

namespace ZreperujTo.Web.Controllers
{
    [Authorize]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IZreperujToService _serviceCore;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, 
            IZreperujToService serviceCore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _serviceCore = serviceCore;
        }

        [HttpGet("")]
        [HttpGet("userId")]
        public async Task<IActionResult> Index(string userId=null)
        {
            bool isOwner = false;
            if (String.IsNullOrWhiteSpace(userId))
            {
                userId = _userManager.GetUserId(User);
                isOwner = true;
            }
            var user = _serviceCore.GetUserInfoDbModelAsync(userId);
            var bids = _serviceCore.GetBidDbModelsAsync(userId);
            var fails = _serviceCore.GetUserFailsMetaAsync(userId);
            await Task.WhenAll(user, bids, fails);
            var userBidFails = await _serviceCore.GetFailsMetaAsync(bids.Result.Select(x => x.FailId).ToList());

            var list = new List<KeyValuePair<BidDbModel, FailMetaModel>>();
            foreach (var bid in bids.Result)
            {
                var fail = userBidFails.FirstOrDefault(x => x.Id == bid.FailId.ToString());
                list.Add(new KeyValuePair<BidDbModel, FailMetaModel>(bid, fail));
            }
            var result = new UserInfoViewModel
            {
                User = user.Result,
                BidAndFails = list,
                Fails = fails.Result,
                IsOwner = isOwner
            };

            return View(result);
        }

        public class UserInfoViewModel
        {
            public UserInfoDbModel User { get; set; }
            public List<KeyValuePair<BidDbModel, FailMetaModel>> BidAndFails { get; set; }
            public List<FailMetaModel> Fails { get; set; }
            public bool IsOwner { get; set; }
        }

        public static string GetBidBackground(BidDbModel model)
        {
            if (model.Assigned)
            {
                if (model.Active)
                {
                    return "bg-success";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                if (model.Active)
                {
                    return "bg-warning";
                }
                else
                {
                    return "bg-warning";
                }
            }
        }
    }
}