using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Unit : NetworkBehaviour
{
	[Networked, OnChangedRender(nameof(NameChanged))]
	public NetworkString<_32> unitName { get; set; }

	[Networked, OnChangedRender(nameof(IdChanged))]
	public int unitLevel { get; set; }

	public float damage;

	public float maxHP;
	[Networked, OnChangedRender(nameof(HealthChanged))]
	public float currentHP { get; set; }=100;

	public BattleHUD playerHuDScript;
	void HealthChanged()
    {
		Debug.Log("Hp Changed!"+currentHP);
		//playerHuDScript.SetHP(currentHP);
		//change


	}
	public void SetNameAndHp(string name,float hp,int id)
    {
		unitName = name;
		currentHP = hp;
		unitLevel = id;
    }
	void NameChanged()
	{
		Debug.Log("Hp name!"+unitName);
		//playerHuDScript.SetName(unitName);
	}
	void IdChanged()
	{
		Debug.Log("Hp name!" + unitName);
		//playerHuDScript.SetName(unitName);
	}
	[Rpc (RpcSources.StateAuthority,RpcTargets.All)]
	public void RPC_TakeDamage(float dmg)
	{

		currentHP -= dmg;

		if (currentHP <= 0)
		{//	return true;
		}
		else
		{   //return false;
		}
	}

	public void TakeDamage(float dmg)
	{

		currentHP -= dmg;

		if (currentHP <= 0)
		{//	return true;
		}
		else
		{   //return false;
		}
	}
	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

}
