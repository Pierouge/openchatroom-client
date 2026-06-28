using System.ComponentModel.DataAnnotations;

public class Server
{
  [StringLength(32)]
  public string? Id { get; set; }

  [StringLength(64)]
  public string Name { get; set; } = null!;

  [StringLength(32)]
  public string? OwnerId { get; set; }
}
