using UnityEngine;

public class Mailbox : MonoBehaviour, IInteractable
{
    [SerializeField] private string address;

    private MailBag mailBag;

    private void Start()
    {
        mailBag = FindFirstObjectByType<MailBag>();

        if (mailBag == null)
        {
            Debug.LogError("MailBag не найден.");
        }
    }

    public bool CanInteract()
    {
        if (mailBag == null)
            return false;

        return mailBag.HasMailFor(address, MailType.Letter);
    }

    public string GetInteractionText()
    {
        return "[E] Доставить письмо";
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        mailBag.DeliverMail(address, MailType.Letter);
    }
}