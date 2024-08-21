using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameplayUI : MonoBehaviour
{
    public GameObject GameplayPlayerTurn, GameplayOtherTurn;
    public TMP_Text PlayerTurnTimerTxt, OtherTurnTimerText;
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
    public void PlayerTurnOn()
    {
        GameplayPlayerTurn.SetActive(true);
        GameplayOtherTurn.SetActive(false);
        if(_TimerCoroutine!=null)
        {
            StopCoroutine(_TimerCoroutine);
        }
        _TimerCoroutine=StartCoroutine(TimerCoroutine(true));
    }
    public void OtherTurnOn()
    {
        GameplayPlayerTurn.SetActive(false);
        GameplayOtherTurn.SetActive(true);
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
        if (totalTime < 0 && isPlayer)
        {
            BattleSystem.instance.OtherPlayerTurn();
        }
    }
    public void DisconectClick()
    {
        FusionConnection.instance.DissconctPlayer();
    }
}
