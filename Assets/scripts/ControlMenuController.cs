using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMenuController : MonoBehaviour
{
    public KeyCode P1L, P1R, P1D, P1P, P1PU, P2L, P2R, P2U, P2P, P2PU;

    


    // Start is called before the first frame update
    void Start()
    {
        P1L = Settings.Instance.getPlayerOneLeft();
        P1R = Settings.Instance.getPlayerOneRight();
        P1D = Settings.Instance.getPlayerOneDrop();
        //P1P = Settings.Instance.getPlayerOnePick();
        P1PU = Settings.Instance.getPlayerOnePick();
        P2L = Settings.Instance.getPlayerTwoLeft();
        P2R = Settings.Instance.getPlayerTwoRight();
        P2U = Settings.Instance.getPlayerTwoDrop();
        //P2P = Settings.Instance.getPlayerOneLeft();
        P2PU = Settings.Instance.getPlayerTwoPick();
    }

    // Update is called once per frame
    void Update()
    {
         
    }



    

}
