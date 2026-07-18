namespace InfoTrack.Solicitors.Api.Contracts;

/// <summary>Optional per-call override of which locations to search; defaults to the saved active list.</summary>
public record RunSearchRequest(string[]? Locations);
