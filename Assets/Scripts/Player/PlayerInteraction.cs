using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TMP_Text interactionPromptText;

    private readonly List<MonoBehaviour> nearbyInteractables =
        new();

    private IInteractable currentInteractable;
    private DialogueManager dialogueManager;
    private GameInputManager inputManager;

    private void Start()
    {
        dialogueManager =
            FindFirstObjectByType<DialogueManager>();

        inputManager =
            FindFirstObjectByType<GameInputManager>();

        if (dialogueManager == null)
        {
            Debug.LogError(
                "PlayerInteraction: DialogueManager не найден.");
        }

        if (inputManager == null)
        {
            Debug.LogError(
                "PlayerInteraction: GameInputManager не найден.");
        }

        UpdatePrompt();
    }

    private void Update()
    {
        // Диалог обрабатываем отдельно.
        // Dialogue сам является блокировщиком управления,
        // но E всё равно должна переключать его реплики.
        if (dialogueManager != null &&
            dialogueManager.IsDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
                dialogueManager.ContinueDialogue();

            UpdatePrompt();
            return;
        }

        // Если управление заблокировано чем-то другим
        // (журнал, сумка, экран итогов),
        // взаимодействовать с миром нельзя.
        if (inputManager != null &&
            inputManager.IsPlayerInputBlocked)
        {
            currentInteractable = null;
            UpdatePrompt();
            return;
        }

        FindClosestInteractable();

        if (Input.GetKeyDown(KeyCode.E) &&
            currentInteractable != null &&
            currentInteractable.CanInteract())
        {
            currentInteractable.Interact();
            FindClosestInteractable();
        }

        UpdatePrompt();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        MonoBehaviour[] components =
            other.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            if (component is not IInteractable)
                continue;

            if (!nearbyInteractables.Contains(component))
                nearbyInteractables.Add(component);
        }

        FindClosestInteractable();
        UpdatePrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        MonoBehaviour[] components =
            other.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
            nearbyInteractables.Remove(component);

        FindClosestInteractable();
        UpdatePrompt();
    }

    private void FindClosestInteractable()
    {
        currentInteractable = null;
        float closestDistance = float.MaxValue;

        for (int i = nearbyInteractables.Count - 1;
             i >= 0;
             i--)
        {
            MonoBehaviour component =
                nearbyInteractables[i];

            if (component == null)
            {
                nearbyInteractables.RemoveAt(i);
                continue;
            }

            if (component is not IInteractable interactable)
                continue;

            if (!interactable.CanInteract())
                continue;

            float distance =
                Vector2.SqrMagnitude(
                    (Vector2)(
                        component.transform.position -
                        transform.position));

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }
    }

    private void UpdatePrompt()
    {
        if (interactionPrompt == null ||
            interactionPromptText == null)
            return;

        if (dialogueManager != null &&
            dialogueManager.IsDialogueActive)
        {
            interactionPrompt.SetActive(false);
            return;
        }

        if (inputManager != null &&
            inputManager.IsPlayerInputBlocked)
        {
            interactionPrompt.SetActive(false);
            return;
        }

        bool shouldShow =
            currentInteractable != null &&
            currentInteractable.CanInteract();

        interactionPrompt.SetActive(shouldShow);

        if (shouldShow)
        {
            interactionPromptText.text =
                currentInteractable.GetInteractionText();
        }
    }
}