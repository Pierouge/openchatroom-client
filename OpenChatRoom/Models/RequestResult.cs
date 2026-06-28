public class RequestResult
{
  public bool IsSuccess { get; }
  public string? ErrorMessage { get; }
  public ApiResult? ApiResult { get; }

  protected RequestResult(bool success, string? errorMessage, ApiResult? apiResult)
  {
    IsSuccess = success;
    ErrorMessage = errorMessage;
    ApiResult = apiResult;
  }

  public static RequestResult Success(ApiResult? apiResult)
      => new(true, null, apiResult);

  public static RequestResult Failure(string error, ApiResult? apiResult)
      => new(false, error, apiResult);

  public async static Task<RequestResult> FromApiResult(ApiResult result)
  {
    if (!result.IsSuccess) return Failure(result.Exception!.Message, result);

    if (result.Response!.IsSuccessStatusCode) return Success(result);
    return Failure(await result.Response!.Content.ReadAsStringAsync()!, result);
  }
}


public class RequestResult<T> : RequestResult
{
  public T? Data { get; }

  private RequestResult(bool success, T? data, string? errorMessage, ApiResult? apiResult)
      : base(success, errorMessage, apiResult)
  {
    Data = data;
  }

  public static RequestResult<T> Success(T data, ApiResult? apiResult)
      => new(true, data, null, apiResult);

  public static new RequestResult<T> Failure(string error, ApiResult? apiResult)
      => new(false, default, error, apiResult);

  public static RequestResult<T> FromFailedRequestResult(RequestResult requestResult)
  {
    if (requestResult.IsSuccess)
      throw new ArgumentException("Given RequestResult is Successful");
    return Failure(requestResult.ErrorMessage!, requestResult.ApiResult);
  }
}

