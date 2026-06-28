using System.ComponentModel.DataAnnotations;

public class Channel
{
  [StringLength(32)]
  public string? Id { get; set; }

  [StringLength(64)]
  public string Name { get; set; } = null!;

  [StringLength(32)]
  public string? ServerId { get; set; }

  public DateTime LastMessage { get; set; } = DateTime.Now;
}

