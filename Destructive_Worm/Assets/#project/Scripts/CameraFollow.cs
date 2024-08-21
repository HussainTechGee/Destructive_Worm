using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    GameObject player; // Reference to the player's transform
    public float xMargin = 1.0f; // Margin for the X-axis
    public float smoothTime = 0.3f; // Time for the camera to smooth its movement

    private Vector3 offset;

    void Start()
    {
        OnInit();
    }

    void Update()
    {
        if (BattleSystem.instance.isCameraFollow)
        {
            if(player)
            {
                Vector3 targetPosition = player.transform.position + offset;

                // Check if the player is outside the margin on the X-axis
                if (Mathf.Abs(transform.position.x - targetPosition.x) > xMargin)
                {
                    // Lerp towards the target position
                    transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), Time.deltaTime * smoothTime);

                }
            }
            else
            {
                OnInit();
            }
            
        }
        
    }
    void OnInit()
    {
        player = BattleSystem.instance.LocalPlayer;
        // Calculate the initial offset
        if (player)
        {
            offset = transform.position - player.transform.position;
        }
    }
}
