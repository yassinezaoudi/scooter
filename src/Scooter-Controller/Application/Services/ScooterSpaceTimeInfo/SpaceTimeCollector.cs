using Coravel.Invocable;
using Scooter_Controller.Application.Services.ScooterSpaceTimeInfo.ScooterInfoRequests;
using Scooter_Controller.Database;
using Scooter_Controller.Models;

namespace Scooter_Controller.Application.Services.ScooterSpaceTimeInfo;

public class SpaceTimeCollector : IInvocable
{
    private readonly AppDbContext _context;
    private readonly IScooterInfoRequester _infoRequester;

    public SpaceTimeCollector(AppDbContext context, IScooterInfoRequester infoRequester)
    {
        _context = context;
        _infoRequester = infoRequester;
    }
    
    public async Task Invoke()
    {
        foreach (var scooter in _context.Scooters)
        {
            var spaceTime = _infoRequester.GetScooterInfo(scooter);

            _context.SpaceTimes.Add(spaceTime);
        }

        await _context.SaveChangesAsync();
    }
}