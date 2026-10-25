public interface IInteractable
{
    bool CanInteract();
    string GetInteractionText();
    void Interact();
}