namespace IR
{
  using System;

  public static class ExceptionHelper
  {
    public static bool Try<T>(Action action)
    {
      return Try(delegate { action(); return 0; });
    }
    public static bool Try<T>(Func<T> action)
    {
      try
      {
        action?.Invoke();
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}