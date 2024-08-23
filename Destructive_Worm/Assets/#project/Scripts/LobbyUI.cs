
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour
{
    public GameObject LobbyPanel, WaitingPanel, LoadingPanel;
    public GameObject SessionEnteryPrefab;
    public Button waitingStartBtn;
    public Transform SessionEntryParentContent;
    public TMP_Text WaitingPanelPlayerNameTxt, WaitingPanelMasterText;
    public static LobbyUI instance;
    private void Awake()
    {
        if(instance==null)
        {
            instance=this;
        }
    }

    public void OpenLobbyPanel()
    {
        WaitingPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }
    public void OpenWaitingPanel()
    {
        if(FusionConnection.instance.isMasterClient())
        {
            waitingStartBtn.interactable = true;
        }
        else
        {
            waitingStartBtn.interactable = false;
        }
        WaitingPanel.SetActive(true);
        LoadingPanel.SetActive(false);
        LobbyPanel.SetActive(false);
    }
    public void OpenLoadingPanel()
    {
        WaitingPanel.SetActive(false);
        LoadingPanel.SetActive(true);
        LobbyPanel.SetActive(false);
    }
    public GameObject SessionListEntryAdd()
    {
        GameObject entryObj = GameObject.Instantiate(SessionEnteryPrefab);
        entryObj.transform.parent = SessionEntryParentContent;
        return entryObj;
    }
    public void WaitingPanelUpdate(int playerIndex)
    {
        WaitingPanelPlayerNameTxt.text = GameManager.instance.PlayerName;
        if(playerIndex==1)
        {
            WaitingPanelMasterText.text = "Yes";
        }
        else
        {
            WaitingPanelMasterText.text = "No";
        }
       
    }
    public void DissconectBtnClick()
    {
        FusionConnection.instance.DissconctPlayer();
    }
    public void CreateNewRoom()
    {
        FusionConnection.instance.CreateRandomSession();

    }
  
    public void GoToGameScene()
    {
        FusionConnection.instance.StartGameScene();
    }
}
