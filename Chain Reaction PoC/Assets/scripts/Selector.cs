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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && CurCol > MIN_BOARD_SIZE || Input.GetKeyDown(KeyCode.LeftArrow) && CurCol > MIN_BOARD_SIZE)
        {

            transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            CurCol -= 1;
        }
        if (Input.GetKeyDown(KeyCode.D) && CurCol < MAX_BOARD_SIZE || Input.GetKeyDown(KeyCode.RightArrow) && CurCol < MAX_BOARD_SIZE)
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            CurCol += 1;

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameMng.AttemptGrabOrb(CurCol);
        }

    }
}
