using System;
using System.Collections.Generic;
using UnityEngine;

public class MailBag : MonoBehaviour
{
    private readonly List<MailItem> mail =
        new();

    private readonly List<MailItem> deliveredMail =
        new();

    public event Action OnMailChanged;

    public IReadOnlyList<MailItem> Mail => mail;

    public IReadOnlyList<MailItem> DeliveredMail =>
        deliveredMail;

    public bool HasUndeliveredMail =>
        mail.Count > 0;

    public int DeliveredTodayCount
    {
        get;
        private set;
    }

    public void PrepareForDay(int day)
    {
        DeliveredTodayCount = 0;
        mail.Clear();

        switch (day)
        {
            case 1:
                AddMail(
                    new MailItem(
                        "Arthur Miller",
                        "Oak Street 12",
                        MailType.RegisteredLetter));

                AddMail(
                    new MailItem(
                        "Evelyn Parker",
                        "Oak Street 18",
                        MailType.Letter));

                AddMail(
                    new MailItem(
                        "Thomas Reed",
                        "River Road 4",
                        MailType.Letter));
                break;

            default:
                Debug.Log(
                    $"Для дня {day} пока нет новой почты.");
                break;
        }

        OnMailChanged?.Invoke();

        Debug.Log(
            $"Почта для дня {day} подготовлена. Отправлений: {mail.Count}");
    }

    public void AddMail(MailItem item)
    {
        mail.Add(item);

        OnMailChanged?.Invoke();

        Debug.Log(
            $"Получено письмо: {item.recipientName} — {item.address}");
    }

    public bool HasMailFor(string address)
    {
        return mail.Exists(
            item =>
                item.address == address);
    }

    public bool HasMailFor(
        string address,
        MailType mailType)
    {
        return mail.Exists(
            item =>
                item.address == address &&
                item.mailType == mailType);
    }

    public bool WasMailDelivered(string address)
    {
        return deliveredMail.Exists(
            item =>
                item.address == address);
    }

    public bool DeliverMail(string address)
    {
        MailItem item =
            mail.Find(
                mailItem =>
                    mailItem.address == address);

        if (item == null)
            return false;

        mail.Remove(item);
        deliveredMail.Add(item);

        DeliveredTodayCount++;

        OnMailChanged?.Invoke();

        Debug.Log(
            $"Доставлено письмо для {item.recipientName}");

        return true;
    }

    public bool DeliverMail(
        string address,
        MailType mailType)
    {
        MailItem item =
            mail.Find(
                mailItem =>
                    mailItem.address == address &&
                    mailItem.mailType == mailType);

        if (item == null)
            return false;

        mail.Remove(item);
        deliveredMail.Add(item);

        DeliveredTodayCount++;

        OnMailChanged?.Invoke();

        Debug.Log(
            $"Доставлено отправление для {item.recipientName}");

        return true;
    }

    public List<MailItemSaveData> GetMailSaveData()
    {
        List<MailItemSaveData> result =
            new();

        foreach (MailItem item in mail)
        {
            result.Add(
                new MailItemSaveData(
                    item.recipientName,
                    item.address,
                    item.mailType));
        }

        return result;
    }

    public List<MailItemSaveData>
        GetDeliveredMailSaveData()
    {
        List<MailItemSaveData> result =
            new();

        foreach (MailItem item in deliveredMail)
        {
            result.Add(
                new MailItemSaveData(
                    item.recipientName,
                    item.address,
                    item.mailType));
        }

        return result;
    }

    public void LoadSaveData(
        List<MailItemSaveData> savedMail,
        List<MailItemSaveData> savedDeliveredMail,
        int deliveredToday)
    {
        mail.Clear();
        deliveredMail.Clear();

        if (savedMail != null)
        {
            foreach (MailItemSaveData item
                     in savedMail)
            {
                if (item != null)
                    mail.Add(item.ToMailItem());
            }
        }

        if (savedDeliveredMail != null)
        {
            foreach (MailItemSaveData item
                     in savedDeliveredMail)
            {
                if (item != null)
                {
                    deliveredMail.Add(
                        item.ToMailItem());
                }
            }
        }

        DeliveredTodayCount =
            Mathf.Max(0, deliveredToday);

        OnMailChanged?.Invoke();
    }
}