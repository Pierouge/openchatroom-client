public static class LoginResponseModel
{
    public enum loginResponse
    {
        noResponse,
        wrongCredentials,
        emptyFields,
        notMatchingPasswords,
        privateKeyError,
        alreadyExistingUser,
        unknownError
    }

    public static readonly Dictionary<loginResponse, string> responseMessageDict =
        new Dictionary<loginResponse, string>{
                {loginResponse.noResponse, string.Empty},
                {loginResponse.wrongCredentials, "Error: Wrong credentials"},
                {loginResponse.emptyFields, "Error: Some fields are empty"},
                {loginResponse.notMatchingPasswords, "Error: the passwords do not match"},
                {loginResponse.privateKeyError, "Error while generating the private key"},
                {loginResponse.alreadyExistingUser, "Error: this user already exists"},
                {loginResponse.unknownError, "An unknown error occured"}
            };

    public static string getMessageFromResponse(loginResponse response)
    {
        return responseMessageDict.GetValueOrDefault(response, string.Empty);
    }
}