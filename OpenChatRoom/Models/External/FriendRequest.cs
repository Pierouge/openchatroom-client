using System.ComponentModel.DataAnnotations;

public class FriendRequest
{

  public FriendRequest() { }

  public FriendRequest(string authorId, string receiverId)
  {
    AuthorId = authorId;
    ReceiverId = receiverId;
    IsAccepted = false;
  }

  [StringLength(32)]
  public string AuthorId { get; set; } = null!;

  [StringLength(32)]
  public string ReceiverId { get; set; } = null!;

  public bool IsAccepted { get; set; } = false;
}
