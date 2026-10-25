using System;

public enum MailType
{
    Letter,
    RegisteredLetter
}

[Serializable]
public class MailItem
{
    public string recipientName;
    public string address;
    public MailType mailType;

    public MailItem(
        string recipientName,
        string address,
        MailType mailType = MailType.Letter)
    {
        this.recipientName = recipientName;
        this.address = address;
        this.mailType = mailType;
    }
}