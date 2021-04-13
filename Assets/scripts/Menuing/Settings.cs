using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Settings : MonoBehaviour
{


    public List<InputUser> Players;
    public List<InputDevice> Devices;

    public List<playerAddController.PlayerStuff> playerData;


    public static Settings Instance;

    public float Vol;



    void Awake ()   
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Players = new List<InputUser>();
            playerData = new List<playerAddController.PlayerStuff>();
            Devices = new List<InputDevice>();

            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy (gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }


    


}
