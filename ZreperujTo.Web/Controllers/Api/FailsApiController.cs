using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.FailModels;

namespace ZreperujTo.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Fails")]
    public class FailsApiController : Controller
    {
        // GET: api/Fails/Browse/{categoryId?}/{subcategoryId?}
        [HttpGet("Browse")]
        [HttpGet("Browse/{categoryId:int}")]
        [HttpGet("Browse/{categoryId:int}/{subcategoryId:int?}")]
        [ProducesResponseType(typeof(IEnumerable<FailListModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(
            int? categoryId,
            int? subcategoryId,
            [FromQuery] string city,
            [FromQuery] string district,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? validThrough,
            [FromQuery] List<SpecialRequirement> requirements,
            [FromQuery] string sortOrder)
        {
            return Ok(new
            {
                category = categoryId, subcategory = subcategoryId, city = city,
                district = district, minPrice = minPrice, maxPrice = maxPrice,
                validThrough = validThrough, requirements = requirements,
                sortOrder = sortOrder
            });
        }

        // GET: api/Fails/Details/5
        [HttpGet("Details/{id}")]
        [ProducesResponseType(typeof(FailReadModel),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(new {failId = id});
        }
        
        // POST: api/Fails/Create
        [HttpPost("Create")]
        [ProducesResponseType(typeof(FailReadModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]FailWriteModel writeModel)
        {
            return Ok(new {requestObject = writeModel});
        }
        
        //// PUT: api/Fails/
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}
        
        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
