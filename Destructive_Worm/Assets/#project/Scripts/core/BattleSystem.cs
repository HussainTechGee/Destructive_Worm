﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Fusion;
public enum BattleState { START, PLAYERTURN, OTHERTURN	, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;

	public GameObject LocalPlayer;
	public Transform[] playerBattleStation;

	[HideInInspector]
	public bool isCameraFollow;

	public static BattleSystem instance;

	public BattleState state;
	public int currentTurnId;
	List<playerData> AllPlayers; 
    private void Awake()
    {
        if(instance==null)
        {
			instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
		currentTurnId = 1;
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		yield return new WaitForSeconds(1f);
		LocalPlayer = FusionConnection.instance.CreatePlayerOnGameScene();
		LocalPlayer.GetComponent<Unit>().SetNameAndHp(GameManager.instance.PlayerName,GameManager.instance.playerHP, LocalPlayer.GetComponent<PlayerController>().PlayerId);
		yield return new WaitForSeconds(1f);
		GetAllPlayer();
		yield return new WaitForSeconds(.5f);
		TurnSetup();
	}
	void TurnSetup()
    {
		playerData pData = CurrentTurnPlayerData();
		if(pData==null)
        {
			Debug.Log("Something is Wrong!");
			return;
        }
		if (isMyTurn())
		{
			state = BattleState.PLAYERTURN;
			isCameraFollow = true;
			pData.pObject.GetComponent<PlayerController>().isFired = false;
			GameplayUI.instance.PlayerTurnOn(pData.pName,pData.playerId);
		}
		else
		{
			//Bot Turn
			if(pData.isBot)
            {
				BotManager.instance.BotTurn(pData.pObject,AllPlayers[GetRandomExcluding()].pObject);
            }
			//Other Player Turn
			state = BattleState.OTHERTURN;
			isCameraFollow = false;
			GameplayUI.instance.OtherTurnOn(pData.pName,pData.playerId);


		}
	}
	bool isMyTurn()
    {
		if(currentTurnId==LocalPlayer.GetComponent<PlayerController>().PlayerId)
        {
			return true;
        }
		else
        {
			return false;
        }
    }

	public void NextTurn(float delay)
    {
		StartCoroutine(NextTurnCoroutine(delay));
    }
	IEnumerator NextTurnCoroutine(float delay)
    {
		yield return new WaitForSeconds(delay);
		for (int i = 0; i < AllPlayers.Count; i++)
		{
			if (AllPlayers[i].playerId == currentTurnId)
			{
				if (i < AllPlayers.Count - 1)
				{
					currentTurnId = AllPlayers[(i + 1)].playerId;
					Debug.Log("NextIndex !");
				}
				else
				{
					Debug.Log("First Index !");
					currentTurnId = AllPlayers[0].playerId;
				}
				break;
			}
		}
		TurnSetup();

	}
	playerData CurrentTurnPlayerData()
    {
		foreach(playerData pData in AllPlayers)
        {
			if(currentTurnId==pData.playerId)
            {
				return pData;
            }
        }
		return null;
    }
	int CurrentTurnPlayerIndex()
	{
		for (int i=0;i<AllPlayers.Count;i++)
		{
			if (currentTurnId ==AllPlayers[i].playerId)
			{
				return i;
			}
		}
		return 0;
	}
	void GetAllPlayer()
    {
		AllPlayers = new List<playerData>();
		PlayerController[] pObjList = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
		BotController[] bObjList = FindObjectsByType<BotController>(FindObjectsSortMode.None);
		Debug.Log("GetAllPlayer Called");
		for(int i=0;i<pObjList.Length;i++)
        {
			PlayerController pObj = Array.Find(pObjList, item => item.PlayerId == (i+1));
			Debug.Log("GetAllPlayer Loop");
			playerData pData = new playerData();
			pData.pObject = pObj.gameObject;
			pData.isBot = false;
			pData.playerId = pObj.PlayerId;
			pData.pName= pObj.GetComponent<Unit>().unitName.ToString();
			AllPlayers.Add(pData);
		}
		int index = pObjList.Length;
		for (int i = 1; i <= bObjList.Length; i++)
		{
			BotController bObj = Array.Find(bObjList, item => item.PlayerId == (index + i));
			playerData pData = new playerData();
			pData.pObject = bObj.gameObject;
			pData.isBot = true;
			pData.playerId = bObj.PlayerId;
			pData.pName = bObj.GetComponent<Unit>().unitName.ToString();
			AllPlayers.Add(pData);
		}
	}
	int GetRandomExcluding()
	{
		
		int randomValue = UnityEngine.Random.Range(0, AllPlayers.Count);  // Generate a random number in the range [min, max)
		int currentPlayerIndex = CurrentTurnPlayerIndex();
		// If the random value is the excluded value, regenerate it
		if (randomValue == currentPlayerIndex)
		{
			// If randomValue is equal to excludedValue, we need to choose from one less value
			randomValue = (randomValue == 0) ? randomValue + 1 : randomValue - 1;
		}

		return randomValue;
	}
	void EndBattle()
	{
		if(state == BattleState.WON)
		{
		//	dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST)
		{
			//dialogueText.text = "You were defeated.";
		}
	}


	//IEnumerator PlayerHeal()
	//{
	//	playerUnit.Heal(5);

	//	playerHUD.SetHP(playerUnit.currentHP);
	//	dialogueText.text = "You feel renewed strength!";

	//	yield return new WaitForSeconds(2f);

	//	state = BattleState.ENEMYTURN;
	//	StartCoroutine(EnemyTurn());
	//}

	public void SkipEnemyTurn()
	{
		state = BattleState.PLAYERTURN;
		GameplayUI.instance.PlayerTurnOn(CurrentTurnPlayerData().pName,CurrentTurnPlayerData().playerId);
		isCameraFollow = true;
	}

	[System.Serializable]
	public class playerData
    {
		public GameObject pObject;
		public int playerId;
		public bool isBot;
		public string pName;
    }
}
