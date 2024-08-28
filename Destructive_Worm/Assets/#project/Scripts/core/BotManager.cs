using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BotManager : MonoBehaviour
{
    public static BotManager instance;
    public GameObject BotPrefab;
    public Transform BotPos,targetPlayer;
    public List<GameObject> botsList = new List<GameObject>();
    public int currentbot = 0;
    GameObject BotObj;
    //-----------------
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
       // StartCoroutine(BotsTurn());
    }
    public void initBot()
    {
        BotObj = Instantiate(BotPrefab, BotPos.position, Quaternion.identity);
    }
    public void BotTurn(GameObject target)
    {
        targetPlayer = target.transform;
        BotObj.GetComponent<BotController>().isMyTurn = true;
        StartCoroutine(botsList[currentbot].GetComponent<BotController>().Move());
    }
    //
    //IEnumerator BotsTurn()
    //{
    //    yield return new WaitForSeconds(2);
    //    botsList[currentbot].GetComponent<BotController>().isMyTurn = true;
    //    int target = Random.Range(0, botsList.Count);
    //    while (target == currentbot)
    //    {
    //        target = Random.Range(0, botsList.Count);
    //        if (target != currentbot) { break; }
    //    }
    //    Debug.Log("Target: "+target);
    //    StartCoroutine(botsList[currentbot].GetComponent<BotController>().Move(target));
    //    yield return new WaitForSeconds(6);
    //    botsList[currentbot].GetComponent<BotController>().canMove = false;
    //    botsList[currentbot].GetComponent<BotController>().isMyTurn = false;
    //    //StopCoroutine(botsList[currentbot].GetComponent<BotController>().Move());
    //    currentbot++;
    //    if (currentbot >= botsList.Count)
    //    {
    //        currentbot = 0;
    //    }
    //    //StartCoroutine(BotsTurn());
    //}
}
