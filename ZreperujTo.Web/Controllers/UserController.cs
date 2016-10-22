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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = _serviceCore.GetUserInfoDbModelAsync(_userManager.GetUserId(User));
            var bids = _serviceCore.GetBidDbModelsAsync(_userManager.GetUserId(User));
            var fails = _serviceCore.GetUserFailsMetaAsync(_userManager.GetUserId(User));
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
                Fails = fails.Result
            };

            return View(result);
        }

        public class UserInfoViewModel
        {
            public UserInfoDbModel User { get; set; }
            public List<KeyValuePair<BidDbModel, FailMetaModel>> BidAndFails { get; set; }
            public List<FailMetaModel> Fails { get; set; }
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