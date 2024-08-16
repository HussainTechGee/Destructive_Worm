using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public TMP_Text nameText;
	public TMP_Text levelText;
	public TMP_Text HpText;
 	public Image hpSlider;

	public void SetHUD(Unit unit)
	{
		nameText.text = unit.unitName;
		levelText.text = unit.unitLevel.ToString();
		hpSlider.fillAmount = unit.currentHP/unit.maxHP;
		HpText.text = unit.currentHP.ToString();
		//hpSlider.value = unit.currentHP;
	}

	public void SetHP(float hp)
	{
		HpText.text = hp.ToString();
		hpSlider.fillAmount = hp/100f;
	}

}
