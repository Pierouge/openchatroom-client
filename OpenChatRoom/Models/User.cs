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
        this.Id = Guid.NewGuid().ToString("N");
        this.Username = username;
        this.VisibleName = visiblename;
        this.Verifier = verifier;
        this.Salt = salt;
        this.IsAdmin = false;
    }

    public User(string id, string username, string visiblename, string verifier, string salt, bool isadmin)
    {
        this.Id = id;
        this.Username = username;
        this.VisibleName = visiblename;
        this.Verifier = verifier;
        this.Salt = salt;
        this.IsAdmin = isadmin;
    }
}