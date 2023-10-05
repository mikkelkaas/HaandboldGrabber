using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace HaandboldGrabber.Helpers;

public static class CalendarConverter
{
    private static CalendarEvent ConvertToCalendarEvent(Match match)
    {
        return new CalendarEvent
        {
            Uid = Guid.NewGuid().ToString(),
            Summary = match.Team1 + " - " + match.Team2,
            Location = match.TvChannel,
            Start = new CalDateTime(match.GameTime, "Europe/Copenhagen"),

            End = new CalDateTime(match.GameTime.AddHours(2), "Europe/Copenhagen"),
            IsAllDay = false
        };
    }

    public static Calendar ConvertToCalendar(IEnumerable<Match> matches)
    {
        var calendar = new Calendar();

        foreach (var item in matches.Select(ConvertToCalendarEvent)) calendar.Events.Add(item);

        return calendar;
    }

    public static string Serialize(Calendar calendar)
    {
        var serializer = new CalendarSerializer(new SerializationContext());

        return serializer.SerializeToString(calendar);
    }
}