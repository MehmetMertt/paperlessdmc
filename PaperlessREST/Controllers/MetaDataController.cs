using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaperlessREST.Domain.Entities;
using System.Reflection.Metadata.Ecma335;

namespace PaperlessREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    public class MetaDataController : Controller
    {

        // GET: MetaDataController
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<ActionResult<List<MetaData>>> GetMetaDatasAsync()
        {



            return Task.FromResult<ActionResult<List<MetaData>>>(Ok(list));
        }

        // GET: MetaDataController/<guuid>
        [HttpGet("{guuid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<ActionResult<MetaData>> GetMetaDataAsync(int guuid)
        {
            MetaData metaData = new MetaData
            {
                Author = "Mehmet",
                
                FileExtension = ".txt",
                Id = Guid.NewGuid(),
                Name = "Teststoff",
                OwnerId = 1,
                LastModified = DateTime.Now
            };
            return Task.FromResult<ActionResult<MetaData>>(Ok(metaData));
        }
    }
}