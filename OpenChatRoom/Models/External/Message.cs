using System.ComponentModel.DataAnnotations;
public class Message
{
  public Message() { }

  public Message(string Text, bool IsModified, string ChannelId)
  {
    this.Text = Text;
    this.IsModified = IsModified;
    this.ChannelId = ChannelId;
  }

  [StringLength(32)]
  public string? Id { get; set; }

  [StringLength(2048)]
  public string Text { get; set; } = null!;

  public DateTime Time { get; set; } = DateTime.Now;

  public bool IsModified { get; set; } = false;

  [StringLength(32)]
  public string AuthorId { get; set; } = null!;

  [StringLength(32)]
  public string ChannelId { get; set; } = null!;

}
