using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using PaperlessREST.Application.DTOs;
using PaperlessREST.Application.Commands;
using PaperlessREST.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using PaperlessREST.DataAccess.Service;

namespace PaperlessREST.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    public class MetaDataController : Controller
    {
        private readonly IMetaDataService _metaDataService;

        public MetaDataController(IMetaDataService metaDataService)
        {
            _metaDataService = metaDataService;
        }

        // GET: MetaDataController
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<List<MetaData>> GetMetaDatas()
        {
            var metaDatas = _metaDataService.GetAllMetaData();

            if (!metaDatas.Any())
            {
                return NoContent();
            }

            return Ok(metaDatas);
        }

        // GET: MetaDataController/<guuid>
        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<MetaData> MetaDataAsync(Guid guid)
        {
            var metaData = _metaDataService.GetMetaDataByGuid(guid);

            if (metaData == null) 
                return NoContent();

            return Ok(metaData);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateMetaData(CreateMetaDataCommand command)
        {
           // ValidationResult result = validator.Validate(command);
            var createdMetaData = _metaDataService.CreateMetaData(command);
            return CreatedAtAction(nameof(GetMetaDatas), new { guid = createdMetaData.Id },
                new MetaDataDto(createdMetaData.Id,command.Title, command.FileType, command.FileSize, command.Summary, command.ModifiedLast,command.CreatedOn));
        }
    }
}