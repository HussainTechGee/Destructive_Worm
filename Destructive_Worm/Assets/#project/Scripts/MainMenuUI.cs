using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    public Button InputBtn;
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
            InputBtn.gameObject.SetActive(false);
        }
        else
        {
            InputBtn.interactable = false;
            InputBtn.gameObject.SetActive(true);
        }
    }
    public void NameUpdate()
    {
        string name = nameInpuit.text;
        PlayerPrefs.SetString("playerName", name);
        GameManager.instance.PlayerName= name;
        ToastScript.instance.ToastShow("Name Updated!");
        InputBtn.gameObject.SetActive(false);
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
