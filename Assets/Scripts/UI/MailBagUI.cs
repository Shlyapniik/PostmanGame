using TMPro;
using UnityEngine;

public class MailBagUI : MonoBehaviour
{
    [SerializeField] private GameObject mailBagPanel;
    [SerializeField] private TMP_Text mailListText;

    private MailBag mailBag;
    private DialogueManager dialogueManager;

    private void Start()
    {
        mailBag = FindFirstObjectByType<MailBag>();
        dialogueManager = FindFirstObjectByType<DialogueManager>();

        if (mailBag == null)
        {
            Debug.LogError("MailBag не найден.");
            return;
        }

        mailBag.OnMailChanged += UpdateMailList;

        UpdateMailList();

        mailBagPanel.SetActive(false);
    }

    private void Update()
    {
        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            if (mailBagPanel.activeSelf)
            {
                mailBagPanel.SetActive(false);
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMailBag();
        }
    }

    private void ToggleMailBag()
    {
        mailBagPanel.SetActive(!mailBagPanel.activeSelf);

        if (mailBagPanel.activeSelf)
        {
            UpdateMailList();
        }
    }

    private void UpdateMailList()
    {
        if (mailBag.Mail.Count == 0)
        {
            mailListText.text = "ПОЧТОВАЯ СУМКА\n\nПисем нет";
            return;
        }

        mailListText.text = "ПОЧТОВАЯ СУМКА\n\n";

        foreach (MailItem item in mailBag.Mail)
        {
            string typeText = item.mailType switch
            {
                MailType.RegisteredLetter => "Заказное письмо",
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
            mailBag.OnMailChanged -= UpdateMailList;
        }
    }
}