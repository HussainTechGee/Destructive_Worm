using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WeaponSelection : MonoBehaviour
{
    public Transform[] AllWeaponObjList;
    public Sprite selectedSprite, unSelectedSprite;
    int currentIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        currentIndex=PlayerPrefs.GetInt("currentWeaponIndex");
        UIRefresh();
        
    }
    void UIRefresh()
    {
        for (int i = 0; i < AllWeaponObjList.Length; i++)
        {
            //Selected
            if(i==currentIndex)
            {
                AllWeaponObjList[i].GetChild(0).GetComponent<Image>().sprite = selectedSprite;
            }
            //unSelected
            else
            {
                AllWeaponObjList[i].GetChild(0).GetComponent<Image>().sprite = unSelectedSprite;
            }
        }

        BattleSystem.instance.currentWeaponIndex = currentIndex;
        BattleSystem.instance.WeaponChange();
    }

    public void WeaponSelectClick(int index)
    {
        currentIndex = index;
        PlayerPrefs.SetInt("currentWeaponIndex", currentIndex);
        UIRefresh();
    }
}
