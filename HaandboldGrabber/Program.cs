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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseResponseCaching();

app.UseHttpsRedirection();


app.MapControllers();

app.Run();