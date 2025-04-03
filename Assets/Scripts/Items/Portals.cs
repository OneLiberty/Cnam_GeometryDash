public class Portal : InteractiveObject
{
    public PlayerController.GameMode targetMode;

    public override void Interact(PlayerController player)
    {
        if (player == null) return;
        if (player.currentGameMode == targetMode) return;  // prevent excessive mode change when triggering the collider

        player.ChangeGameMode(targetMode);
    }
    // I could add visual and audio effects here
}
 