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
    private GameInputManager inputManager;

    private void Start()
    {
        dayManager =
            FindFirstObjectByType<DayManager>();

        inputManager =
            FindFirstObjectByType<GameInputManager>();

        if (dayManager == null)
        {
            Debug.LogError(
                "DaySummaryUI: DayManager не найден.");

            return;
        }

        if (inputManager == null)
        {
            Debug.LogError(
                "DaySummaryUI: GameInputManager не найден.");
        }

        dayManager.OnStateChanged +=
            HandleStateChanged;

        if (nextDayButton != null)
        {
            nextDayButton.onClick.AddListener(
                OnNextDayClicked);
        }

        summaryPanel.SetActive(false);
    }

    private void HandleStateChanged(
        WorkDayState state)
    {
        if (state == WorkDayState.Summary)
        {
            ShowSummary();
            return;
        }

        HideSummary();
    }

    private void ShowSummary()
    {
        titleText.text =
            "Рабочий день завершён";

        dayNumberText.text =
            $"День {dayManager.CurrentDay}";

        deliveredText.text =
            $"Доставлено: " +
            $"{dayManager.LastDayDeliveredCount}";

        remainingText.text =
            $"Осталось: " +
            $"{dayManager.LastDayRemainingCount}";

        summaryPanel.SetActive(true);

        inputManager?.AddBlocker(
            InputBlocker.DaySummary);
    }

    private void HideSummary()
    {
        summaryPanel.SetActive(false);

        inputManager?.RemoveBlocker(
            InputBlocker.DaySummary);
    }

    private void OnNextDayClicked()
    {
        HideSummary();

        dayManager.StartNextDay();
    }

    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnStateChanged -=
                HandleStateChanged;
        }

        if (nextDayButton != null)
        {
            nextDayButton.onClick.RemoveListener(
                OnNextDayClicked);
        }

        inputManager?.RemoveBlocker(
            InputBlocker.DaySummary);
    }
}