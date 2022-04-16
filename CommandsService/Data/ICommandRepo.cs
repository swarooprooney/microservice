using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepo
    {
        Task<bool> SaveChangesAsync();

        Task<IEnumerable<Platform>> GetAllPlatformsAsync();

        void CreatePlatform(Platform platform);

        bool PlatformExists(int platformId);

        Task<IEnumerable<Command>> GetCommandsForPlatformAsync(int platformId);

        Task<Command> GetCommandAsync(int platformId, int commandId);

        void CreateCommand(int platformId, Command command);
    }
}