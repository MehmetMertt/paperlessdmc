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
using Minio;
using Minio.DataModel.Args;
using System.IO;
using System.Text;
using Tesseract;

namespace PaperlessREST.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    public class MetaDataController : Controller
    {
        private readonly IMetaDataService _metaDataService;
        private readonly RabbitMqService _rabbit;
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName = "paperless-data";
        private readonly ILogger<MetaDataController> _logger;
        private readonly string minIoPort = "9000";





        public MetaDataController(IMetaDataService metaDataService, RabbitMqService rabbit, ILogger<MetaDataController> logger, IMinioClient minioClient)
        {
            _metaDataService = metaDataService;
            _rabbit = rabbit;
            _logger = logger;
            _minioClient = minioClient;
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

        /*[HttpPost()]
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
            
        }*/

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file)
        {
            try
            {

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload failed: no file provided or file was empty.");
                    return BadRequest("No file provided.");
                }

                // Read metadata from FormData
                var form = Request.Form;
                var title = form["title"].ToString();
                var summary = form["summary"].ToString();
                var fileType = form["fileType"].ToString();
                var fileSize = double.Parse(form["fileSize"]);
                var createdOn = DateTime.Parse(form["createdOn"]);
                var modifiedLast = DateTime.Parse(form["modifiedLast"]);

                _logger.LogDebug($"{title}\n {fileType}\n {fileSize} \n {createdOn}");

                // Create MetaData entity
                var metaData = new MetaData(
                    id: Guid.NewGuid(),
                    title: title,
                    fileType: fileType,
                    fileSize: fileSize,
                    summary: summary,
                    createdOn: createdOn,
                    modifiedLast: modifiedLast
                );

                metaData.Summary = ("No Summary");

                var command = new CreateMetaDataCommand(

                    metaData.Id,
                    metaData.Title,
                    metaData.FileType,
                    metaData.FileSize,
                    metaData.Summary,
                    metaData.CreatedOn,
                    metaData.ModifiedLast);
                
                // Filename in minio
                var objectName = $"{metaData.Id}_{file.FileName}";
                
                //Ensure bucket exists
                bool found = await _minioClient.BucketExistsAsync(
                    new Minio.DataModel.Args.BucketExistsArgs().WithBucket(_bucketName)
                );

                if (!found)
                    await _minioClient.MakeBucketAsync(
                        new Minio.DataModel.Args.MakeBucketArgs().WithBucket(_bucketName)
                    );

                // Upload file to minio

                using (var stream = file.OpenReadStream())
                {
                    await _minioClient.PutObjectAsync(
                        new Minio.DataModel.Args.PutObjectArgs()
                            .WithBucket(_bucketName)
                            .WithObject(objectName)
                            .WithStreamData(stream)
                            .WithObjectSize(file.Length)
                            .WithContentType(file.ContentType)
                    );
                }                
                
                // Save metadata in DB
                var created = _metaDataService.CreateMetaData(command);


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