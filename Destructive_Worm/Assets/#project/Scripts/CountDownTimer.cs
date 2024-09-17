using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using System.Linq;
using Fusion.Sockets;
using System;

public class CountDownTimer : NetworkBehaviour
{ 
    [SerializeField] float totalTime;

    [HideInInspector]
    [Networked, OnChangedRender(nameof(OnTimerChanged))]
    public float CountdownTime { get; set; } // The networked countdown timer
    
    bool isTimerRunning = false;
    bool isMaster = false;
    public override void Spawned()
    {
        if (Object.HasStateAuthority) // Only the host client initializes the timer
        {
            StartCountdown(totalTime);
        }
    }
    private void StartCountdown(float time)
    {
        isMaster = FusionConnection.instance.runnerIstance.IsSharedModeMasterClient;
        CountdownTime = time;
        isTimerRunning = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (isTimerRunning) // Only the host updates the timer
        {
            if (CountdownTime > 0)
            {
                CountdownTime -= Runner.DeltaTime; // Decrease time
            }
            else
            {
                CountdownTime = 0;
                isTimerRunning = false;
                OnCountdownFinished(); // Trigger any event when countdown finishes
            }
        }
    }
    private void OnTimerChanged()
    {
        UpdateCountdownDisplay();
    }

    private void UpdateCountdownDisplay()
    {
       // if (CountdownText != null)
        {
            LobbyUI.instance.CountdownText.text = Mathf.CeilToInt(CountdownTime).ToString(); // Update UI element to show the current countdown time
        }
    }
    private void OnCountdownFinished()
    {
        // Logic for what happens when the countdown finishes
        Debug.Log("Countdown Finished!");
        LobbyUI.instance.GoToGameScene();
    }

    public void OnPlayerLeft(PlayerRef player)
    {
        Debug.Log("Player Left");
        // Handle reassignment of bot control when the host client disconnects
        if (player == Object.StateAuthority || player == Object.InputAuthority)
        {
            Debug.Log("Player Left in");
            PlayerRef newOwner = Runner.ActivePlayers.FirstOrDefault(p => p != player);
            if (newOwner != Object.StateAuthority && newOwner.IsRealPlayer)
            {
                // Reassign state authority to the first valid player found
                Object.AssignInputAuthority(newOwner);
                Debug.Log($"New bot host assigned: {newOwner}");
                StartCountdown(CountdownTime);
            }
            else
            {
                Debug.Log("Not Exist");
            }
            
        }
        else
        {
            Debug.Log("No");
        }
    }
   
}
