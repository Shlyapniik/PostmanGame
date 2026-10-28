using System;
using UnityEngine;

public enum WorkDayState
{
    Morning,
    Delivering,
    Summary
}

public class DayManager : MonoBehaviour
{
    [SerializeField] private int startingDay = 1;

    private MailBag mailBag;

    public int CurrentDay { get; private set; }
    public WorkDayState CurrentState { get; private set; }

    public int LastDayDeliveredCount { get; private set; }
    public int LastDayRemainingCount { get; private set; }

    public event Action<int> OnDayStarted;
    public event Action<int> OnDayEnded;
    public event Action<WorkDayState> OnStateChanged;

    private void Awake()
    {
        CurrentDay = startingDay;
        CurrentState = WorkDayState.Morning;
    }

    private void Start()
    {
        mailBag =
            FindFirstObjectByType<MailBag>();

        if (mailBag == null)
        {
            Debug.LogError(
                "DayManager: MailBag не найден.");

            return;
        }

        StartDay();
    }

    public void StartDay()
    {
        CurrentState =
            WorkDayState.Morning;

        Debug.Log(
            $"=== День {CurrentDay} начался ===");

        OnStateChanged?.Invoke(
            CurrentState);

        OnDayStarted?.Invoke(
            CurrentDay);
    }

    public void BeginDeliveries()
    {
        if (CurrentState !=
            WorkDayState.Morning ||
            mailBag == null)
        {
            return;
        }

        mailBag.PrepareForDay(
            CurrentDay);

        CurrentState =
            WorkDayState.Delivering;

        Debug.Log(
            $"День {CurrentDay}: почта получена.");

        OnStateChanged?.Invoke(
            CurrentState);
    }

    public void EndDay()
    {
        if (CurrentState !=
            WorkDayState.Delivering ||
            mailBag == null)
        {
            return;
        }

        if (mailBag.HasUndeliveredMail)
        {
            Debug.Log(
                "Нельзя закончить день: осталась почта.");

            return;
        }

        LastDayDeliveredCount =
            mailBag.DeliveredTodayCount;

        LastDayRemainingCount =
            mailBag.Mail.Count;

        CurrentState =
            WorkDayState.Summary;

        Debug.Log(
            $"=== День {CurrentDay} завершён ===");

        OnDayEnded?.Invoke(
            CurrentDay);

        OnStateChanged?.Invoke(
            CurrentState);
    }

    public void StartNextDay()
    {
        if (CurrentState !=
            WorkDayState.Summary)
        {
            return;
        }

        CurrentDay++;

        StartDay();
    }

    public void LoadState(
        int day,
        WorkDayState state,
        int lastDayDeliveredCount,
        int lastDayRemainingCount)
    {
        CurrentDay =
            Mathf.Max(1, day);

        CurrentState =
            state;

        LastDayDeliveredCount =
            Mathf.Max(
                0,
                lastDayDeliveredCount);

        LastDayRemainingCount =
            Mathf.Max(
                0,
                lastDayRemainingCount);

        OnStateChanged?.Invoke(
            CurrentState);
    }
}