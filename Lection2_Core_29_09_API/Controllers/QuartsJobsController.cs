using Lection2_Core_BL;
using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Jobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using System.Reflection;

namespace Lection2_Core_API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuartsJobsController : ControllerBase
{
    private ILogger<QuartsJobsController> _logger;
    private ISchedulerFactory _factory;
    private static IEnumerable<Type> _jobs;

    public QuartsJobsController(
        ISchedulerFactory factory,
        ILogger<QuartsJobsController> logger)
    {
        _factory = factory;
        _logger = logger;
        if(_jobs == null)
        {
            _jobs =
                Assembly.GetAssembly(typeof(SendRegularMessageToClientsJob))
                    .ExportedTypes
                    .Where(x =>
                        x.GetInterfaces().Any(y => y == typeof(IJob)));
        }
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartJob(StartJobRequest request)
    {
        IScheduler scheduler = await _factory.GetScheduler();

        var job = _jobs.FirstOrDefault(x => x.Name.EndsWith(
            request.JobTitle,
            StringComparison.OrdinalIgnoreCase));

        if (job != null)
        {
            await scheduler.ScheduleJob(
               JobBuilder.Create(job)
                   .WithIdentity(request.JobTitle)
                   .Build(),
               TriggerBuilder.Create()
                   .WithIdentity(request.JobTitle)
                   .StartNow()
                   .WithSimpleSchedule(x => x
                       .WithIntervalInSeconds(request.IntervalInSeconds)
                       .WithRepeatCount(request.RepeatCount))
                   .Build());

            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("stop")]
    public async Task StopJob(string jobTitle)
    {
        IScheduler scheduler = await _factory.GetScheduler();
        await scheduler.DeleteJob(new JobKey(jobTitle));
    }
}