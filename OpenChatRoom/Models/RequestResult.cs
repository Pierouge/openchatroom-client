public class RequestResult
{
  public bool IsSuccess { get; }
  public string? ErrorMessage { get; }

  protected RequestResult(bool success, string? errorMessage)
  {
    IsSuccess = success;
    ErrorMessage = errorMessage;
  }

  public static RequestResult Success()
      => new(true, null);

  public static RequestResult Failure(string error)
      => new(false, error);
}


public class RequestResult<T> : RequestResult
{
  public T? Data { get; }

  private RequestResult(bool success, T? data, string? errorMessage)
      : base(success, errorMessage)
  {
    Data = data;
  }

  public static RequestResult<T> Success(T data)
      => new(true, data, null);

  public static new RequestResult<T> Failure(string error)
      => new(false, default, error);
}

