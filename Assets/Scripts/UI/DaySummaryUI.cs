using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DaySummaryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text dayNumberText;
    [SerializeField] private TMP_Text deliveredText;
    [SerializeField] private TMP_Text remainingText;
    [SerializeField] private Button nextDayButton;

    private DayManager dayManager;
    private PlayerMovement playerMovement;

    private void Start()
    {
        dayManager = FindFirstObjectByType<DayManager>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (dayManager == null)
        {
            Debug.LogError("DaySummaryUI: DayManager не найден.");
            return;
        }

        dayManager.OnStateChanged += HandleStateChanged;

        nextDayButton.onClick.AddListener(OnNextDayClicked);

        summaryPanel.SetActive(false);
    }

    private void HandleStateChanged(WorkDayState state)
    {
        if (state == WorkDayState.Summary)
        {
            ShowSummary();
        }
        else
        {
            summaryPanel.SetActive(false);
        }
    }

    private void ShowSummary()
    {
        titleText.text = "Рабочий день завершён";
        dayNumberText.text = $"День {dayManager.CurrentDay}";
        deliveredText.text =
            $"Доставлено: {dayManager.LastDayDeliveredCount}";
        remainingText.text =
            $"Осталось: {dayManager.LastDayRemainingCount}";

        summaryPanel.SetActive(true);
    }

    private void OnNextDayClicked()
    {
        dayManager.StartNextDay();
    }

    private void OnDestroy()
    {
        if (dayManager != null)
            dayManager.OnStateChanged -= HandleStateChanged;

        if (nextDayButton != null)
            nextDayButton.onClick.RemoveListener(OnNextDayClicked);
    }
}