public record TokenPair(
    [NotNullOrWhiteSpace]
    string Token,

    [NotNullOrWhiteSpace]
    string RefreshToken);

