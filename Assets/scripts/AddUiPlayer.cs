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
        if (ready)
        {
            controller.TryToStart();
        }
        else
        {
            controller.ReadyPlayer(gameObject.GetComponent<PlayerInput>());
            ready = true;
        }
    }
    public void OnDrop()
    {
        if (!ready)
        {
            controller.OnPlayerleft(gameObject.GetComponent<PlayerInput>());
            Destroy(gameObject);
        }
        else
        {
            controller.UnReadyPlayer(gameObject.GetComponent<PlayerInput>());
            ready = false;
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
