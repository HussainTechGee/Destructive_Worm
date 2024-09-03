using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField nameInpuit;
    bool isClicked;
    // Start is called before the first frame update
    void Start()
    {
        string pName = PlayerPrefs.GetString("playerName","");
        if (pName!="")
        {
            nameInpuit.text = pName;
            GameManager.instance.PlayerName = pName;
        }
    }
    public void NameUpdate()
    {
        string name = nameInpuit.text;
        PlayerPrefs.SetString("playerName", name);
        GameManager.instance.PlayerName= name;
    }
    public void StartBtnClick()
    {
        if(!isClicked)
        {
            isClicked = true;
            Invoke("DelayBeforeLoad",.25f);
        }
        
    }
    void DelayBeforeLoad()
    {
        GameManager.instance.GoToScene(1);
    }
}
