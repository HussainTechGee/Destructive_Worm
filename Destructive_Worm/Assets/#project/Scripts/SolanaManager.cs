//using Solana.Unity.SDK;
//using Solana.Unity.Wallet;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
public class SolanaManager : MonoBehaviour
{
    public GameObject WalletPanel;
    public TMP_Text publicKeyText, balanceText;
    public GameObject NotConnectPanel;

    void Start()
    {
      //  Web3.Instance.LoginWithWalletAdapter();
    }

    public void OpenWalletPanel()
    {
        WalletPanel.SetActive(true);
        if (publicKeyText && balanceText)
        {
            if(PlayerPrefs.GetString("KeyVal","")!="")
            {
                NotConnectPanel.SetActive(false);
                publicKeyText.text = PlayerPrefs.GetString("KeyVal");
                balanceText.text= PlayerPrefs.GetString("Balance");
            }
            else
            {
                NotConnectPanel.SetActive(true);
            }
        }
    }
    public void CloseWalletPanel()
    {
        WalletPanel.SetActive(false);
        if (publicKeyText && balanceText)
        {
            publicKeyText.text = "";
            balanceText.text = "";
        }
    }
    //private void OnEnable()
    //{
    //    Web3.OnLogin += OnLogin;
    //    Web3.OnBalanceChange += OnBalanceChange;
    //}
    //private void OnDisable()
    //{
    //    Web3.OnLogin -= OnLogin;
    //    Web3.OnBalanceChange -= OnBalanceChange;
    //}
    //void OnLogin(Account account)
    //{
    //    PlayerPrefs.SetString("keyVal", account.PublicKey.ToString());
    //}
    //void OnBalanceChange(double amount)
    //{
    //    PlayerPrefs.SetString("Balance", amount.ToString(CultureInfo.InvariantCulture));
    //}
}
