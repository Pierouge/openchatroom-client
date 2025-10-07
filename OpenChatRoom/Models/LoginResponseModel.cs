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
        wrongUsernameCharacters,
        userNotFound,
        srpError,
        unknownError
    }

    public static readonly Dictionary<loginResponse, string> responseMessageDict =
        new()
        {
                {loginResponse.noResponse, string.Empty},
                {loginResponse.wrongCredentials, "Error: Wrong credentials"},
                {loginResponse.emptyFields, "Error: Some fields are empty"},
                {loginResponse.notMatchingPasswords, "Error: the passwords do not match"},
                {loginResponse.privateKeyError, "Error while generating the private key"},
                {loginResponse.alreadyExistingUser, "Error: this user already exists"},
                {loginResponse.wrongUsernameCharacters,
                "Error: illegal characters detected in the username, please only use alphanumeric characters." },
                {loginResponse.userNotFound, "Error: such user was not found."},
                {loginResponse.srpError, "An error occured in the login procedure."},
                { loginResponse.unknownError, "An unknown error occured"}
            };

    public static string getMessageFromResponse(loginResponse response)
    {
        return responseMessageDict.GetValueOrDefault(response, string.Empty);
    }
}