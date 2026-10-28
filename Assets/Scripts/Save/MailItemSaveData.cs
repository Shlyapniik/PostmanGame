using System;

[Serializable]
public class MailItemSaveData
{
    public string recipientName;
    public string address;
    public MailType mailType;

    public MailItemSaveData(
        string recipientName,
        string address,
        MailType mailType)
    {
        this.recipientName = recipientName;
        this.address = address;
        this.mailType = mailType;
    }

    public MailItem ToMailItem()
    {
        return new MailItem(
            recipientName,
            address,
            mailType);
    }
}