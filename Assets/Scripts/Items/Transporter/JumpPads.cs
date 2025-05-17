public class JumpPad : Transporter
{
    protected override void Awake()
    {
        transporterType = TransporterType.Pad; // Set the correct type
        base.Awake();
    }

    public override void Interact(PlayerController player)
    {
        ApplyJumpForce(player);
    }
}