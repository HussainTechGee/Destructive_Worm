using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToastScript : MonoBehaviour
{
    public float toastLife;
    public TMP_Text ToastText;
    GameObject ToastObj;
    public static ToastScript instance;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    void Start()
    {
        ToastObj = transform.GetChild(0).gameObject;
    }

    public void ToastShow(string msg)
    {
        ToastText.text = msg;
        ToastObj.SetActive(true);
        StartCoroutine(ToastHide());
    }
    IEnumerator ToastHide()
    {
        yield return new WaitForSeconds(toastLife);
        ToastObj.SetActive(false);
    }
}
