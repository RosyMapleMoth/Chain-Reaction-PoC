using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;


public class playerAddController : MonoBehaviour
{

    public struct PlayerStuff
    {
        public int PlayerNum;
        public GameObject Pannal;
       public bool Ready;

       public InputDevice myDevice;

       public void setReady(bool readySet)
       {
           Debug.Log("Settings ready from : " + Ready + " to " + readySet );
           this.Ready = readySet;
       }
    }


    public List<GameObject> UnCheckedOutPannals;
    public List<PlayerInput> Players;
    public Dictionary<PlayerInput, PlayerStuff> CheckedOutPannals;

    // Start is called before the first frame update
    void Start()
    {
        CheckedOutPannals = new Dictionary<PlayerInput, PlayerStuff>();

        // load in existing players, XXX this may need to be modified before actual use due to playerInputs being disconnected form their bodies
        Players = Settings.Instance.Players;

        // set up lists as thought the players joined, we may still have the bodieless issue of the players
        foreach (PlayerInput playerInput in Players)
        {
            Players.Add(playerInput);
            PlayerStuff thisPlayer = new PlayerStuff();
            thisPlayer.Pannal = UnCheckedOutPannals[0];
            thisPlayer.Ready = false;
            thisPlayer.PlayerNum = Players.Count;

            CheckedOutPannals.Add(playerInput, thisPlayer);
            CheckedOutPannals[playerInput].Pannal.SetActive(true);
            UnCheckedOutPannals.RemoveAt(0);
            playerInput.GetComponent<AddUiPlayer>().controller = gameObject.GetComponent<playerAddController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
                
    }


    // when a player is added make sure that lists are populated properly, and the player UI element is showen
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // add player to list 
        Players.Add(playerInput);
        
        PlayerStuff thisPlayer = new PlayerStuff();
        thisPlayer.Pannal = UnCheckedOutPannals[0];
        thisPlayer.Ready = false;
        thisPlayer.myDevice = playerInput.devices.First();
        thisPlayer.PlayerNum = Players.Count;


        Debug.Log("player # " + thisPlayer.PlayerNum + "paird with device" + thisPlayer.myDevice.name);

        CheckedOutPannals.Add(playerInput, thisPlayer);
        CheckedOutPannals[playerInput].Pannal.SetActive(true);
        
        UnCheckedOutPannals.RemoveAt(0);

        playerInput.GetComponent<AddUiPlayer>().controller = gameObject.GetComponent<playerAddController>();
    }

    // when a player is removed make sure that lists are populated properly, and the player UI element is not shown
    public void OnPlayerleft(PlayerInput playerInput)
    {
        CheckedOutPannals[playerInput].Pannal.SetActive(false);
        UnCheckedOutPannals.Add(CheckedOutPannals[playerInput].Pannal);
        UnCheckedOutPannals = UnCheckedOutPannals.OrderBy(tile => tile.name).ToList();
        CheckedOutPannals.Remove(playerInput);

        Players.Remove(playerInput);
    }


    public void Save()
    {
        List<PlayerStuff> temp = new List<PlayerStuff>();

        foreach(PlayerInput playerInput in Players)
        {
            temp.Add(CheckedOutPannals[playerInput]);
        }

        temp = temp.OrderBy(tile => tile.PlayerNum).ToList();
        
        Settings.Instance.Players = Players; 
        Settings.Instance.playerData = temp; 

    }


    public void ReadyPlayer(PlayerInput player)
    {
        PlayerStuff temp = CheckedOutPannals[player];
        temp.setReady(true);
        CheckedOutPannals.Remove(player);
        CheckedOutPannals.Add(player,temp);
        CheckedOutPannals[player].Pannal.transform.GetChild(1).gameObject.SetActive(true);

    }

    public void UnReadyPlayer(PlayerInput player)
    {
        PlayerStuff temp = CheckedOutPannals[player];
        temp.setReady(false);
        CheckedOutPannals.Remove(player);
        CheckedOutPannals.Add(player,temp);
        CheckedOutPannals[player].Pannal.transform.GetChild(1).gameObject.SetActive(false);

    }


    public void TrySelectleft()
    {

    }


    public void T



    public void TryToStart()
    {
        bool fullcheck = true;
        foreach (PlayerInput playerInput in Players)
        {
            Debug.Log(CheckedOutPannals[playerInput].Ready);
            if (!CheckedOutPannals[playerInput].Ready)
            {
                fullcheck = false;
                break;
            }
        }
        if (fullcheck)
        {
            startGame();
        }
    }

    private void startGame()
    {
        Save();
        SceneManager.LoadScene("multiPlayer");
    }
}
