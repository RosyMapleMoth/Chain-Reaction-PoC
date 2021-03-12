using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using UnityEngine.Events;

public class MultiplayerHandler : MonoBehaviour
{
    public List<PlayerInput> Players;
    public List<Image> portrats;

    public List<playerGameOverHandler> GameOverHandlers;

    private int RemainingPlayers;
    public List<Sprite> possablePortrats;

    // UI refrences
    public GameObject GameOverUI;

    // Start is called before the first frame update
    void Start()
    {
        setplayers();
        for (int i = 0; i < Players.Count; i++)
        {
            // I hate this, but the scope of i is outside of each loop, so we need to make an in scope varaible that will stay the same while we wait on the delegate
            int j = i;

            // set up player loss Listener 
            Players[i].transform.GetComponentInParent<BoardManager>().GameOver.AddListener(delegate{Debug.Log(" players " + j +  " eleminated"); playerLoss(j);});
        }
        RemainingPlayers = Players.Count;
    }


    // Trigger a loss for player at index playerNum. see GameOverHandlers for index values
    private void playerLoss(int playerNum)
    {
        //Players[playerNum].user.UnpairDevices();
        GameOverHandlers[playerNum].endGame(RemainingPlayers--,playerNum);
    }


    public void endGame()
    {
        GameOverUI.SetActive(true); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void setplayers()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            try {
                portrats[i].sprite = possablePortrats[Settings.Instance.playerData[i].SelectedChar];
            Debug.Log("setting player " + i + " with device " + Settings.Instance.Devices[i]);
            Players[i].user.UnpairDevices();
            InputUser.PerformPairingWithDevice(Settings.Instance.Devices[i],Players[i].user);
            }
            catch
            {
                Debug.Log("own no youw nowt supposed tuwu see thiws scween");
            }
        }
    }
}

