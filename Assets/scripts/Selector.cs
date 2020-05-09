using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{

    public GameObject Cursor;
    private int CurCol = 3;
    static int MIN_BOARD_SIZE = 0;
    static int MAX_BOARD_SIZE = 6;
    public BoardManager gameMng;
    public LineRenderer selectorLine;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int GetCurCol()
    {
        return CurCol;
    }


    // Update is called once per frame
    void Update()
    {
        // move left
        if (Input.GetKeyDown(KeyCode.A) && CurCol > MIN_BOARD_SIZE || Input.GetKeyDown(KeyCode.LeftArrow) && CurCol > MIN_BOARD_SIZE)
        {

            transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            

            CurCol -= 1;
        }
        // move right
        if (Input.GetKeyDown(KeyCode.D) && CurCol < MAX_BOARD_SIZE || Input.GetKeyDown(KeyCode.RightArrow) && CurCol < MAX_BOARD_SIZE)
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            

            CurCol += 1;

        }
        // grab
        if (Input.GetKeyDown(KeyCode.Z))
        {
           
            gameMng.AttemptGrabOrb(CurCol);
           
        }
        // Place
        if (Input.GetKeyDown(KeyCode.X))
        {
            gameMng.DropOrbs(CurCol);
        }

    }
}
