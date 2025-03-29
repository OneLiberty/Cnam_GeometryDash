public class DeathZone : InteractiveObject
{
    public override void Interact(PlayerController player)
    {
        if (player == null) return;
        player.Die();
    }
}