using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public playerGameOverHandler afterGameMenu;
    public LineRenderer selectLine;
    public GameObject Cursor;
    private int CurCol = 3;

    static float MOVE_TIME = 0.05f;
    static int MIN_BOARD_SIZE = 0;
    static int MAX_BOARD_SIZE = 6;
    public BoardManager gameMng;
    public LineRenderer selectorLine;

    public float elapsedTime = 0.00f;

    public bool moving;

    public Vector3 MoveTarget;
    public Vector3 Ancor;
    public Vector3 MoveStart;

    public KeyCode Playerleft, Playerright, Playerup, Playerdown, PlayerPick, PlayerDrop;




    // Start is called before the first frame update
    void Start()
    {
        Ancor = transform.position;
        moving = false;
    }

    public int GetCurCol()
    {
        return CurCol;
    }


    // Update is called once per frame
    void Update()
    {
        switch (gameMng.curstate)
        {
            case BoardManager.GameState.starting:
                break;
            case BoardManager.GameState.playing:
                playingUpdate();
                break;
            default:
                break;
        }
    }


    public void playingUpdate()
    {

        if (gameMng.Cols[GetCurCol()].Count > 0)
        {
            selectLine.SetPosition(0, new Vector3(selectLine.GetPosition(0).x, gameMng.Cols[GetCurCol()].Last.Value.transform.position.y - 0.9f, selectLine.GetPosition(0).z));
        }
        else
        {
            selectLine.SetPosition(0, new Vector3(selectLine.GetPosition(0).x, 6, selectLine.GetPosition(0).z));
        }



        if (moving)
        {
            if (elapsedTime < MOVE_TIME)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(MoveStart, MoveTarget, (elapsedTime / MOVE_TIME));
                if (gameMng.Cols[GetCurCol()].Count > 0)
                {
                    selectLine.SetPosition(0, new Vector3(1.2f+transform.position.x - gameMng.transform.position.x, gameMng.Cols[GetCurCol()].Last.Value.transform.position.y - 0.9f, selectLine.GetPosition(0).z));
                    selectLine.SetPosition(1, new Vector3(1.2f+transform.position.x - gameMng.transform.position.x, selectLine.GetPosition(1).y, selectLine.GetPosition(1).z));

                }
                else
                {
                    selectLine.SetPosition(0, new Vector3(1.2f+transform.position.x - gameMng.transform.position.x, 6, selectLine.GetPosition(0).z));
                    selectLine.SetPosition(1, new Vector3(1.2f+transform.position.x - gameMng.transform.position.x, selectLine.GetPosition(1).y, selectLine.GetPosition(1).z));
                }
            }
            else
            {
                transform.position = MoveTarget;
                moving = false;
            }
        }
    }

    public void OnDrop()
    {
        if (gameMng.curstate == BoardManager.GameState.playing && gameMng.OrbsBeingGrabed == 0)
        {
            gameMng.DropOrbs(CurCol);
        }
    }

    public void OnPickup()
    {
        if (gameMng.curstate == BoardManager.GameState.playing && gameMng.OrbsBeingGrabed == 0)
        {
            gameMng.AttemptGrabOrb(CurCol);
        }
    }




    public void OnMoveleft()
    {

        if (gameMng.isGameOver)
        {
            afterGameMenu.OnMoveleft();
        }
        else
        {        
            if (CurCol > MIN_BOARD_SIZE && gameMng.curstate == BoardManager.GameState.playing)
            {
                // if we are already moving snap to target
                if (moving)
                {
                    
                    transform.position = MoveTarget;

                }
                
                // set movement start and end positions
                MoveStart = transform.position;
                MoveTarget = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);

                // set up move helper to be active
                moving = true;
                elapsedTime = 0.00f;
                
                // update the col data
                CurCol -= 1; 
            }
        }
    }

    public void OnMoveright()
    {
        if (gameMng.isGameOver)
        {
            afterGameMenu.OnMoveright();
        }
        else
        {
            if (CurCol < MAX_BOARD_SIZE && CurCol < MAX_BOARD_SIZE  && gameMng.curstate == BoardManager.GameState.playing)
            {      
            // if we are already moving snap to target
            if (moving)
            {
                
                transform.position = MoveTarget;

            }

            MoveStart = transform.position;
            MoveTarget = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            moving = true;
            elapsedTime = 0.00f;


            CurCol += 1;
            }
        }
    }




  /*/// <summary>
    /// 
    /// </summary>
    public void updateSelectLineVert()
    {
        if (Cols[switcher.GetComponent<Selector>().GetCurCol()].Count > 0)
        {
            selectLine.SetPosition(0, new Vector3(-3f + switcher.GetComponent<Selector>().GetCurCol(), Cols[switcher.GetComponent<Selector>().GetCurCol()].Last.Value.transform.position.y - 0.9f, selectLine.GetPosition(0).z));
            selectLine.SetPosition(1, new Vector3(-3f + switcher.GetComponent<Selector>().GetCurCol(), selectLine.GetPosition(1).y, selectLine.GetPosition(1).z));

        }
        else
        {
            selectLine.SetPosition(0, new Vector3(-3f + switcher.GetComponent<Selector>().GetCurCol(), 6, selectLine.GetPosition(0).z));
            selectLine.SetPosition(1, new Vector3(-3f + switcher.GetComponent<Selector>().GetCurCol(), selectLine.GetPosition(1).y, selectLine.GetPosition(1).z));
        }
    }
*/
}