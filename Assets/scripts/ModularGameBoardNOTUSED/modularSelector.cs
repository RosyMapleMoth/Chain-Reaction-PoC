using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modularSelector : MonoBehaviour
{
    public playerGameOverHandler afterGameMenu;
    public LineRenderer selectLine;
    private int CurCol = 3;
    static float MOVE_TIME = 0.05f;
    static int MIN_BOARD_SIZE = 0;
    static int MAX_BOARD_SIZE = 6;
    public OrbManipulator OrbManip;

    public GameStateManger stateManger;
    public float elapsedTime = 0.00f;
    public bool moving;
    public Vector3 MoveTarget;
    public Vector3 Ancor;
    public Vector3 MoveStart;

    public (int col, string PickUp, float timer) pickUpQue;

    // Start is called before the first frame update
    void Start()
    {
        pickUpQue = (-1, "", -1);
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
        
        playingUpdate();

    }


    public void playingUpdate()
    {

        if (OrbManip.board.GetColSize(GetCurCol()) > 0)
        {
            selectLine.SetPosition(0, new Vector3(selectLine.GetPosition(0).x, OrbManip.board.getRelativeOragin().y - (1.125f)*OrbManip.board.GetColSize(GetCurCol()) - 1, selectLine.GetPosition(0).z));
        }
        else
        {
            selectLine.SetPosition(0, new Vector3(selectLine.GetPosition(0).x, 6 - GameBoard.Y_OFF_SET*2, selectLine.GetPosition(0).z));
        }
        if (OrbManip.OrbsBeingPickedUp > 0 || OrbManip.OrbsBeingDropped > 0)
        {
            if (pickUpQue.col != -1)
            {
                if (pickUpQue.PickUp == "Up")
                {
                    stateManger.RequestGetOrb(CurCol);
                }
                else
                {
                    OrbManip.AttemptDropOrbs(CurCol);
                }
            }
        }
        if (pickUpQue.timer > 0)
        {
            pickUpQue.timer -= Time.deltaTime;

        }
        else
        {
            pickUpQue = (-1, "None", -1);
        }



        if (moving)
        {
            if (elapsedTime < MOVE_TIME)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(MoveStart, MoveTarget, (elapsedTime / MOVE_TIME));
                if (OrbManip.board.GetColSize(GetCurCol()) > 0)
                {
                    selectLine.SetPosition(0, new Vector3(transform.position.x - OrbManip.transform.position.x, OrbManip.board.getRelativeOragin().y - OrbManip.board.GetColSize(GetCurCol())- 1, selectLine.GetPosition(0).z));
                    selectLine.SetPosition(1, new Vector3(transform.position.x - OrbManip.transform.position.x, selectLine.GetPosition(1).y, selectLine.GetPosition(1).z));

                }
                else
                {
                    selectLine.SetPosition(0, new Vector3(transform.position.x - OrbManip.transform.position.x, 6, selectLine.GetPosition(0).z));
                    selectLine.SetPosition(1, new Vector3(transform.position.x - OrbManip.transform.position.x, selectLine.GetPosition(1).y, selectLine.GetPosition(1).z));
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
        if (stateManger.GetState() == GameStateManger.GameState.GameOver)
        {
            //afterGameMenu.OnMoveright();
        }
        else
        {  
            if (/*gameMng.isInState(GameStateManger.GameState.playing) &&*/ OrbManip.CanAct()  && OrbManip.heldOrbs.Count > 0)
            {
                OrbManip.AttemptDropOrbs(GetCurCol());
            }
            else if (OrbManip.OrbsBeingPickedUp > 0 || OrbManip.OrbsBeingDropped > 0)
            {
                pickUpQue = (GetCurCol(),"Drop",0.2f);
            }
        }
    }

    public void OnPickup()
    {
         if (stateManger.GetState() == GameStateManger.GameState.GameOver)
        {
            //afterGameMenu.OnMoveright();
        }
        else
            {  
            if (/*gameMng.isInState(GameStateManger.GameState.playing) && */ OrbManip.CanAct())
            {
                stateManger.RequestGetOrb(GetCurCol());
            }
            else if (OrbManip.OrbsBeingPickedUp > 0 || OrbManip.OrbsBeingDropped > 0)
            {
                pickUpQue = (GetCurCol(),"Up",0.2f);

            }
        }
    }




    public void OnMoveleft()
    {

        if (stateManger.GetState() == GameStateManger.GameState.GameOver)
        {
            //afterGameMenu.OnMoveright();
        }
        else
        {  
            if (CurCol > MIN_BOARD_SIZE)
            {
                // if we are already moving snap to target
                if (moving)
                {
                    
                    transform.position = MoveTarget;

                }
                
                // set movement start and end positions
                MoveStart = transform.position;
                MoveTarget = new Vector3(transform.position.x - 1.125f, transform.position.y, transform.position.z);

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
        if (stateManger.GetState() == GameStateManger.GameState.GameOver)
        {
            //afterGameMenu.OnMoveright();
        }
        else
        {
            if (CurCol < MAX_BOARD_SIZE && CurCol < MAX_BOARD_SIZE  /* && gameMng.isInState(GameStateManger.GameState.playing) */)
            {      
            // if we are already moving snap to target
            if (moving)
            {
                
                transform.position = MoveTarget;

            }

            MoveStart = transform.position;
            MoveTarget = new Vector3(transform.position.x + 1.125f, transform.position.y, transform.position.z);
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
