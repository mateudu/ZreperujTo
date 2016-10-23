using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using ZreperujTo.Web.Data;
using ZreperujTo.Web.Helpers;
using ZreperujTo.Web.Models;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.CategoryModels;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.DbModels;
using ZreperujTo.Web.Models.FailModels;
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Controllers
{
    [Route("Fails")]
    public class FailsController : Controller
    {
        private readonly IZreperujToService _serviceCore;

        public FailsController(IZreperujToService serviceCore)
        {
            _serviceCore = serviceCore;
        }

        [HttpGet("Browse")]
        [HttpGet("Browse/{categoryId}")]
        [HttpGet("Browse/{categoryId}/{subcategoryId}")]
        public async Task<IActionResult> Browse(
            string categoryId,
            string subcategoryId,
            [FromQuery] string city,
            [FromQuery] string district,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? validThrough,
            [FromQuery] List<SpecialRequirement> requirements,
            [FromQuery] string sortOrder,
            [FromQuery] int pageLimit = 10,
            [FromQuery] int pageNumber = 1
            )
        {
            ObjectId categoryObjectId, subcategoryObjectId;
            ObjectId.TryParse(categoryId, out categoryObjectId);
            ObjectId.TryParse(subcategoryId, out subcategoryObjectId);
            var obj = new BrowseFailsViewModel();

            var fails = _serviceCore.GetFailsMetaAsync(categoryObjectId, subcategoryObjectId, city, district, minPrice, maxPrice,
                validThrough, requirements, sortOrder, pageLimit, pageNumber);
            var categories = _serviceCore.GetCategoryReadModelsAsync();
            await Task.WhenAll(fails, categories);
            obj.Fails = fails.Result;
            obj.Categories = categories.Result;




            return View(obj);
        }

        // GET: Fail/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            ObjectId failId;
            FailDbModel failDbModel;
            if (!ObjectId.TryParse(id, out failId))
            {
                ModelState.AddModelError("failId", "'failId' is incorrect");
                return BadRequest(ModelState);
            }

            FailReadModel read = null;
            try
            {
                read = await _serviceCore.GetFailReadModelAsync(failId);
            }
            catch (FailDoesNotExistException)
            {
                ModelState.AddModelError("failId", "The fail does not exist");
                return BadRequest(ModelState);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return View(read);
        }

        // GET: Fails/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccessFailedCount,ConcurrencyStamp,Email,EmailConfirmed,LockoutEnabled,LockoutEnd,NormalizedEmail,NormalizedUserName,PasswordHash,PhoneNumber,PhoneNumberConfirmed,Points,SecurityStamp,TwoFactorEnabled,UserName")] ApplicationUser applicationUser)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(applicationUser);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction("Index");
            //}
            //return View(applicationUser);
            return RedirectToAction("Index");
        }

        public class BrowseFailsViewModel
        {
            public List<CategoryReadModel> Categories { get; set; }
            public List<FailMetaModel> Fails { get; set; }
        }
    }
}
