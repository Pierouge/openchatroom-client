public class User
{
    public string Id { get; }
    public string Username { get; }
    public string VisibleName { get; }
    public string Verifier { get; }
    public string Salt { get; }
    public bool IsAdmin { get; }

    public User(string username, string visiblename, string verifier, string salt)
    {
        Id = Guid.NewGuid().ToString("N");
        Username = username;
        VisibleName = visiblename;
        Verifier = verifier;
        Salt = salt;
        IsAdmin = false;
    }

    public User(string id, string username, string visiblename, string verifier, string salt, bool isadmin)
    {
        Id = id;
        Username = username;
        VisibleName = visiblename;
        Verifier = verifier;
        Salt = salt;
        IsAdmin = isadmin;
    }
}