public class Portal : InteractiveObject
{
    public PlayerController.GameMode targetMode;

    public override void Interact(PlayerController player)
    {
        player.ChangeGameMode(targetMode);
    }
    // I could add visual and audio effects here
}
 