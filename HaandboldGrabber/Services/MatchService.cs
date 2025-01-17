using System.Globalization;
using HtmlAgilityPack;

namespace HaandboldGrabber.Services;

public class MatchService : IMatchService
{
    private readonly HttpClient _httpClient;
    private readonly HaandboldGrabberOptions _options;

    public MatchService(IHttpClientFactory clientFactory, IOptions<HaandboldGrabberOptions> options)
    {
        _httpClient = clientFactory.CreateClient("HaandboldGrabber");
        _options = options.Value;
    }

    public async Task<List<Match>> GetMatches()
    {
        var response = await _httpClient.GetAsync(_options.Url);
        var matches = new List<Match>();

        if (response.IsSuccessStatusCode)
        {
            var htmlDocument = new HtmlDocument();
            var htmlstring = await response.Content.ReadAsStringAsync();
            htmlDocument.LoadHtml(htmlstring);

            var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='main-grid hidden rounded py-5 md:grid']");

            if (nodes != null)
            {
                var now = DateTime.Now;
                var cultureinfo = new CultureInfo("da-DK");

                foreach (var node in nodes)
                {
                    var match = new Match();

                    // Extracting date and time based on their order
                    var dateNode = node.SelectSingleNode("./p[1]");
                    var timeNode = node.SelectSingleNode("./p[2]");
                    var dateStr = dateNode?.InnerText.Trim();
                    var timeStr = timeNode?.InnerText.Replace("kl.", "").Trim();
                    var fullDateTimeStr = $"{dateStr} {timeStr}";

                    // Extract the date and time part from the string
                    var datePart = fullDateTimeStr.Substring(fullDateTimeStr.IndexOf(' ') + 1);

                    // Convert to DateTime
                    var dateTime = DateTime.ParseExact(datePart, "d. MMM HH:mm", cultureinfo);

                    // Set the year to the current year
                    dateTime = dateTime.AddYears(now.Year - dateTime.Year);

                    // If the date is in the past, add one year to make it future
                    if (dateTime < now) dateTime = dateTime.AddYears(1);

                    match.GameTime = dateTime;

                    // Extracting teams based on their order
                    var teamsNode = node.SelectSingleNode("./div[1]//p[1]");
                    var teams = teamsNode?.InnerText.Split(" - ");
                    if (teams is {Length: 2})
                    {
                        match.Team1 = teams[0].Trim();
                        match.Team2 = teams[1].Trim();
                    }

                    // Extracting league based on its order
                    var leagueNode = node.SelectSingleNode("./div[1]//p[2]");
                    match.League = leagueNode?.InnerText.Trim();

                    var imageNode = node.SelectSingleNode("./div[2]//img");
                    var imagestring = imageNode?.Attributes["src"].Value;
                    match.TvChannel = GetTvChannelFromImage(imagestring);


                    matches.Add(match);
                }
            }
        }


        return matches;
    }

    private string GetTvChannelFromImage(string imagestring)
    {
        var last19= imagestring.Substring(imagestring.Length - 19);

        return last19 switch
        {
            "QAAAABJRU5ErkJggg==" => "TV2 Sport",
            "DYAAAAASUVORK5CYII=" => "TV2",
            "qgAAAAASUVORK5CYII=" => "TV2 Play",
            "DUAAAAASUVORK5CYII=" => "DR2",
            "wAAAABJRU5ErkJggg==" => "DR1",
            "KKACiiigAooooA//9k=" => "DR TV",
            _ => "Ukendt"
        };
    }
}