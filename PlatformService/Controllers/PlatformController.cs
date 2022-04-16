using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMessageBusClient _messageBusClient;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformController(
            IPlatformRepo platformRepo,
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _commandDataClient = commandDataClient;
            _mapper = mapper;
            _platformRepo = platformRepo;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetAll()
        {
            var platforms = await _platformRepo.GetAllPlatformsAsync();
            var returnValue = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Ok(returnValue);
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<PlatformReadDto>> GetById(int id)
        {
            var platform = await _platformRepo.GetPlatformByIdAsync(id);
            if (platform == null)
            {
                return NotFound();
            }
            var returnValue = _mapper.Map<PlatformReadDto>(platform);
            return Ok(returnValue);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PlatformCreateDto platformCreate)
        {
            var platform = _mapper.Map<Platform>(platformCreate);
            platform = _platformRepo.CreatePlatForm(platform);
            if (await _platformRepo.SaveChangesAsync())
            {
                var platformToReturn = _mapper.Map<PlatformReadDto>(platform);
                try
                {
                    await _commandDataClient.SendPlatformToCommand(platformToReturn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to send the message to command service, the exception is {ex.ToString}");
                }
                try
                {
                    var publishedPlatform = _mapper.Map<PlatformPublishedDto>(platformToReturn);
                    publishedPlatform.Event = "Platform_Published";
                    _messageBusClient.PublishNewPlatform(publishedPlatform);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to send the message to command service, the exception is {ex.ToString}");
                }
                return CreatedAtRoute(nameof(GetById), new { id = platformToReturn.Id }, platformToReturn);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}