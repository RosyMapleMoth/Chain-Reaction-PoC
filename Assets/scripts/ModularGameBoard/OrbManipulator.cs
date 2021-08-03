using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbManipulator : MonoBehaviour
{
    public GameBoard board;

    public GameStateManger manager;
    public Stack<GameObject> heldOrbs;
    public OrbType curHeldType;
    public Transform GrabbedOrbs;
    public Transform DroppingOrbs;
    public int OrbsBeingPickedUp = 0;
    public int OrbsBeingDropped = 0;


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
        if (board.GetColSize(col) > 0)
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


        IEnumerator PickUp()
        {
            while (elapsedTime <= waitTime)
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
            
            OrbsBeingPickedUp--;
            if (OrbsBeingPickedUp == 0)
            {
                manager.FinishManipulatingOrb();
            }
            moving.transform.localPosition = new Vector3(0F, heldOrbs.Count, 0F);
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
            DroppingOrbs.position = board.getRelativeOragin() + new Vector3(col + col*GameBoard.X_OFF_SET, -GameBoard.BOARD_HIGHT,0f);
            while (heldOrbs.Count > 0)
            {
                GameObject temp = heldOrbs.Pop();
                Debug.Log("starting to drop " + temp.name);
                temp.transform.SetParent(DroppingOrbs);
                temp.transform.localPosition = new Vector3(0,OrbsBeingDropped - (GameBoard.Y_OFF_SET) *(heldOrbs.Count),0);
                OrbsBeingDropped++;
            }
            DropOrb(DroppingOrbs, col, 0.25f - 0.01f * board.GetColSize( col ));
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
                Vector3 endPosition = board.getRelativeOragin() +new Vector3(col + (GameBoard.X_OFF_SET), - board.GetColSize(col) - moving.childCount,0f);
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
                Debug.Log("all dropping orbs are placed");
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

