using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;
public class BattleHUD : MonoBehaviour
{

	public TMP_Text nameText;
	public TMP_Text levelText;
	public TMP_Text HpText;
 	public Image hpSlider;
	float previousHp=0;
	public Unit unitScript;

	public void SetHUD(Unit unit)
	{
		nameText.text = unit.unitName.ToString();
		levelText.text = unit.unitLevel.ToString();
		hpSlider.fillAmount = unit.currentHP/unit.maxHP;
		HpText.text = unit.currentHP.ToString();
		//hpSlider.value = unit.currentHP;
	}

	public void SetHP(float hp)
	{
		Debug.Log("HP:" + hp);
		HpText.text = hp.ToString();
		hpSlider.fillAmount = hp/100f;
	}
	public void SetName(string name)
	{
		Debug.Log(name);
		nameText.text = name;
	}
    private void Update()
    {
		if(previousHp!=unitScript.currentHP)
        {
			previousHp = unitScript.currentHP;
			HpText.text = previousHp.ToString();
			nameText.text = unitScript.unitName.ToString();
			hpSlider.fillAmount = previousHp / 100f;
		}
		
	}
}
