namespace HaandboldGrabber.Controllers;

[ApiController]
[Route("[controller]")]
public class HaandboldController : ControllerBase
{
    private readonly IMatchService _matchService;

    public HaandboldController(IMatchService  matchService)
    {
        _matchService = matchService;
    }

    // GET api/calendar/json
    [HttpGet]
    // [ResponseCache(Duration = 43200, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Get()
    {
        var matches = await _matchService.GetMatches();
        var calendar = CalendarConverter.ConvertToCalendar(matches);
        var ical = CalendarConverter.Serialize(calendar);
        return File(Encoding.UTF8.GetBytes(ical), "text/calendar", "calendar.ics");
    }
}