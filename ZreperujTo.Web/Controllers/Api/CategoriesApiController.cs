using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Categories")]
    public class CategoriesApiController : Controller
    {
        // GET: api/Categories
        [HttpGet]
        [ProducesResponseType(typeof(List<Category>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            return Ok(new List<Category>
            {
                new Category
                {
                    CategoryId = 1,
                    Name = "Zwierz?ta",
                    Subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                            SubcategoryId = 1,
                            Name = "Spacery"
                        },
                        new Subcategory
                        {
                            SubcategoryId = 2,
                            Name = "Opieka"
                        }
                    }
                },
                new Category
                {
                    CategoryId = 1,
                    Name = "Motoryzacja",
                    Subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                            SubcategoryId = 1,
                            Name = "Serwis"
                        },
                        new Subcategory
                        {
                            SubcategoryId = 2,
                            Name = "Wulkanizacja"
                        }
                    }
                }
            });
        }
    }
}
