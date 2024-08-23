using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;

	public GameObject LocalPlayer;
	public Transform[] playerBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	//public Text dialogueText;

	 BattleHUD playerHUD;
	 BattleHUD enemyHUD;

	[HideInInspector]
	public bool isCameraFollow;

	public static BattleSystem instance;

	public BattleState state;

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
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		yield return new WaitForSeconds(1f);
		GameObject playerGO = FusionConnection.instance.CreatePlayerOnGameScene();
		playerUnit = playerGO.GetComponent<Unit>();

		playerUnit.currentHP = GameManager.instance.playerHP;
		playerUnit.unitName = GameManager.instance.PlayerName;
		playerHUD = playerGO.GetComponentInChildren<BattleHUD>();

		//GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		//enemyUnit = enemyGO.GetComponent<Unit>();
		//enemyHUD = enemyGO.GetComponentInChildren<BattleHUD>();
		//dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

	//	playerHUD.SetHUD(playerUnit);
	//	enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		isCameraFollow = true;
		GameplayUI.instance.PlayerTurnOn();
	}
	public void PlayerBulletHit()
    {
		StartCoroutine(PlayerAttack());
	}
	IEnumerator PlayerAttack()
	{
		//bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		//enemyHUD.SetHP(enemyUnit.currentHP);

		yield return new WaitForSeconds(2f);

		//if(isDead)
		//{
		//	state = BattleState.WON;
		//	EndBattle();
		//} else
		//{
		////	OtherPlayerTurn();
		//}
	}
	public void OtherPlayerTurn()
    {
		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}
	IEnumerator EnemyTurn()
    {
		yield return new WaitForSeconds(2f);
		GameplayUI.instance.OtherTurnOn();
		isCameraFollow = false;
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
		GameplayUI.instance.PlayerTurnOn();
		isCameraFollow = true;
	}

}
