using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
public enum BattleState { START, PLAYERTURN, OTHERTURN	, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;

	public GameObject LocalPlayer;
	public Transform[] playerBattleStation;

	Unit playerUnit;

	//public Text dialogueText;

	 BattleHUD playerHUD;
	 BattleHUD enemyHUD;

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
		playerUnit = LocalPlayer.GetComponent<Unit>();

		playerUnit.currentHP = GameManager.instance.playerHP;
		playerUnit.unitName = GameManager.instance.PlayerName;

		yield return new WaitForSeconds(1f);
		BotCreationCheck();
		GetAllPlayer();
		yield return new WaitForSeconds(.5f);
		TurnSetup();
	}
	void BotCreationCheck()
    {
		//Only one Player
		if(FusionConnection.instance.runnerIstance.SessionInfo.PlayerCount==1)
		{
			BotManager.instance.initBot();
        }
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
			GameplayUI.instance.PlayerTurnOn(pData.pName);
		}
		else
		{
			//Bot Turn
			if(pData.isBot)
            {
				BotManager.instance.BotTurn(AllPlayers[0].pObject);
            }
			//Other Player Turn
			else
            {
				state = BattleState.OTHERTURN;
				isCameraFollow = false;
				GameplayUI.instance.OtherTurnOn(pData.pName);
			}
			
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

	public void NextTurn()
    {
		for(int i=0;i<AllPlayers.Count;i++)
        {
			if(AllPlayers[i].playerId==currentTurnId)
            {
				if(i<AllPlayers.Count-1)
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
	void GetAllPlayer()
    {
		AllPlayers = new List<playerData>();
		GameObject[] pObjList = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] bObjList = GameObject.FindGameObjectsWithTag("Bot");
		Debug.Log("GetAllPlayer Called");
		for(int i=0;i<pObjList.Length;i++)
        {
			Debug.Log("GetAllPlayer Loop");
			playerData pData = new playerData();
			pData.pObject = pObjList[i];
			pData.isBot = false;
			pData.playerId = pObjList[i].GetComponent<PlayerController>().PlayerId;
			pData.pName= pObjList[i].GetComponent<Unit>().unitName.ToString();
			AllPlayers.Add(pData);
		}
		for (int i = 0; i < bObjList.Length; i++)
		{
			playerData pData = new playerData();
			pData.pObject = bObjList[i];
			pData.isBot = true;
			pData.playerId = bObjList[i].GetComponent<BotController>().PlayerId;
			AllPlayers.Add(pData);
		}
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
		GameplayUI.instance.PlayerTurnOn(CurrentTurnPlayerData().pName);
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
