using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManger : MonoBehaviour
{
    public GameBoard board;
    public OrbManipulator orbTransport;
    public GameState curState;
    public GameState nextState;
    public enum GameState {idle, addLines, evaluate, popping, dropping};

    public static float POP_TIMER = 1.0f;
    public bool evalWhenReady = false;

    private bool grabWhenReady = false;
    private bool putWhenReady = false;


    public float popCountDown;
    private int grabCol = 0;
    private int putCol = 0;
    public bool addLineWhenReady = false;


    // Start is called before the first frame update
    void Start()
    {
        curState = GameState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabWhenReady)
        {
            orbTransport.AttmeptGrabOrbs(grabCol);
        }
        if (putWhenReady)
        {
            orbTransport.AttemptDropOrbs(putCol);
        }
        switch (curState)
        {
            case GameState.idle: IdleState(); break;
            case GameState.evaluate: EvaluateState(); break;
            case GameState.popping: PopState(); break;
            case GameState.dropping: DropState(); break;
            case GameState.addLines: AddLineState(); break;
        }
    }

    void LateUpdate()
    {
        curState = nextState;
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


    
    private void AddLineState()
    {
        
    }

    private void EvaluateState()
    {
        bool OrbsToPop = false;
        // Evaluate the board to see if any orbs meet the pop condition 
        if (OrbsToPop)
        {
            nextState = GameState.popping;
            popCountDown = POP_TIMER;
        }
        else 
        {
            nextState = GameState.idle;
        }
    }

    private void PopState()
    {
        popCountDown -= Time.deltaTime;

        if (popCountDown <= 0)
        { 
            // pop orbs
            
            //if (orbs need to drop)
            //     nextState = GameState.dropping
            //else
            //     nextState = GameState.idle
        }
        

    }

    private void DropState()
    {
        /*
            Drop timer
        
            when drop timer is over move to eval again
        */
    }

    private void IdleState()
    {
        if (evalWhenReady)
        {
            nextState = GameState.evaluate;
        }
        else 
        {
            // Kill chain
            if (addLineWhenReady)
            {
                nextState = GameState.addLines;
            }
        }
        
    }

    public void RequestEval()
    {
        evalWhenReady = true;
    }

    public void RequestGrab(int col)
    {
        grabWhenReady = true;
        grabCol = col;
    }


    public void dropEvalState()
    {

    }

    public void RequestPut(int col)
    {
        putWhenReady = true;
        putCol = col;
    }



}
