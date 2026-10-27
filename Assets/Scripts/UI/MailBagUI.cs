using TMPro;
using UnityEngine;

public class MailBagUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject mailBagPanel;
    [SerializeField] private TMP_Text mailListText;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    private MailBag mailBag;
    private GameInputManager inputManager;

    private void Start()
    {
        mailBag =
            FindFirstObjectByType<MailBag>();

        inputManager =
            FindFirstObjectByType<GameInputManager>();

        if (mailBag == null)
        {
            Debug.LogError(
                "MailBagUI: MailBag не найден.");

            return;
        }

        if (inputManager == null)
        {
            Debug.LogError(
                "MailBagUI: GameInputManager не найден.");
        }

        mailBag.OnMailChanged +=
            UpdateMailList;

        mailBagPanel.SetActive(false);

        UpdateMailList();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(toggleKey))
            return;

        if (mailBagPanel.activeSelf)
        {
            CloseMailBag();
            return;
        }

        if (inputManager != null &&
            inputManager.IsPlayerInputBlocked)
        {
            return;
        }

        OpenMailBag();
    }

    private void OpenMailBag()
    {
        mailBagPanel.SetActive(true);

        inputManager?.AddBlocker(
            InputBlocker.MailBag);

        UpdateMailList();
    }

    private void CloseMailBag()
    {
        mailBagPanel.SetActive(false);

        inputManager?.RemoveBlocker(
            InputBlocker.MailBag);
    }

    private void UpdateMailList()
    {
        if (mailBag == null ||
            mailListText == null)
        {
            return;
        }

        if (mailBag.Mail.Count == 0)
        {
            mailListText.text =
                "ПОЧТОВАЯ СУМКА\n\nПисем нет";

            return;
        }

        mailListText.text =
            "ПОЧТОВАЯ СУМКА\n\n";

        foreach (MailItem item in mailBag.Mail)
        {
            string typeText =
                item.mailType switch
                {
                    MailType.RegisteredLetter =>
                        "Заказное письмо",

                    _ => "Письмо"
                };

            mailListText.text +=
                $"{item.recipientName}\n" +
                $"{item.address}\n" +
                $"{typeText}\n\n";
        }
    }

    private void OnDestroy()
    {
        if (mailBag != null)
        {
            mailBag.OnMailChanged -=
                UpdateMailList;
        }

        inputManager?.RemoveBlocker(
            InputBlocker.MailBag);
    }
}