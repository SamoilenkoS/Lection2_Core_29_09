using Lection2_Core_BL;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace Lection2_Core_API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuartsJobsController : ControllerBase
{
    private ILogger<QuartsJobsController> _logger;
    private ISchedulerFactory _factory;

    public QuartsJobsController(
        ISchedulerFactory factory,
        ILogger<QuartsJobsController> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task StartJob()
    {
        IScheduler scheduler = await _factory.GetScheduler();
        await scheduler.ScheduleJob(
            JobBuilder.Create<SendRegularMessageToClientsJob>()
                .WithIdentity(nameof(SendRegularMessageToClientsJob))
                .Build(),
            TriggerBuilder.Create()
                .WithIdentity(nameof(SendRegularMessageToClientsJob))
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(5)
                    .WithRepeatCount(10))
                .Build());
    }

    [HttpPost("stop")]
    public async Task StopJob()
    {
        
    }

}