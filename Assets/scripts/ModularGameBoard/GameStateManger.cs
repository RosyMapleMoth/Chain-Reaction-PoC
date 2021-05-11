using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManger : MonoBehaviour
{
    public GameBoard board;
    public OrbManipulator orbTransport;
    public GameState curState;
    public GameState nextState;
    public enum GameState {idle, addLines, evaluate, initPop, popping, dropping};

    private static float POP_TIMER = 1.0f;
    private static float DROP_TIMER_MAX = 6f; 

    private static int POP_AT_ORB_COUNT = 3;
    public bool evalWhenReady = false;

    private bool grabWhenReady = false;
    private bool putWhenReady = false;

    private bool[,] recentlyMoved;
    
    

    public Text DEBUGCurState;
    public Text DEBUGNextState;

    public bool DEBUGpuased;
    public float popCountDown;
    private int grabCol = 0;
    private int putCol = 0;
    public bool addLineWhenReady = false;
    private Transform PutReuestParent;

    private int incomingSend = 1;

    private bool OrbBeingMoved = false;

    private float linedropTimer = 6; 

    public int CurrentChain = 0;

    // Start is called before the first frame update
    void Start()
    {
        curState = GameState.idle;
        nextState =GameState.idle;
        recentlyMoved = new bool[GameBoard.BOARD_HIGHT,GameBoard.BOARD_WIDTH];
    }







    // Update is called once per frame
    void Update()
    {
        DEBUGstatWatcher();
        if (DEBUGpuased)
        {

        }
        else
        {
            // tick down time until next drop
            linedropTimer -= Time.deltaTime;

            if (linedropTimer <= 0)
            {
                addLineWhenReady = true;
            }
            
            // if a grab was requested grab
            if (grabWhenReady)
            {
                orbTransport.AttmeptGrabOrbs(grabCol);
                OrbBeingMoved = false;
            }

            // if a put was attempted put it
            if (putWhenReady)
            {
                ResolvePutRequest();
                OrbBeingMoved = false;
                evalWhenReady = true;
            }
            
            // run current state
            switch (curState)
            {
                case GameState.idle: IdleState(); break;
                case GameState.evaluate: EvaluateState(); break;
                case GameState.initPop: PopInit(); break;
                case GameState.popping: PopState(); break;
                case GameState.dropping: DropState(); break;
                case GameState.addLines: AddLineState(); break;
                default: IdleState(); break;
            }   
        }
    }

    void LateUpdate()
    {
        if (curState != nextState)
        {
            Debug.Log("current State "+ curState.ToString() + " changed to " +nextState.ToString());
            curState = nextState;
        }
    }
    



    private void IdleState()
    {
        if (evalWhenReady)
        {
            nextState = GameState.evaluate;
        }
        else 
        {
            CurrentChain = 0;
            if (addLineWhenReady && !OrbBeingMoved)
            {
                nextState = GameState.addLines;
                addLineWhenReady = false;
            }
        }
    }

    private void EvaluateState()
    {

        // Evaluate the board to see if any orbs meet the pop condition 
        if (Evaluate())
        {
            nextState = GameState.initPop;
            popCountDown = POP_TIMER;
            CurrentChain++;
        }
        else 
        {
            nextState = GameState.idle;
        }
        evalWhenReady = false;
    }

    private void PopInit()
    {
        board.StartPoppingAllReadyOrbs();
        nextState = GameState.popping;
        popCountDown = POP_TIMER;
    }

    private void PopState()
    {
        popCountDown -= Time.deltaTime;

        if (popCountDown <= 0)
        { 
            board.endPoppingAllOrbs();

            if (board.OrbsCurrentlyDisplaced())
            {
                nextState = GameState.dropping;
            }
            else
            {
                nextState = GameState.idle;
            }
        }
    }


    private void DropState()
    {
        bool dropCompleate = true;
        if (dropCompleate)
        {
            nextState = GameState.evaluate;
        }

    }



    private void AddLineState()
    {
        board.dropLines(incomingSend);
        linedropTimer = DROP_TIMER_MAX;
        addLineWhenReady = false;
        Debug.Log("TEST");

        nextState = GameState.idle;
    }







    /// get and put Requests

    public void RequestGetOrb(int col)
    {
        grabWhenReady = true;
        grabCol = col;
    }
    
    public void RequestPutOrb(int col, Transform parent)
    {
        putWhenReady = true;
        PutReuestParent = parent;
        putCol = col;
    }




    
    private void ResolvePutRequest()
    {
        while (PutReuestParent.childCount > 0)
        {
            Debug.Log("placing orb " + PutReuestParent.GetChild(0).gameObject.name + " is done");
            PutReuestParent.GetChild(0).GetComponent<Orb>().SetOrbMoved(true);
            board.PlaceOrb(PutReuestParent.GetChild(0).gameObject, putCol);
            putWhenReady = false;
        }
        Debug.Log("all dropping orbs are placed");
        OrbBeingMoved = false;
    }





        /// <summary>
    /// 
    /// </summary>
    /// <param name="check"></param>
    /// <returns></returns>
    public bool isInState(GameState check)
    {
        return curState == check;
    }


    /// Debug tools


    public void DEBUGstatWatcher()
    {
        DEBUGCurState.text = curState.ToString();
        DEBUGNextState.text = nextState.ToString();
    }


    public void DEBUGPauseState()
    {
        DEBUGpuased = true;
    }





    /// Manipulation Checks


    public bool CanManipulateOrb()
    {
        return (!OrbBeingMoved);
    }

    public void ManipulatingOrb()
    {
        OrbBeingMoved = true;
    }









    public bool Evaluate()
    {
        bool[,] evalMemo;
        evalMemo = new bool[GameBoard.BOARD_WIDTH,GameBoard.BOARD_HIGHT];
        Queue<Orb> orbsToSet;
        orbsToSet = new Queue<Orb>();
        bool OrbsNeedToPop = false;

        for (int c = 0; c < GameBoard.BOARD_WIDTH; c++)
        {
            for (int r = 0; r < GameBoard.BOARD_HIGHT; r++)
            {
                if (!evalMemo[c,r])
                {
                    GameObject me;
                    me = board.At(c,r);
                    if (me != null)
                    {
                        Debug.Log("evaling orb at " + c + "," + r);
                        if (DFS_Orb_eval(evalMemo, orbsToSet, c, r, me.GetComponent<Orb>().GetOrbType()) && orbsToSet.Count >= POP_AT_ORB_COUNT)
                        {
                            Debug.Log("setting a bunch of orbs to ready to pop");
                            while (orbsToSet.Count > 0)
                            {
                                Debug.Log("Setting orb " + orbsToSet.Peek().gameObject.name + " to pop");
                                orbsToSet.Dequeue().SetReadyToPop();
                                OrbsNeedToPop = true;
                            }
                        }
                        else
                        {
                            orbsToSet.Clear();
                        }
                    }
                }
            }    
        }
        return OrbsNeedToPop;
    } 

    public bool DFS_Orb_eval(bool[,] evalMemo, Queue<Orb> orbsToSet, int x, int y, OrbType type)
    {

        // if we are out of bounds exit
        if (x < 0 || x >= GameBoard.BOARD_WIDTH || y < 0 || y >= GameBoard.BOARD_HIGHT || evalMemo[x,y])
        {
            return false;
        } 

        // minor caching (becuase at is an intensive fucntion)
        GameObject me = board.At(x,y);
        if (me == null)
        {
            return false;
        }


        Orb myOrb = me.GetComponent<Orb>();
        // if this orb isn't our color exit 
        if (!myOrb.checkOrbType(type))
        {
            return false;
        }
        else
        {
            Debug.Log("Setting " + x + ","+ y + " to evaluated");
            // mark that we have checked this orb
            evalMemo[x,y] = true;
            orbsToSet.Enqueue(myOrb);

            bool center = myOrb.HasOrbMoved();
            myOrb.SetOrbMoved(false);

            bool right = DFS_Orb_eval(evalMemo, orbsToSet, x+1, y, type);
            bool down = DFS_Orb_eval(evalMemo, orbsToSet, x, y+1, type);;

            return (center || right || down);
        }
        



    }
}
