using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameplayUI : MonoBehaviour
{
    public GameObject GameplayPlayerTurn, GameplayOtherTurn;
    public TMP_Text PlayerTurnTimerTxt, PlayerTurnIdTxt, OtherTurnTimerText, OtherTurnIdText, playerTurnNameText,OtherTurnNameText;
    public int AttackTime;

    Coroutine _TimerCoroutine;

    public static GameplayUI instance;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    public void PlayerTurnOn(string pName, int id)
    {
       // playerTurnNameText.text = pName +" Turn!";
        GameplayPlayerTurn.SetActive(true);
        GameplayOtherTurn.SetActive(false);
        PlayerTurnIdTxt.text = id.ToString();
        if(_TimerCoroutine!=null)
        {
            StopCoroutine(_TimerCoroutine);
        }
        _TimerCoroutine=StartCoroutine(TimerCoroutine(true));
    }
    public void OtherTurnOn(string pName,int id)
    {
        OtherTurnNameText.text = pName+" Turn!";
        GameplayPlayerTurn.SetActive(false);
        GameplayOtherTurn.SetActive(true);
        OtherTurnIdText.text = id.ToString();
        if (_TimerCoroutine != null)
        {
            StopCoroutine(_TimerCoroutine);
        }
        _TimerCoroutine = StartCoroutine(TimerCoroutine(false));
    }
    IEnumerator TimerCoroutine(bool isPlayer)
    {
        int totalTime = AttackTime;
        while(totalTime>=0)
        {
            //For Player Turn
            if(isPlayer)
            {
                PlayerTurnTimerTxt.text = totalTime.ToString();
            }
            //Other Turn
            else
            {
                OtherTurnTimerText.text = totalTime.ToString();
            }
            yield return new WaitForSeconds(1f);
            totalTime--;
        }
        if (totalTime < 0 && FusionConnection.instance.runnerIstance.IsSharedModeMasterClient)
        {
            Debug.Log("Next Turn on Master");
          //  BattleSystem.instance.LocalPlayer.GetComponent<PlayerController>().RPC_NextTurn();
        }
    }
    public void DisconectClick()
    {
        FusionConnection.instance.DissconctPlayer();
    }
}
