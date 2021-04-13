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
    public GameObject GameOverUI;

    public List<int> playerSelections;

    // Start is called before the first frame update
    void Start()
    {
        setplayers();
        for (int i = 0; i < Players.Count; i++)
        {
            //playerSelections = 0;
            // I hate this, but the scope of i is outside of each loop, so we need to make an in scope varaible that will stay the same while we wait on the delegate
            int j = i;

            // set up player loss Listener 
            Players[i].transform.GetComponentInParent<BoardManager>().GameOver.AddListener(delegate
            {
                Debug.Log("player " + j +  " eleminated"); 
                playerLoss(j);
            });

            // set up ready player Listener
            GameOverHandlers[i].OptionSelected.AddListener(delegate
            {
                Debug.Log("player " + j +  " selected option ");
                // TODO
                Debug.LogWarning("Selected call from player " + j + ": Function not implamented yet "); 
            });

            GameOverHandlers[i].OptionDeselected.AddListener(delegate
            {
                Debug.Log("player " + j +  " deselected option ");
                // TODO 
                Debug.LogWarning("Deselected call from player " + j + ": Function not implamented yet ");
            });

        }
        RemainingPlayers = Players.Count;
    }


    // Trigger a loss for player at index playerNum. see GameOverHandlers for index values
    private void playerLoss(int playerNum)
    {
        //Players[playerNum].user.UnpairDevices();
        GameOverHandlers[playerNum].endGame(RemainingPlayers--);
    }


    public void endGame()
    {
        GameOverUI.SetActive(true); 
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

    private void exacuteSelection()
    {

    } 
}

