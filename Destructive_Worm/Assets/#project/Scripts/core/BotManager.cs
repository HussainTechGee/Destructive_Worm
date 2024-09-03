using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Fusion;
using static UnityEngine.GraphicsBuffer;

public class BotManager : NetworkBehaviour
{
    public static BotManager instance;
    public GameObject BotPrefab;
    public Transform targetPlayer;
    public int currentbot = 0;
    private bool isHostClient = false;
    public string[] FirstNameList;
    public string[] LastNameList;
    //-----------------
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
       // StartCoroutine(BotsTurn());
    }
    public override void Spawned()
    {
        // Check if this client is the designated host in Shared Mode
        if (Runner.IsSharedModeMasterClient)
        {
            Debug.Log("Master for Bot");
            isHostClient = true;
            SpawnBots();
        }
        else
        {
            // Disable the script if this is not the host client
            enabled = false;
        }
    }
    private void SpawnBots()
    {
        int playerCount = Runner.SessionInfo.PlayerCount;
        int botCount = 4 - playerCount;

        // Spawn the specified number of bots
        for (int i = 0; i < botCount; i++)
        {
            SpawnBot(playerCount+i);
        }
    }
    private void SpawnBot(int index)
    {
        if (Object.HasStateAuthority)
        {
            // Spawn the bot prefab with network synchronization
           NetworkObject botObj= Runner.Spawn(BotPrefab, BattleSystem.instance.playerBattleStation[index].position, Quaternion.identity);

            //Bot Name
            string botName = FirstNameList[Random.Range(0, FirstNameList.Length)] + " " + LastNameList[Random.Range(0, LastNameList.Length)];
            int botHp = Random.Range(70, 101);
            botObj.GetComponent<BotController>().PlayerId = index + 1;
            botObj.GetComponent<Unit>().SetNameAndHp(botName, botHp,index+1);
        }
    }
    public void BotTurn(GameObject botObj,GameObject target)
    {
        targetPlayer = target.transform;
        botObj.GetComponent<BotController>().isMyTurn = true;
        StartCoroutine(botObj.GetComponent<BotController>().Move());
    }
    public void OnPlayerLeft(PlayerRef player)
    {
        // Handle reassignment of bot control when the host client disconnects
        if (player == Object.StateAuthority)
        {
            AssignNewBotHost();
        }
    }
    private void AssignNewBotHost()
    {
        // Loop through all active players to find a new host
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            // Skip the current disconnected player and any invalid players
            if (player != Object.StateAuthority && player.IsRealPlayer)
            {
                // Reassign state authority to the first valid player found
                Object.AssignInputAuthority(player);
                Debug.Log($"New bot host assigned: {player}");
                return; // Exit once a new host is found
            }
        }

        // If no suitable player is found, log a warning
        Debug.LogWarning("No available player to reassign bot authority!");
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
