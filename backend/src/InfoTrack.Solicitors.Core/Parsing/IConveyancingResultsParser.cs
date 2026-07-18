using InfoTrack.Solicitors.Core.Models;

namespace InfoTrack.Solicitors.Core.Parsing;

/// <summary>
/// Parses a solicitors.com conveyancing results page into structured listings.
/// </summary>
public interface IConveyancingResultsParser
{
    IReadOnlyList<SolicitorListing> Parse(string html);
}
