using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Choices")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TMP_Text[] choiceTexts;

    private DialogueData currentDialogue;
    private Action<string> currentActionHandler;
    private int currentLineIndex;
    private GameInputManager inputManager;

    public bool IsDialogueActive { get; private set; }

    private void Awake()
    {
        inputManager =
            FindFirstObjectByType<GameInputManager>();

        if (inputManager == null)
        {
            Debug.LogError(
                "DialogueManager: GameInputManager не найден.");
        }
    }

    public void StartDialogue(
        DialogueData dialogue,
        Action<string> actionHandler = null)
    {
        if (dialogue == null ||
            dialogue.lines == null ||
            dialogue.lines.Length == 0)
            return;

        currentDialogue = dialogue;
        currentActionHandler = actionHandler;
        currentLineIndex = 0;

        IsDialogueActive = true;
        inputManager?.AddBlocker(InputBlocker.Dialogue);
        dialoguePanel.SetActive(true);

        HideChoices();
        ShowCurrentLine();
    }

    public void ContinueDialogue()
    {
        if (!IsDialogueActive)
            return;

        if (choicePanel.activeSelf)
            return;

        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            ShowChoicesOrEnd();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        speakerNameText.text = currentDialogue.speakerName;
        dialogueText.text = currentDialogue.lines[currentLineIndex];
    }

    private void ShowChoicesOrEnd()
    {
        if (currentDialogue.choices != null &&
            currentDialogue.choices.Length > 0)
        {
            ShowChoices();
            return;
        }

        EndDialogue();
    }

    private void ShowChoices()
    {
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentDialogue.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = currentDialogue.choices[i].text;

                int index = i;

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(
                    () => SelectChoice(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SelectChoice(int index)
    {
        if (currentDialogue == null ||
            currentDialogue.choices == null ||
            index < 0 ||
            index >= currentDialogue.choices.Length)
            return;

        DialogueChoice choice = currentDialogue.choices[index];

        if (!string.IsNullOrWhiteSpace(choice.actionId) ||
            (choice.additionalActionIds != null &&
             choice.additionalActionIds.Length > 0))
        {
            Action<string> handler = currentActionHandler;
            string actionId = choice.actionId;
            string[] additionalActions = choice.additionalActionIds;

            EndDialogue();

            handler?.Invoke(actionId);

            if (additionalActions != null)
            {
                foreach (string additionalAction in additionalActions)
                {
                    if (!string.IsNullOrWhiteSpace(additionalAction))
                        handler?.Invoke(additionalAction);
                }
            }

            return;
        }

        HideChoices();

        if (choice.nextLineIndex < 0 ||
            choice.nextLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        currentLineIndex = choice.nextLineIndex;
        ShowCurrentLine();
    }

    private void HideChoices()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    private void EndDialogue()
    {
        IsDialogueActive = false;
        inputManager?.RemoveBlocker(InputBlocker.Dialogue);

        currentDialogue = null;
        currentActionHandler = null;
        currentLineIndex = 0;

        HideChoices();
        dialoguePanel.SetActive(false);
    }
}