﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class OrbManipulator : MonoBehaviour
{
    public GameBoard board;
    public UnityEvent DroppingOrbsEvt;
    public UnityEvent GrabbingOrbsEvt;
    public GameStateManger manager;
    public Stack<GameObject> heldOrbs;
    public OrbType curHeldType;
    public int DropRow =-1;
    public Transform GrabbedOrbs;
    public Transform DroppingOrbs;
    public int OrbsBeingPickedUp = 0;
    public int OrbsBeingDropped = 0;

    public float shotspeed = 0.025f;


    // Start is called before the first frame update
    void Start()
    {
        heldOrbs = new Stack<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Attmepts to pick up an orb if sucsuesful stores 
    /// </summary>
    /// <param name="col"></param>
    public void AttmeptGrabOrbs(int col)
    {
        if (board.GetColSize(col) > 0 && manager.CanManipulateOrb())
        {
            if (heldOrbs.Count == 0 && board.CanPickUp(col))
            {
                curHeldType = board.GetOrbType(col);
                heldOrbs.Push(board.GrabOrb(col));
                PickUpOrb(heldOrbs.Peek().transform);
                manager.ManipulatingOrb();
            }

            // this is an if and a while effectivly
            while (board.GetColSize(col) > 0 && board.GetOrbType(col) == curHeldType && board.CanPickUp(col))
            {
                heldOrbs.Push(board.GrabOrb(col));
                PickUpOrb(heldOrbs.Peek().transform);
                manager.ManipulatingOrb();
            }
        }
        // handle in air orbs
        else if (DropRow == col && DroppingOrbs.childCount > 0)
        {
            GrabbingOrbsEvt.Invoke();
            curHeldType = DroppingOrbs.GetComponentInChildren<Orb>().orbScript.orbType;
            heldOrbs.Push(DroppingOrbs.GetChild(0).gameObject);
            DroppingOrbs.GetChild(0).SetParent(null);
            PickUpOrb(heldOrbs.Peek().transform);
            manager.ManipulatingOrb();

            while (DroppingOrbs.childCount > 0)
            {
                heldOrbs.Push(DroppingOrbs.GetChild(0).gameObject);
                DroppingOrbs.GetChild(0).SetParent(null);
                PickUpOrb(heldOrbs.Peek().transform);
                manager.ManipulatingOrb();
            }
        }
    }


    /// <summary>
    /// Moves the orb from its current position down off the board.   
    /// XXX this function is very specific and could be made more modular if need be in the future
    /// </summary>
    /// <param name="moving">pass in the transform of the orb to be moved from its current position to the bottom </param>
    private void PickUpOrb(Transform moving)
    {
        OrbsBeingPickedUp++;
        Vector3 currentPos = moving.position;
        Vector3 EndPos = new Vector3(moving.position.x,
                                     board.getRelativeOragin().y - GameBoard.BOARD_HIGHT,
                                     moving.position.z);
        
        Debug.Log("Jumping orb from " + currentPos + " to " +EndPos );
        float elapsedTime = 0f, waitTime = 0.20f - 0.01f * (GameBoard.SPAWN_Y_Val - moving.position.y);
        moving.SetParent(GrabbedOrbs);
        bool earlyEnd = false;
        void earlyEndInvoke()
        {
            earlyEnd = true;
        }
        IEnumerator PickUp()
        {
            DroppingOrbsEvt.AddListener(earlyEndInvoke);

            while (elapsedTime <= waitTime && earlyEnd == false)
            {
                try
                {
                    moving.transform.position = Vector3.Lerp(currentPos,
                                                             EndPos,
                                                             Mathf.Clamp((elapsedTime / waitTime), 0, 1));
                }
                catch
                {

                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            DroppingOrbsEvt.RemoveListener(earlyEndInvoke);
            OrbsBeingPickedUp--;
            if (OrbsBeingPickedUp <= 0 && earlyEnd == false)
            {
                manager.FinishManipulatingOrb();
            }
            if (earlyEnd == false)
            {
                moving.transform.localPosition = new Vector3(0F, heldOrbs.Count, 0F);
            }
            yield return null;
        }
    StartCoroutine(PickUp());
    }


    /// <summary>
    /// Drops orbs if held, dropped orbs will animate from the potion to the baord, and then will be added to the board
    /// </summary>
    /// <param name="col">the collum to drop orbs on</param>
    public void AttemptDropOrbs(int col)
    {
        if (heldOrbs.Count > 0)
        {
            manager.ManipulatingOrb();
            DroppingOrbsEvt.Invoke();
            DropRow = col;
            DroppingOrbs.position = board.getRelativeOragin() + new Vector3(col + col*GameBoard.X_OFF_SET, -GameBoard.BOARD_HIGHT-3,0f);
            while (heldOrbs.Count > 0)
            {
                GameObject temp = heldOrbs.Pop();
                Debug.Log("starting to drop " + temp.name);
                temp.transform.SetParent(DroppingOrbs);
                temp.transform.localPosition = new Vector3(0,OrbsBeingDropped - (GameBoard.Y_OFF_SET) *(heldOrbs.Count),0);
                OrbsBeingDropped++;
            }
            /// BOARD_HIGHT - col size to get total squars away from shooter then multipler by shot speed
            DropOrb(DroppingOrbs, col, shotspeed * (GameBoard.BOARD_HIGHT - board.GetColSize( col )),board.GetlastOrbInCol(col));
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="moving"></param>
    /// <param name="col"></param>
    public void DropOrb(Transform moving, int col, float dropTime)
    {
        IEnumerator MoveToSpot()
            { 
                float elapsedTime = 0; // set counter to 0
                float waitTime = dropTime; // set total wait time based on how far 
                Vector3 startPosition = moving.position;
                Vector3 endPosition = board.getRelativeOragin() + new Vector3(col + (GameBoard.X_OFF_SET), - board.GetColSize(col) - moving.childCount,0f);
                Debug.Log(endPosition);
                while (elapsedTime < waitTime)
                {


                    try
                    {
                        if (board.getRelativeOragin() + new Vector3(col, - board.GetColSize(col) - moving.childCount,0f) != endPosition)
                        {
                            endPosition = board.getRelativeOragin() +  new Vector3(col + (GameBoard.X_OFF_SET * col), -  board.GetColSize(col) - moving.childCount -(GameBoard.Y_OFF_SET *Mathf.Abs(board.GetColSize(col))) - GameBoard.Y_OFF_SET,0f);
                        }
                        moving.position = Vector3.Lerp( startPosition, 
                                                        endPosition,
                                                        Mathf.Clamp((elapsedTime / waitTime), 0, 1));
                    }
                    catch
                    {
                        Debug.Log("FUCK IT hAPPEND");
                    }
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                // Make sure we got there
                try
                {
                    // Snap to proper position if we somehow ended with out hitting the correct position
                    moving.position = Vector3.Lerp( startPosition, 
                                                    endPosition,
                                                    1);
                }
                catch
                {
                    // TODO add wait timer for a small amount of time before reechecking
                }

                
                manager.RequestPutOrb(col, moving.transform);
                OrbsBeingDropped = 0;
                if (OrbsBeingDropped == 0)
                {
                    
                }
                Debug.Log("all dropping orbs are placed");
                yield return null;
            }

            StartCoroutine(MoveToSpot());
    }



    public void DropOrb(Transform moving, int col, float dropTime, Transform EndPos)
    {
        bool earlyEnd = false;
        void earlyEndInvoke()
        {
            earlyEnd = true;
        }
        IEnumerator MoveToSpot()
            { 
                GrabbingOrbsEvt.AddListener(earlyEndInvoke);
                float elapsedTime = 0; // set counter to 0
                float waitTime = dropTime; // set total wait time based on how far 
                Vector3 startPosition = moving.position;
                Debug.Log("DEBUG: start Position "  +startPosition);
                while (elapsedTime < waitTime && !earlyEnd)
                {
                    moving.position = Vector3.Lerp( startPosition, 
                                                    board.GetlastOrbInCol(col).position - new Vector3(0,moving.childCount+ 0.125f,0),
                                                    Mathf.Clamp((elapsedTime / waitTime), 0, 1));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                
                if (!earlyEnd)
                {
                    // Snap to proper position if we somehow ended with out hitting the correct position
                    moving.position = Vector3.Lerp( startPosition, 
                                                    board.GetlastOrbInCol(col).position - new Vector3(0,moving.childCount+ 0.125f,0),
                                                    1);
                    Debug.Log("DEBUG: Final startPosition "  + startPosition);
                    manager.RequestPutOrb(col, moving.transform);
                    OrbsBeingDropped = 0;
                }
                else
                {
                    /// Free up whatever is blocking
                    OrbsBeingDropped = 0;
                }
                DropRow = -1;
                GrabbingOrbsEvt.RemoveListener(earlyEndInvoke);
                yield return null;
            }

            StartCoroutine(MoveToSpot());
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool IsGrabAvilable()
    {
        return OrbsBeingPickedUp == 0;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool IsDropAvilable()
    {
        return OrbsBeingDropped == 0;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool CanAct()
    {
        return IsDropAvilable() && IsGrabAvilable();
    }

}

