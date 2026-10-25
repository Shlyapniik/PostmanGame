using UnityEngine;

public class PostOffice : MonoBehaviour, IInteractable
{
    private DayManager dayManager;
    private MailBag mailBag;

    private void Start()
    {
        dayManager = FindFirstObjectByType<DayManager>();
        mailBag = FindFirstObjectByType<MailBag>();

        if (dayManager == null)
            Debug.LogError("PostOffice: DayManager не найден.");

        if (mailBag == null)
            Debug.LogError("PostOffice: MailBag не найден.");
    }

    public bool CanInteract()
    {
        return dayManager != null &&
               mailBag != null &&
               dayManager.CurrentState != WorkDayState.Summary;
    }

    public string GetInteractionText()
    {
        if (dayManager == null || mailBag == null)
            return "";

        if (dayManager.CurrentState == WorkDayState.Morning)
            return "[E] Получить почту";

        if (mailBag.HasUndeliveredMail)
            return "[E] Проверить оставшуюся почту";

        return "[E] Завершить рабочий день";
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        if (dayManager.CurrentState == WorkDayState.Morning)
        {
            dayManager.BeginDeliveries();
            return;
        }

        if (mailBag.HasUndeliveredMail)
        {
            Debug.Log($"Осталось доставить: {mailBag.Mail.Count}");
            return;
        }

        dayManager.EndDay();
    }
}