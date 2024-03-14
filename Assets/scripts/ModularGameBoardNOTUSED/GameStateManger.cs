using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameStateManger : MonoBehaviour
{
    public GameBoard board;
    public OrbManipulator orbTransport;
    public UnityEvent GameOverDel = new UnityEvent();
    public scoreTMP score;
    public bool DebugMode;
    public Animator GameOverScreen;
    public Animator dropArrow;


    public GameState curState;
    public GameState nextState;
    public TextMeshProUGUI maxtime;
    public enum GameState {idle, addLines, evaluate, initPop, popping, dropping, EndingGame, GameOver};
    private static float POP_TIMER = 1.0f;
    private static float DROP_TIMER_MAX = 5f; 
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
    public float LinesToAdd = 0;
    private Transform PutReuestParent;
    private int incomingSend = 1;
    public bool OrbBeingMoved = false;
    private float linedropTimer = 6; 
    public float dropTime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        curState = GameState.idle;
        nextState = GameState.idle;
        recentlyMoved = new bool[GameBoard.BOARD_HIGHT,GameBoard.BOARD_WIDTH];
    }

    public GameState GetState()
    {
        return curState;
    }





    // Update is called once per frame
    void Update()
    {
        if (DebugMode)
        {
            DEBUGstatWatcher();
        }
        if (DEBUGpuased)
        {

        }
        else
        {
            // tick down time until next wrop
            linedropTimer -= Time.deltaTime;
            if (linedropTimer <= 2.30 && linedropTimer > 0)
            {
                
                dropArrow.SetBool("Switching", true);
                
            }
            if (linedropTimer <= 0)
            {
                addLineWhenReady = true;
            }
            
            // if a grab was requested grab
            if (grabWhenReady && curState != GameState.evaluate && nextState != GameState.evaluate)
            {
                //OrbBeingMoved = true;
                grabWhenReady = false;
                orbTransport.AttmeptGrabOrbs(grabCol);
            }

            // if a put was attempted put it
            if (putWhenReady)
            {
                ResolvePutRequest();
                //OrbBeingMoved = false;
                evalWhenReady = true;
                FinishManipulatingOrb();
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
                case GameState.EndingGame: EndGame(); break;
                case GameState.GameOver: GameOver(); break;
                default: IdleState(); break;
            }   
        }
    }

    private void GameOver()
    {
        return;
    }

    private void EndGame()
    {
        GameOverDel.Invoke();
        GameOverScreen.SetTrigger("death");
        nextState  = GameState.GameOver;
    }

    /// 
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
            score.resetChain();
            if (board.isGameOver())
            {
                nextState = GameState.EndingGame;
            }
            else if (addLineWhenReady && !OrbBeingMoved)
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
        }
        else 
        {
            nextState = GameState.idle;
        }
        evalWhenReady = false;
    }

    private void PopInit()
    {
        score.scoreBlock(board.StartPoppingAllReadyOrbs());
        nextState = GameState.popping;
        popCountDown = POP_TIMER;
    }

    private void PopState()
    {
        popCountDown -= Time.deltaTime;

        if (popCountDown <= 0)
        { 
            board.endPoppingAllOrbs();
            score.finializePop();
            board.MarkAllDisplacedOrbs();
        
            if (board.OrbsCurrentlyDisplaced())
            {
                nextState = GameState.dropping;
                dropTime = 0.155f;
            }
            else
            {
                nextState = GameState.idle;
            }
        }
    }


    private void DropState()
    {

        dropTime -= Time.deltaTime;
        if (dropTime <= 0)
        {
            nextState = GameState.evaluate;
            board.FinishAllDroppingOrbs();
        }

    }



    private void AddLineState()
    {
        board.dropLines(incomingSend);
        restLineDropTimer();
        if (DebugMode)
        {
            maxtime.text = linedropTimer.ToString();
        }
        addLineWhenReady = false;
        Debug.Log("TEST");
        dropArrow.SetBool("Switching", false);
        dropArrow.SetTrigger("orbs Dropped");
        nextState = GameState.idle;
    }







    /// <summary>
    /// 
    /// </summary>
    /// <param name="col"></param>
    public void RequestGetOrb(int col)
    {
        grabWhenReady = true;
        grabCol = col;
    }






    /// <summary>
    /// 
    /// </summary>
    /// <param name="col"></param>
    /// <param name="parent"></param>
    public void RequestPutOrb(int col, Transform parent)
    {
        putWhenReady = true;
        PutReuestParent = parent;
        putCol = col;
    }




    /// <summary>
    /// 
    /// </summary>
    private void ResolvePutRequest()
    {
        while (PutReuestParent.childCount > 0)
        {
            Transform curChild = PutReuestParent.GetChild(PutReuestParent.childCount - 1);
            Debug.Log("placing orb " + curChild.gameObject.name + " is done");
            curChild.GetComponent<Orb>().SetOrbMoved(true);
            board.PlaceOrb(curChild.gameObject, putCol);
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

        if (DebugMode)
        {
            DEBUGCurState.text = curState.ToString();
            DEBUGNextState.text = nextState.ToString();
        }
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

    public void FinishManipulatingOrb()
    {
        OrbBeingMoved = false;
    }




    /// <summary>
    /// Evaluates the board
    /// </summary>
    /// <returns></returns>
    public bool Evaluate()
    {
        bool[,] evalMemo;
        evalMemo = new bool[GameBoard.BOARD_WIDTH,board.longestHight()];
        Queue<Orb> orbsToSet;
        orbsToSet = new Queue<Orb>();
        bool OrbsNeedToPop = false;

        for (int c = 0; c < GameBoard.BOARD_WIDTH; c++)
        {
            for (int r = 0; r < board.longestHight(); r++)
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






    /// <summary>
    /// Helper function for eval function which uses a DFS to evaluate all orbs
    /// </summary>
    /// <param name="evalMemo">array that we use to memeoize our problem</param>
    /// <param name="orbsToSet">Que of Orbs we will have to change after the eval is resolved</param>
    /// <param name="x">X value of orb being evaluated</param>
    /// <param name="y">Y value of orb being evaluated</param>
    /// <param name="type">Color of orb being evaluated</param>
    /// <returns>Returns true if current orb is the same type as inital orb, or if current orb has alreadty been evaluated as needing to be changed</returns>
    private bool DFS_Orb_eval(bool[,] evalMemo, Queue<Orb> orbsToSet, int x, int y, OrbType type)
    {

        // if we are out of bounds exit
        if (x < 0 || x >= GameBoard.BOARD_WIDTH || y < 0 || y >= board.longestHight() || evalMemo[x,y])
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

            bool right = DFS_Orb_eval(evalMemo, orbsToSet, x + 1, y, type);
            bool down = DFS_Orb_eval(evalMemo, orbsToSet, x, y + 1, type);
            bool up = DFS_Orb_eval(evalMemo, orbsToSet, x - 1, y, type);
            bool left = DFS_Orb_eval(evalMemo, orbsToSet, x, y - 1, type);

            return (center || right || down || up || left);
        }
    }

    public void restLineDropTimer()
    {
        linedropTimer = DROP_TIMER_MAX - (Mathf.Pow(Mathf.Log10(score.level),2)); // look at this on demose if you want to know why this is the alg
    }

    public float getMaxOrbDropTimer()
    {
        return DROP_TIMER_MAX - (Mathf.Log10(score.level)*3);
    }
}
