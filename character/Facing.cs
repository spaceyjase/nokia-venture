using System;

public enum Facing
{
  Right,
  Left,
  Up,
  Down
}

public static class FacingExtensions
{
  public static void Random()
  {
    var values = Enum.GetValues(typeof(Facing)); 
    var random = new Random();
    var randomFacing = (Facing)values.GetValue(random.Next(values.Length));
  }
}