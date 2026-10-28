using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField]
    private string saveFileName =
        "save.json";

    private DayManager dayManager;
    private PlayerMovement playerMovement;
    private StoryFlags storyFlags;
    private InventoryManager inventoryManager;
    private QuestManager questManager;
    private MailBag mailBag;

    private string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            saveFileName);

    private void Start()
    {
        dayManager =
            FindFirstObjectByType<DayManager>();

        playerMovement =
            FindFirstObjectByType<PlayerMovement>();

        storyFlags =
            FindFirstObjectByType<StoryFlags>();

        inventoryManager =
            FindFirstObjectByType<InventoryManager>();

        questManager =
            FindFirstObjectByType<QuestManager>();

        mailBag =
            FindFirstObjectByType<MailBag>();

        if (dayManager == null)
        {
            Debug.LogError(
                "SaveManager: DayManager не найден.");
        }

        if (playerMovement == null)
        {
            Debug.LogError(
                "SaveManager: PlayerMovement не найден.");
        }

        if (storyFlags == null)
        {
            Debug.LogError(
                "SaveManager: StoryFlags не найден.");
        }

        if (inventoryManager == null)
        {
            Debug.LogError(
                "SaveManager: InventoryManager не найден.");
        }

        if (questManager == null)
        {
            Debug.LogError(
                "SaveManager: QuestManager не найден.");
        }

        if (mailBag == null)
        {
            Debug.LogError(
                "SaveManager: MailBag не найден.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame();

        if (Input.GetKeyDown(KeyCode.F9))
            LoadGame();
    }

    public void SaveGame()
    {
        if (dayManager == null ||
            playerMovement == null ||
            storyFlags == null ||
            inventoryManager == null ||
            questManager == null ||
            mailBag == null)
        {
            return;
        }

        SaveData data = new();

        data.currentDay =
            dayManager.CurrentDay;

        data.currentDayState =
            dayManager.CurrentState;

        data.deliveredTodayCount =
            mailBag.DeliveredTodayCount;
        
        data.lastDayDeliveredCount =
            dayManager.LastDayDeliveredCount;

        data.lastDayRemainingCount =
            dayManager.LastDayRemainingCount;

        data.SetPlayerPosition(
            playerMovement.transform.position);

        data.storyFlags =
            storyFlags.GetAllFlags();

        data.inventoryItemIds =
            inventoryManager.GetOwnedItemIds();

        data.quests =
            questManager.GetSaveData();

        data.currentMail =
            mailBag.GetMailSaveData();

        data.deliveredMail =
            mailBag.GetDeliveredMailSaveData();

        string json =
            JsonUtility.ToJson(
                data,
                true);

        File.WriteAllText(
            SavePath,
            json);

        Debug.Log(
            $"[Save] Игра сохранена: {SavePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log(
                "[Save] Сохранение не найдено.");

            return;
        }

        string json =
            File.ReadAllText(SavePath);

        SaveData data =
            JsonUtility.FromJson<SaveData>(
                json);

        if (data == null)
        {
            Debug.LogError(
                "[Save] Не удалось прочитать сохранение.");

            return;
        }

        storyFlags.LoadFlags(
            data.storyFlags);

        inventoryManager.LoadItems(
            data.inventoryItemIds);

        questManager.LoadSaveData(
            data.quests);

        mailBag.LoadSaveData(
            data.currentMail,
            data.deliveredMail,
            data.deliveredTodayCount);

        playerMovement.transform.position =
            data.GetPlayerPosition();

        dayManager.LoadState(
            data.currentDay,
            data.currentDayState,
            data.lastDayDeliveredCount,
            data.lastDayRemainingCount);

        Debug.Log(
            "[Save] Игра загружена.");
    }

    public void DeleteSave()
    {
        if (!File.Exists(SavePath))
            return;

        File.Delete(SavePath);

        Debug.Log(
            "[Save] Сохранение удалено.");
    }
}