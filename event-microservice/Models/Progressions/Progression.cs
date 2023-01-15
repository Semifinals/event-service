using System;

namespace Microservice.Models.Progressions;

/// <summary>
/// Progression class used to implement progression to other classes through
/// composition.
/// </summary>
public class Progression : IProgression
{
  public IProgression[] Progressions { get; set; } = Array.Empty<IProgression>();
}