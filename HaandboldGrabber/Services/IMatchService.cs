using HaandboldGrabber.Models;

namespace HaandboldGrabber.Services;

public interface IMatchService
{
    Task<List<Match>> GetMatches();
}