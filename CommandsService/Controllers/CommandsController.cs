using System.Net;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [ApiController]
    [Route("api/{platformId:int}/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _commandRepo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"Getting command for {platformId}");
            if (!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }
            var commands = await _commandRepo.GetCommandsForPlatformAsync(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId:int}", Name = "GetCommandForPlatform")]
        public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"Getting command for platformId : {platformId} and commandId :{commandId}");
            if (!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }
            var command = await _commandRepo.GetCommandAsync(platformId, commandId);
            if (command == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost("{commandId:int}")]
        public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"Creating command for platformId : {platformId}");
            if (!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }
            var command = _mapper.Map<Command>(commandCreateDto);
            _commandRepo.CreateCommand(platformId, command);

            var commandReadDto = _mapper.Map<CommandReadDto>(command);
            if (await _commandRepo.SaveChangesAsync())
            {
                return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.PlatformId }, commandReadDto);
            }
            return StatusCode(500);
        }
    }
}