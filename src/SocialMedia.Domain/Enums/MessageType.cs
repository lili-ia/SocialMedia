namespace Domain.Enums;

public enum MessageType
{
    Text = 0,
    Image = 1,
    Video = 2, 
    System = 3,        // System-generated message (e.g., "User X joined the chat")
    File = 4
}