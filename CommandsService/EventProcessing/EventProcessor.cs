using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            System.Console.WriteLine("*** Determining the event type");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType?.Event)
            {
                case "Platform_Published":
                    System.Console.WriteLine("***Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    System.Console.WriteLine("***cound not determine the event");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChangesAsync();
                        System.Console.WriteLine("*** platform added");
                    }
                    else
                    {
                        System.Console.WriteLine("*** Not able to add platform to storage");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"***coudn't add the platform to the DB: {ex.Message}");
                }
            }
        }
    }
}