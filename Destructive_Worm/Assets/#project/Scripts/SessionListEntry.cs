using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SessionListEntry : MonoBehaviour
{
    public TMP_Text RoomnameText,PlayerCountText;
    public Button JoinBtn;
    public void Setup(string roomname,string playercount, bool isActive)
    {
        RoomnameText.text = roomname;
        PlayerCountText.text = playercount;
        JoinBtn.interactable=isActive;
    }
    public void JoinBtnClick()
    {
        FusionConnection.instance.JoinSession(RoomnameText.text);
    }
}
