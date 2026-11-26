namespace Interaction
{
    /// <summary>
    /// Anything the player can look at and press Interact on.
    /// The Interactor handles focus, outline and prompt.
    /// </summary>
    public interface IInteractable
    {
        string Prompt { get; } // e.g. "Pick up Crate"
        bool CanInteract(Player.Interactor interactor);
        void Interact(Player.Interactor interactor);
    }
}