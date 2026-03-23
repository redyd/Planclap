namespace Planclap.Client.Infrastructures.Dto;

public record SqlScheduledDto(string Slug, long ShowStart, int Duration, string Reservations);
