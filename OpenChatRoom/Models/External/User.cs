using System.ComponentModel.DataAnnotations;

public class User(string username, string visibleName, string salt, string verifier)
{
  [StringLength(32)]
  public string? Id { get; set; }

  [StringLength(32)]
  [NotNullOrWhiteSpace]
  [UsernameFormat]
  public string Username { get; set; } = username;

  [StringLength(64)]
  [NotNullOrWhiteSpace]
  public string VisibleName { get; set; } = visibleName;

  [StringLength(512)]
  [NotNullOrWhiteSpace]
  public string Verifier { get; set; } = verifier;

  [StringLength(512)]
  [NotNullOrWhiteSpace]
  public string Salt { get; set; } = salt;

  public bool IsAdmin { get; set; } = false;


  // Public info about User
  public record UserInfo(

      [StringLength(32)]
      string? Id,

      [StringLength(32)]
      [NotNullOrWhiteSpace]
      [UsernameFormat]
      string Username,

      [NotNullOrWhiteSpace]
      [StringLength(64)]
      string VisibleName,
      bool IsAdmin);

  public UserInfo GetInfo()
  {
    return new(Id, Username, VisibleName, IsAdmin);
  }
}
