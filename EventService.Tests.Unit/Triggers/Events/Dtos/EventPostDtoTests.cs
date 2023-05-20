namespace Semifinals.Services.EventService.Triggers.Events;

[TestClass]
public class EventPostDtoTests
{
    [TestMethod]
    public void Validator_AcceptsValid()
    {
        // Arrange
        EventPostDto dto = new()
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
        EventPostDto dto = new()
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
        EventPostDto dto = new()
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
        EventPostDto dto = new()
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
        EventPostDto dto = new()
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
        EventPostDto dto = new()
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
        EventPostDto dto = new()
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
        EventPostDto dto = new()
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
        EventPostDto dto = new()
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