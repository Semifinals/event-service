namespace Microservice.Models.Progressions;

/// <summary>
/// Interface representing something that can be used as a progression
/// </summary>
public interface IProgression
{
  /// <summary>
  /// An array of progressions which match where teams progress to based on
  /// their standings in a set.
  /// </summary>
  public IProgression[] Progressions { get; }
}