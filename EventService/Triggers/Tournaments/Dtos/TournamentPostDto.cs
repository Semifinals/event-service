namespace Semifinals.Services.Event.Triggers.Tournaments;

public class TournamentPostDto : Dto, IBodyDto
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("startTime")]
    [JsonConverter(typeof(EpochConverter))]
    public DateTime StartTime { get; set; }

    [JsonProperty("endTime")]
    [JsonConverter(typeof(EpochConverter))]
    public DateTime EndTime { get; set; }

    [JsonIgnore]
    public override IDtoValidator Validator { get; } = new DtoValidator<TournamentPostDto>(validator =>
    {
        validator.RuleFor(dto => dto.Name)
            .NotNull()
            .MinimumLength(3)
            .MaximumLength(64);

        validator.RuleFor(dto => dto.StartTime)
            .NotNull()
            .Must(p => p > DateTime.Now);

        validator.RuleFor(p => p.EndTime)
            .NotNull()
            .Must(p => p > DateTime.Now)
            .Must((dto, p) => p > dto.StartTime);
    });

    //public TournamentPostDto(string name, DateTime startTime, DateTime endTime)
    //{
    //    Name = name;
    //    StartTime = startTime;
    //    EndTime = endTime;
    //}
}