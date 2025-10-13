using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using PaperlessREST.Infrastructure;
using PaperlessREST.Application.Commands;
using PaperlessREST.Application.DTOs;
using PaperlessREST.DataAccess.Service;
using PaperlessREST.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace PaperlessREST.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    public class MetaDataController : Controller
    {
        private readonly IMetaDataService _metaDataService;
        private readonly RabbitMqService _rabbit;
        private readonly ILogger<MetaDataController> _logger;

        public MetaDataController(IMetaDataService metaDataService, RabbitMqService rabbit, ILogger<MetaDataController> logger)
        {
            _metaDataService = metaDataService;
            _rabbit = rabbit;
            _logger = logger;
        }

        // GET: MetaDataController
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<List<MetaData>> GetMetaDatas()
        {
            try
            {
                var metaDatas = _metaDataService.GetAllMetaData();

                if (!metaDatas.Any())
                {
                    _logger.LogInformation("No metadata found.");
                    return NoContent();
                }

                return Ok(metaDatas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching metadata list.");
                return StatusCode(500, "Internal server error while fetching metadata.");
            }
        }

        // GET: MetaDataController/<guuid>
        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<MetaData> MetaDataAsync(Guid guid)
        {
            try
            {
                var metaData = _metaDataService.GetMetaDataByGuid(guid);

                if (metaData == null)
                {
                    _logger.LogWarning("Metadata with ID {Guid} not found.", guid);
                    return NoContent();
                }

                return Ok(metaData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching metadata with ID {Guid}", guid);
                return StatusCode(500, "Internal server error while fetching metadata.");
            }
        }

        // DELETE: MetaDataController/<guuid> 

        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public ActionResult DeleteMetaData(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    _logger.LogWarning("Attempted to delete metadata with empty GUID.");
                    return BadRequest("Invalid GUID.");
                }

                _metaDataService.DeleteMetadata(guid);
                return Ok(new { deletedId = guid });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Metadata with ID {Guid} not found for deletion.", guid);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting metadata with ID {Guid}", guid);
                return StatusCode(500, "Internal server error while deleting metadata.");
            }
        }

        // PUT: api/documents/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult UpdateMetaData(Guid id, [FromBody] MetaData updatedMetadata)
        {
            try
            {
                // Ensure IDs match
                if (id != updatedMetadata.Id)
                {
                    _logger.LogWarning("ID mismatch: route ID {Id} does not match body ID {BodyId}.", id, updatedMetadata.Id);
                    return BadRequest("Document ID mismatch.");
                }

                _metaDataService.UpdateMetadata(updatedMetadata);
                // Return the updated document
                return Ok(updatedMetadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating metadata with ID {Id}", id);
                return StatusCode(500, "Internal server error while updating metadata.");
            }
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateMetaData(CreateMetaDataCommand command)
        {
            try
            {
                var createdMetaData = _metaDataService.CreateMetaData(command);
                return CreatedAtAction(nameof(GetMetaDatas), new { guid = createdMetaData.Id },
                    new MetaData(createdMetaData.Id, command.Title, command.FileType, command.FileSize, command.Summary, command.ModifiedLast, command.CreatedOn));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating metadata entry.");
                return StatusCode(500, "Internal server error while creating metadata.");
            }
           // ValidationResult result = validator.Validate(command);
            
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file)
        {
            try
            {
                // code for saving file here ...
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload failed: no file provided or file was empty.");
                    return BadRequest("No file provided.");
                }

                // sends message to rabbitmq:
                var message = $"New document uploaded: {file.FileName}";
                _rabbit.SendMessage(message);

                _logger.LogInformation("Document uploaded and message sent to RabbitMQ: {file}", file.FileName);

                return Ok(new { message = "Upload successful, OCR job queued." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during document upload");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}