using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;


public class playerAddController : MonoBehaviour
{

    public class PlayerStuff
    { 
        public int PlayerNum;
        public GameObject Pannal;
        public bool Ready;
        public int SelectedChar;

        public InputDevice myDevice;
        public void setReady(bool readySet)
        {
            Debug.Log("Settings ready from : " + Ready + " to " + readySet );
            this.Ready = readySet;
        }
    }


    public List<GameObject> UnCheckedOutPannals;

    public Dictionary<int, bool> CheckedoutChars;

    public const int AmountOfChar = 4;
    public List<PlayerInput> Players;
    public Dictionary<PlayerInput, PlayerStuff> CheckedOutPannals;

    // Start is called before the first frame update
    void Start()
    {
        CheckedOutPannals = new Dictionary<PlayerInput, PlayerStuff>();
        CheckedoutChars = new Dictionary<int, bool>();

        // load in existing players, XXX this may need to be modified before actual use due to playerInputs being disconnected form their bodies
        Players = new List<PlayerInput>();

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
        for (int i = 0; i < AmountOfChar; i++)
        {
            if (!CheckedoutChars.ContainsKey(i))
            {
                selectImage(thisPlayer.Pannal.transform.GetChild(2).GetChild(i).GetComponentInChildren<Image>());
                thisPlayer.SelectedChar = i;
                break;
            }
        }


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

        deselectImage(CheckedOutPannals[playerInput].Pannal.transform.GetChild(2).GetChild(CheckedOutPannals[playerInput].SelectedChar).GetComponentInChildren<Image>());

        CheckedOutPannals.Remove(playerInput);
        Players.Remove(playerInput);

    }


    public void Save()
    {
        foreach (PlayerInput player in Players)
        {
            Settings.Instance.Players.Add(player.user); 
            Settings.Instance.Devices.Add(player.user.pairedDevices[0]);
        }
    }


    public void ReadyPlayer(PlayerInput player)
    {
        PlayerStuff CurPlayer = CheckedOutPannals[player];
        if (!CheckedoutChars.ContainsKey(CurPlayer.SelectedChar))
        {
            CheckedoutChars.Add(CurPlayer.SelectedChar,true); 
            CurPlayer.setReady(true);
            CurPlayer.Pannal.transform.GetChild(1).gameObject.SetActive(true);
        }
        

    }

    public void UnReadyPlayer(PlayerInput player)
    {
        PlayerStuff CurPlayer = CheckedOutPannals[player];
        CheckedoutChars.Remove(CurPlayer.SelectedChar);
        CurPlayer.setReady(false);
        CurPlayer.Pannal.transform.GetChild(1).gameObject.SetActive(false);

    }


    public void TrySelectleft(PlayerInput player)
    {
        PlayerStuff curPlayer = CheckedOutPannals[player];
        if (!curPlayer.Ready && curPlayer.SelectedChar > 0)
        {
            deselectImage(curPlayer.Pannal.transform.GetChild(2).GetChild(curPlayer.SelectedChar).GetComponentInChildren<Image>());
            curPlayer.SelectedChar--;
            selectImage(curPlayer.Pannal.transform.GetChild(2).GetChild(curPlayer.SelectedChar).GetComponentInChildren<Image>());
        }
    }

    public void TrySelectright(PlayerInput player)
    {
        int temp = CheckedOutPannals[player].SelectedChar;
        if (!CheckedOutPannals[player].Ready && temp < AmountOfChar -1)
        {
            
            deselectImage(CheckedOutPannals[player].Pannal.transform.GetChild(2).GetChild(temp).GetComponentInChildren<Image>());
            temp++;
            selectImage(CheckedOutPannals[player].Pannal.transform.GetChild(2).GetChild(temp).GetComponentInChildren<Image>());

            CheckedOutPannals[player].SelectedChar = temp;
        }
    }


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
            Save();
            startGame(Players.Count);
        }
    }

    private void startGame(int amountOfPlayers)
    {
        switch (amountOfPlayers)
            {
                case 1:
                    break;
                case 2:
                    Save();
                    SceneManager.LoadScene("Two Player");
                    break;
                case 3:
                    Save();
                    SceneManager.LoadScene("Three Player");
                    break;
                case 4:
                    Save();
                    SceneManager.LoadScene("Four Player");
                    break;
                default:
                    break;
            }
    }

    public bool amIReady(PlayerInput player)
    {
        return CheckedOutPannals[player].Ready;

    }


    private void selectImage(Image img)
    {
        img.color = new Color32(255,255,255,255);
        img.transform.localScale = new Vector3(1.2f,1.2f,1);
    }

    private void deselectImage(Image img)
    {
        img.color = new Color32(87,87,87,255);
        img.transform.localScale = new Vector3(1f,1f,1f);
    }

}
