using UnityEngine;

public class JumpOrbs : Transporter
{
    private bool playerInRange = false;
    private PlayerController player;

    private void Awake()
    {
        jumpForcesMap.Add("LowOrb", 24f);
        jumpForcesMap.Add("NormalOrb", 30f);
        jumpForcesMap.Add("HighOrb", 35f);

        if (jumpForcesMap.ContainsKey(transporterType))
        {
            jumpForce = jumpForcesMap[transporterType];
        } 
        else
        {
            Debug.LogError($"JumpPad: transporterType '{transporterType}' not found in jumpForcesMap.");
        }
    }

    private void Update()
    {
        if (playerInRange && player != null)
        {
            if (Input.GetKeyDown(GameManager.Instance.inputSettings.jumpButton_0) || Input.GetKeyDown(GameManager.Instance.inputSettings.jumpButton_1))
            {
                ApplyJumpForce(player);
                playerInRange = false; 
                player = null;
            }
        }
    }

    // = OnTriggerEnter2D(Collider2D other) from InteractiveObject
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
        