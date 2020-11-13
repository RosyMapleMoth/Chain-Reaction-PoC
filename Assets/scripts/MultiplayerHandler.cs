using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class MultiplayerHandler : MonoBehaviour
{
    public List<PlayerInput> Players;

    // Start is called before the first frame update
    void Start()
    {
        setplayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setplayers()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Debug.Log("setting player " + i + " with device " + Settings.Instance.Devices[i]);
            Players[i].user.UnpairDevices();
            InputUser.PerformPairingWithDevice(Settings.Instance.Devices[i],Players[i].user);
        }
    }

    void SetUpPlayer(GameObject playerHolder)
    {
        
        PlayerInput player = playerHolder.AddComponent<PlayerInput>() as PlayerInput;
        player.SwitchCurrentActionMap("ISplayer");
    }
}

