using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class MultiplayerHandler : MonoBehaviour
{
    public List<PlayerInput> Players;
    public List<Image> portrats;

    public List<Sprite> possablePortrats;

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
            try {
                portrats[i].sprite = possablePortrats[Settings.Instance.playerData[i].SelectedChar];
            Debug.Log("setting player " + i + " with device " + Settings.Instance.Devices[i]);
            Players[i].user.UnpairDevices();
            InputUser.PerformPairingWithDevice(Settings.Instance.Devices[i],Players[i].user);
            }
            catch
            {
                Debug.Log("uwu down't have enough contwowwews pwugged in. No pwayews set up");
            }
        }
    }

    void SetUpPlayer(GameObject playerHolder)
    {
        
        PlayerInput player = playerHolder.AddComponent<PlayerInput>() as PlayerInput;
        player.SwitchCurrentActionMap("ISplayer");
    }
}

