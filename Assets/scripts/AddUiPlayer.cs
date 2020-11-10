using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AddUiPlayer : MonoBehaviour
{
    public playerAddController controller;
    public bool ready = false;




    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPickup()
    {
        if (controller.amIReady(gameObject.GetComponent<PlayerInput>()))
        {
            controller.TryToStart();
        }
        else
        {
            controller.ReadyPlayer(gameObject.GetComponent<PlayerInput>());
        }
    }
    public void OnDrop()
    {
        if (!controller.amIReady(gameObject.GetComponent<PlayerInput>()))
        {
            controller.OnPlayerleft(gameObject.GetComponent<PlayerInput>());
            Destroy(gameObject);
        }
        else
        {
            controller.UnReadyPlayer(gameObject.GetComponent<PlayerInput>());
        }
    }

    public void OnMoveleft()
    {
        controller.TrySelectleft(gameObject.GetComponent<PlayerInput>());
    }

    public void OnMoveright()
    {
        controller.TrySelectright(gameObject.GetComponent<PlayerInput>());
    }

}
