﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ZreperujTo.Web.Data;
using ZreperujTo.Web.Models.CategoryModels;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Categories")]
    public class CategoriesApiController : Controller
    {
        private readonly ZreperujToDbClient _zreperujDb;

        public CategoriesApiController(ZreperujToDbClient zreperujDb)
        {
            _zreperujDb = zreperujDb;
        }

        // GET: api/Categories
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryReadModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            var categories = await _zreperujDb.GetCategoriesAsync();
            var subcategories = await _zreperujDb.GetSubcategoriesAsync();
            var result = categories.Select(x=>new CategoryReadModel(x)).ToList();
            foreach (var sub in subcategories)
            {
                var obj = result.FirstOrDefault(x => x.Id == sub.CategoryId.ToString());
                if (obj != null)
                {
                    if (obj.Subcategories == null)
                    {
                        obj.Subcategories = new List<SubcategoryReadModel>();
                    }
                    obj.Subcategories.Add(new SubcategoryReadModel(sub));
                }
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryWriteModel category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var obj = new CategoryDbModel
                {
                    Name = category.Name
                };
                await _zreperujDb.AddCategoryAsync(obj);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
            
        }

        [HttpPost("{categoryId}")]
        public async Task<IActionResult> AddSubcategory([FromBody] SubcategoryReadModel subcategory, string categoryId)
        {
            ObjectId categoryObjectId;
            if (!ObjectId.TryParse(categoryId, out categoryObjectId))
            {
                ModelState.AddModelError("categoryId", "Invalid 'categoryId' URI parameter");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categories = await _zreperujDb.GetCategoriesAsync();
            var category = categories.FirstOrDefault(x => x.Id == categoryObjectId);
            if (category == null)
            {
                ModelState.AddModelError("categoryId", "This category does not exist");
                return BadRequest(ModelState);
            }
            var model = new SubcategoryDbModel
            {
                CategoryId = categoryObjectId,
                Name = subcategory.Name
            };
            await _zreperujDb.AddSubcategoryAsync(model);
            return Ok();
        }
    }
}
