using HaandboldGrabber;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

builder.Services.Configure<HaandboldGrabberOptions>(builder.Configuration.GetSection("HaandboldGrabber"));

HaandboldGrabberOptions haandboldGrabberOptions = new HaandboldGrabberOptions();
builder.Configuration.GetSection("HaandboldGrabber").Bind(haandboldGrabberOptions);

builder.Services.AddScoped<IMatchService, MatchService>();

builder.Services.AddHttpClient("HaandboldGrabber")
    .ConfigureHttpClient(c => c.BaseAddress = haandboldGrabberOptions.BaseAddress);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseResponseCaching();

app.UseHttpsRedirection();

app.MapGet("/haandbold", async (IMatchService matchService) =>
{
    var matches = await matchService.GetMatches();
    var calendar = CalendarConverter.ConvertToCalendar(matches);
    var ical = CalendarConverter.Serialize(calendar);
    return Results.File(Encoding.UTF8.GetBytes(ical), "text/calendar", "calendar.ics");
})
.WithName("GetHaandbold")
.WithOpenApi();

app.Run();
