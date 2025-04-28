using UnityEngine;

public class JumpOrbs : Transporter
{
    private bool playerInRange = false;
    private PlayerController player;

    protected override void Awake()
    {
        transporterType = TransporterType.Orb; // Ensure correct type
        base.Awake();
    }

    private void Update()
    {
        if (playerInRange && player != null)
        {
            if (Input.GetKeyDown(GameManager.Instance.inputSettings.jumpButton_0) || 
                Input.GetKeyDown(GameManager.Instance.inputSettings.jumpButton_1))
            {
                ApplyJumpForce(player);
                playerInRange = false;
                player = null;
            }
        }
    }

    public override void Interact(PlayerController player)
    {
        this.player = player;
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }
}