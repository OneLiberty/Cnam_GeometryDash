using UnityEngine;

public class JumpPad : Transporter
{
    private void Awake()
    {
        jumpForcesMap.Add("LowPad", 24f);
        jumpForcesMap.Add("NormalPad", 30f);
        jumpForcesMap.Add("HighPad", 35f);

        if (jumpForcesMap.ContainsKey(transporterType))
        {
            jumpForce = jumpForcesMap[transporterType];
        } 
        else
        {
            Debug.LogError($"JumpPad: transporterType '{transporterType}' not found in jumpForcesMap.");
        }
    }

    public override void Interact(PlayerController player)
    {
        ApplyJumpForce(player);
    }

}