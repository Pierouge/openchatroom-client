public static class ConnectionResponseModel
{
    // An enum that encapsulates all types of response the overall connection process could have.
    public enum connectionResponse
    {
        noResponse,
        unknownHost,
        noServer,
        connectionSuccess,
        unknownException,
        connectionError,
        invalidAddress
    }

    // Association of the response type and its string error message.
    public static readonly Dictionary<connectionResponse, string> responseMessageDict =
        new Dictionary<connectionResponse, string>{
                {connectionResponse.noResponse, string.Empty},
                {connectionResponse.noServer, "Error: Please type in a server."},
                {connectionResponse.unknownHost, "Error: Impossible to find such server."},
                {connectionResponse.connectionSuccess, "Connection successful!"},
                {connectionResponse.connectionError, "Error: Impossible to establish the connection."},
                {connectionResponse.unknownException, "Error: An unknown error occured."},
                {connectionResponse.invalidAddress, "Error: The address typed is invalid."}
            };

    public static string getMessageFromResponse(connectionResponse response)
    {
        return responseMessageDict.GetValueOrDefault(response, string.Empty);
    }
    
    //The switch statement that translates HTTP error codes to ResponseCodes
    public static connectionResponse httpErrorToResponseCode(HttpRequestError requestError)
    {
        return requestError switch
        {
            HttpRequestError.NameResolutionError => connectionResponse.unknownHost,
            HttpRequestError.ConnectionError => connectionResponse.connectionError,
            _ => connectionResponse.unknownException,
        };
    }
}