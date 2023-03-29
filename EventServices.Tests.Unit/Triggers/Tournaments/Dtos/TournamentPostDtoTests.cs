namespace Semifinals.Services.Event.Triggers.Tournaments;

[TestClass]
public class TournamentPostDtoTests
{
    [TestMethod]
    public void Validator_AcceptsValid()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "Example Name",
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    public void Validator_FailsNoName()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Validator_FailsShortName()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "E",
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Validator_FailsLongName()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "1234567890123456789012345678901234567890123456789012345678901234567890",
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Validator_FailsNoStartTime()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "Example Name",
            EndTime = DateTime.Now.AddHours(2)
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Validator_FailsStartTimeBeforeNow()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "Example Name",
            StartTime = DateTime.Now.AddHours(-1),
            EndTime = DateTime.Now.AddHours(2)
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Validator_FailsNoEndTime()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "Example Name",
            StartTime = DateTime.UtcNow.AddHours(1),
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Validator_FailsEndTimeBeforeNow()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "Example Name",
            StartTime = DateTime.Now.AddHours(-2),
            EndTime = DateTime.Now.AddHours(-1)
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Validator_FailsEndTimeBeforeStartTime()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "Example Name",
            StartTime = DateTime.Now.AddHours(2),
            EndTime = DateTime.Now.AddHours(1)
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }
}