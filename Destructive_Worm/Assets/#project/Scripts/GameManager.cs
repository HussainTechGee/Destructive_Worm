using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string PlayerName;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void StartGame()
    {

    }
    public void GoToScene(int index)
    {
        SceneManager.LoadScene(index);    
    }
}
