using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class playerAddController : MonoBehaviour
{
    public List<GameObject> Pannals;
    public List<PlayerInput> Players;

    public List<bool> Ready;
    // Start is called before the first frame update
    void Start()
    {
        // set up bool list
        Ready = new List<bool>();

        // load in existing players, XXX this may need to be modified before actual use due to playerInputs being disconnected form their bodies
        Players = Settings.Instance.Players;

        // set up lists as thought the players joined, we may still have the bodieless issue of the players
        foreach (PlayerInput playerInput in Players)
        {
            Players.Add(playerInput);
            playerInput.GetComponent<AddUiPlayer>().controller = gameObject.GetComponent<playerAddController>();
            Ready.Add(false);
            Pannals[Players.IndexOf(playerInput)].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // when a player is added make sure that lists are populated properly, and the player UI element is showen
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Players.Add(playerInput);
        playerInput.GetComponent<AddUiPlayer>().controller = gameObject.GetComponent<playerAddController>();
        Ready.Add(false);
        Pannals[Players.IndexOf(playerInput)].SetActive(true);
    }

    // when a player is removed make sure that lists are populated properly, and the player UI element is not shown
    public void OnPlayerleft(PlayerInput playerInput)
    {
        Pannals[Players.IndexOf(playerInput)].SetActive(false);
        Ready.RemoveAt(Players.IndexOf(playerInput));
        Players.Remove(playerInput);
    }


    public void Save()
    {
        Settings.Instance.Players = Players; 
    }


    public void ReadyPlayer(PlayerInput player)
    {
        Ready[Players.IndexOf(player)] = true;
        Pannals[Players.IndexOf(player)].transform.GetChild(1).gameObject.SetActive(true);

    }

    public void UnReadyPlayer(PlayerInput player)
    {
        Ready[Players.IndexOf(player)] = false;
        Pannals[Players.IndexOf(player)].transform.GetChild(1).gameObject.SetActive(false);

    }
    public void TryToStart()
    {
        bool fullcheck = true;
        foreach (bool check in Ready)
        {
            if (!check)
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
