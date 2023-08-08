using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
public class BoardManager : MonoBehaviour
{

    public UnityEvent GameOver = new UnityEvent();
    private const float POPTIMERMAX = 1;
    private const float FALLTIMEMAX = 0.25f;
    private const int MAXORBS = 12;

    // off set's to fix orbs being center pivited
    public const float X_OFF_SET = 0.5f;
    public const float Y_OFF_SET = -0.5f;


    private float timeUntilDrop = 0f;
    private int incomingLines = 1;
    private int reserveLines = 0;
    public Pattern[] StartPatt;
    public GameObject orbPrefab;
    public GameObject orbs;
    public GameObject DebugUI;
    static private int SPAWN_Y_Val = 7;
    static private int SPAWN_X_VAL = -4;
    public GameObject switcher;
    public GameObject GameOverUI;
    public int HeldObrs = 0;
    public OrbType heldType = OrbType.ERROR;
    public LinkedList<GameObject>[] Cols;
    public LinkedList<GameObject>[] OobCols;
    public Sprite[] pickers;
    public float fallenPopConter = - FALLTIMEMAX;
    private Queue<GameObject> toPopOrbs;
    private LinkedList<GameObject> fallenOrbs;
    public Pattern[] PatternList;
    public Text countdown;
    public Text StartCountDown;
    public Transform GrabbedOrbs;
    public int orbIDNext;
    public static int ORB_VIEW_LAYER = -3;
    private Queue<Pattern> dropQue;
    public bool CurrentlyPoping;
    public float ChainTimer;
    public float DropSpeed;
    public float PopTimer = POPTIMERMAX;
    public bool DebugMode;
    public GameState curstate;
    public enum GameState {starting,puase,playing,over};
    public float StartingUp;
    public bool isGameOver = false;
    public scoreTMP playerScore;

    // This keeps track of orbs currently being grabbed,
    // we should not be able to shoot orbs until Orbs Beinggrabed is zero
    public int OrbsBeingGrabed;


    private int level = 0;


    
    



    /// <summary>
    /// set up Col's
    /// </summary>
    void Start()
    {
        orbIDNext = 0;
        Cols = new LinkedList<GameObject>[7];
        for (int i = 0; i < 7; i++)
        {
            Cols[i] = new LinkedList<GameObject>();
        }


        OobCols = new LinkedList<GameObject>[7];
        for (int i = 0; i < 7; i++)
        {
            OobCols[i] = new LinkedList<GameObject>();
        }


        dropQue = new Queue<Pattern>();

        for (int i = 0; i < StartPatt.Length; i++)
        {
            dropQue.Enqueue(StartPatt[i]);
        }

        MakePattern(dropQue.Dequeue());

        timeUntilDrop = 0f;
        fallenOrbs = new LinkedList<GameObject>();
        toPopOrbs = new Queue<GameObject>();
        CurrentlyPoping = false;
        StartingUp = 1f;
        OrbsBeingGrabed = 0;
    }



    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        switch (curstate)
        {
            case GameState.starting:
                startingUpdate();
                break;
            case GameState.playing:
                playingUpdate();
                break;
            case GameState.puase:
                break;
            case GameState.over:
                break;
        }
    }



    public void startingUpdate()
    {
        if (StartingUp > 0)
        {
            StartCountDown.text = Mathf.Ceil(StartingUp).ToString();
        }
        else if (StartingUp > -1)
        {
            StartCountDown.text = "GO !";
        }
        else
        {
            StartCountDown.gameObject.SetActive(false);
            curstate = GameState.playing;
        }
        StartingUp -= Time.deltaTime;

    }




    public void playingUpdate()
    {
        // Generate reserve Orbs
        if (reserveLines < incomingLines)
        {
            while (reserveLines < incomingLines)
            {
                addPatternToQue();
                MakePattern(dropQue.Dequeue());
            }
        }

        // calculation of when board should update
        timeUntilDrop -= Time.deltaTime;
        if (timeUntilDrop <= 0 && !CurrentlyPoping)
        {
            Debug.unityLogger.Log("Genreal", "Lines Moving down start");
            DropLine(1);
            timeUntilDrop = DropSpeed - (playerScore.level-1)*(0.25f);
            Debug.unityLogger.Log("Genreal", "Lines Moving down end");
        }


        // How we handel falling orbs
        if (fallenPopConter > 0)
        {
            fallenPopConter -= Time.deltaTime;

        }
        else if (fallenPopConter > -1 && fallenPopConter <= 0)
        {
            if (!CurrentlyPoping)
            {
                Debug.Log("evaluating orbs from drop");
                evaluateOrbs();
            }
            fallenPopConter = -2;
        }


        // if we are counting down to a pop'
        if(CurrentlyPoping)
        {
            if (PopTimer > 0)
            {
                PopTimer -= Time.deltaTime;        
            }
            else 
            {
                CurrentlyPoping = false;
                endPopingOrbs();
                checkForFalling();
                evaluateOrbs();
                playerScore.finializePop();
                if (!CurrentlyPoping)
                {
                    playerScore.resetChain();
                }
                else
                {
                    playerScore.incressChain();
                }
                
                PopTimer = POPTIMERMAX;
            }
        }
        if (DebugMode)
        {
            DebugUI.SetActive(true);
            // the countdown text 
            countdown.text = timeUntilDrop.ToString("F2");
        }
        else if (DebugMode)
        {
            DebugUI.SetActive(false);
        }

        foreach (LinkedList<GameObject> i in Cols)
        {
            if (i.Count >= MAXORBS && !CurrentlyPoping)
            {
                endGame();
                break;
            }
        }
    }


    public void levelUp()
    {
        level++;
    }


    public void endGame()
    {
        GameOver.Invoke();
        isGameOver = true;
        curstate = GameState.over;


    }


    private void evaluateOrbs()
    {
        Debug.unityLogger.Log("Orb Eval", "Starting");
        foreach (LinkedList<GameObject> Col in Cols) 
        {
            foreach (GameObject orb in Col) 
            {
                Orb temp = orb.GetComponent<Orb>();
                if (temp.curState == Orb.OrbState.Evaluating)
                {
                    if (VertThree(temp,temp.GetOrbType()))
                    {
                        Debug.unityLogger.Log("Orb Eval", temp.name + " is being set ToPop");
                        EvaluateOrbWoQue(temp,temp.GetOrbType(),Orb.OrbState.ToPop);
                    } 
                    else
                    {
                        Debug.unityLogger.Log("Orb Eval", temp.name + " is being set Resting");
                        temp.curState = Orb.OrbState.Resting;
                    }
                }
            } 
        }
        Debug.unityLogger.Log("Orb Eval", "Ending");
        startPopingOrbs();
    }

    private void startPopingOrbs()
    {
        Debug.unityLogger.Log("Start Orb Pop", "Starting");
        // check all movied 
        foreach (LinkedList<GameObject> Col in Cols) 
        {
            foreach (GameObject orb in Col) 
            {
                Orb temporb = orb.GetComponent<Orb>();
                if (temporb.curState == Orb.OrbState.ToPop)
                {
                    playerScore.scoreBlock();
                    Debug.unityLogger.Log("Start Orb Pop", temporb + " has begun To Pop");
                    orb.GetComponentInChildren<Animator>().SetTrigger("pop");
                    temporb.curState = Orb.OrbState.Poping;
                    CurrentlyPoping = true;
                }
            } 
        }
        Debug.unityLogger.Log("Start Orb Pop", "Ending");
    }

    private void endPopingOrbs()
    {
        Debug.unityLogger.Log("End Orb Pop", "Starting");
        foreach (LinkedList<GameObject> Col in Cols) 
        {
            foreach (GameObject orb in Col.ToList()) 
            {
                Orb temporb = orb.GetComponent<Orb>();
                Debug.unityLogger.Log("End Orb Pop", "Orb " + temporb.name + " is being checked");
                if (temporb.curState == Orb.OrbState.Poping)
                {
                    Col.Remove(temporb.gameObject);
                    Destroy(temporb.gameObject);
                    Debug.unityLogger.Log("End Orb Pop", "Orb " + temporb.name + " has been Popped");

                }
            } 
        }
        Debug.unityLogger.Log("End Orb Pop", "Ending");
    }


    /// <summary>
    /// checks if their exists 3 orbs vertically of the same color connected to the current orb
    /// </summary>
    /// <param name="orb"> the starting orb to check vertically up and down from</param>
    /// <param name="color">the color of the orb we are checking</param>
    /// <returns></returns>
    private bool VertThree(Orb orb, OrbType color)
    {
        GameObject curNode = orb.gameObject;
        GameObject lastNode = curNode;

        int VertCount = 0;

        // how many orbs are vertically in a row down from starting orb
        while (curNode != null && curNode.GetComponent<Orb>().GetOrbType() == color && curNode.GetComponent<Orb>().curState != Orb.OrbState.Falling)
        {
            lastNode = curNode;
            VertCount++;
            Vector2 temp = curNode.GetComponent<Orb>().GetRelPos();
            Debug.Log("checking for cur node at location " + temp);
            curNode = GetOrbAtRelPos((int)temp.x, (int)temp.y + 1);
        }

        // if we have already hit 3 vertical return ture
        if (VertCount >= 3)
        {
            return true;
        }
        else // other wise how many orbs exist vertically up from the farest down orb 
        {
            VertCount = 0;
            curNode = lastNode;
            while (curNode != null && curNode.GetComponent<Orb>().GetOrbType() == color && curNode.GetComponent<Orb>().curState != Orb.OrbState.Falling)
            {
                lastNode = curNode;
                VertCount++;
                Vector2 temp = curNode.GetComponent<Orb>().GetRelPos();
                Debug.Log("checking for cur node at location " + temp);

                curNode = GetOrbAtRelPos((int)temp.x, (int)temp.y - 1);
                Debug.Log("node is " + curNode);
            }
        }
        return (VertCount >= 3);
    }





    /// <summary>
    /// TODO : Review this function
    /// </summary>
    private void checkForFalling()
    {
        for (int i = 0; i <= 6; i++)
        {
            LinkedListNode<GameObject> node = Cols[i].First;
            if (node != null)
            {
                int depth = 0;
                checkNode(node.Value, OobCols[i].Last.Value, depth, i);


                while (node.Next != null)
                {
                    depth++;
                    node = node.Next;
                    checkNode(node.Value, node.Previous.Value, depth, i);
                }
            }
        }

        if (fallenOrbs.Count > 0)
        {
            fallenPopConter = 0.12f;
        }
    }








    /// <summary>
    /// Checks a single orb and insure it's relative position is exctly one unit below ancor in the y axie,
    /// if this is not ture it will move orb to relative board position immedidly below ancor; 
    /// </summary>
    /// <param name="Orb">the game object of the orb to be evaluated</param>
    /// <param name="ancor">The position to position derectly below: the last orb to be in the correct relative position in the same row</param>
    private void checkNode(GameObject orb, GameObject ancor, int depth, int Line)
    {
        if (Mathf.Ceil(orb.transform.position.y) != Mathf.Ceil(ancor.transform.position.y) - 1 
         || ancor.GetComponent<Orb>().curState == Orb.OrbState.Falling)
        {
            orb.GetComponent<Orb>().curState = Orb.OrbState.Falling;
            
            Vector3 start = orb.transform.position;
            orb.transform.position = ancor.transform.position + new Vector3 (0, -1 ,0 );

            IEnumerator Fall()
            {
                float elapsedTime = 0;
                float waitTime = 0.1f;
                orb.transform.GetChild(0).position = new Vector3(start.x + X_OFF_SET, start.y + Y_OFF_SET,start.z);;
                Vector3 currentPos = orb.transform.GetChild(0).position;
                

                while (elapsedTime < waitTime)
                {
                    Debug.unityLogger.Log("Fall(checkNode)" + orb.name, "remaining time " + (waitTime - elapsedTime));

                    try
                    {
                        orb.transform.GetChild(0).position = Vector3.Lerp(currentPos,
                                                              new Vector3(currentPos.x,
                                                                           orb.transform.position.y + Y_OFF_SET,
                                                                           currentPos.z),
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
                    orb.transform.GetChild(0).position = Vector3.Lerp(currentPos,
                                                              new Vector3(currentPos.x,
                                                                           orb.transform.position.y + Y_OFF_SET,
                                                                           currentPos.z),
                                                             1);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (orb.GetComponent<Orb>().curState != Orb.OrbState.Poping)
                {
                    orb.GetComponent<Orb>().curState = Orb.OrbState.Evaluating;
                }
                Debug.unityLogger.Log("Fall(checkNode)" + orb.name, "ending fall for " + orb.name);
                yield return null;
            }

            Debug.unityLogger.Log("Fall(checkNode)" + orb.name, "Starting for orb " + orb.name);
            StartCoroutine(Fall());
            if (fallenPopConter <= 0)
            {
                fallenPopConter = 0.14f;
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public void addPatternToQue()
    {
        int pattern = (int)Mathf.Floor(UnityEngine.Random.Range(0, PatternList.Length));
        dropQue.Enqueue(PatternList[pattern]);
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupingToEvaluate"></param>
    /// <returns></returns>
    private bool popCondition(Queue<GameObject> groupingToEvaluate)
    {
        Queue<GameObject> GroupHolder = new Queue<GameObject>(groupingToEvaluate);
        bool output;

        while (GroupHolder.Count > 0)
        {
            GameObject OrbToCheck = GroupHolder.Dequeue();
            output = VertThree(OrbToCheck.GetComponent<Orb>(), OrbToCheck.GetComponent<Orb>().GetOrbType());
            if (output)
            {
                return output;
            }
            
        }
        return false;
    }


    







    /// <summary>
    /// 
    /// </summary>
    /// <param name="patt"></param>
    private void MakePattern(Pattern patt)
    {
        for (int c = 0; c < patt.lines.Length; c++)
        {
            for (int r = 0; r < patt.lines[c].orbs.Length; r++)
            {
                GameObject temp;

                temp = Instantiate(orbPrefab, new Vector3(transform.position.x + SPAWN_X_VAL + r, transform.position.y + SPAWN_Y_Val +  reserveLines, ORB_VIEW_LAYER), Quaternion.identity);
                temp.name = "orb " + orbIDNext.ToString();
                temp.transform.GetChild(1).GetComponentInChildren<Text>().text = temp.name;
                orbIDNext = orbIDNext+1;
                temp.GetComponent<Orb>().LoadOrb(patt.lines[c].orbs[r]);
                temp.transform.SetParent(orbs.transform);
                OobCols[r].AddFirst(temp);
            }
            reserveLines += 1;
        }
    }








    /// <summary>
    /// 
    /// </summary>
    /// <param name="numLines"></param>
    private void DropLine(int numLines)
    {
        try
        {
            for (int i = 0; i < numLines; i++)
            {
                orbs.transform.position = new Vector3(orbs.transform.position.x, orbs.transform.position.y - 1, ORB_VIEW_LAYER);
                for (int c = 0; c < 7; c++)
                {
                    Cols[c].AddFirst(OobCols[c].Last.Value);
                    OobCols[c].RemoveLast();
                }
            }
            reserveLines -= numLines;
        }
        catch (Exception e)
        {
            Debug.LogError("Error Droping line. insure pattern pool is not empty. fucntion halted with exception : " + e);
        }
    }






    /// <summary>
    /// Drops held orbs on to the board
    /// </summary>
    /// <param name="Line"></param>
    public void DropOrbs(int Line)
    {
        bool LastOne = true;
        int initchildcount = GrabbedOrbs.childCount;
        while (GrabbedOrbs.childCount > 0)
        {
            GameObject ancorOrb;
            if (Cols[Line].Count > 0)
            {
                ancorOrb = Cols[Line].Last.Value;
            }
            else
            {
                ancorOrb = OobCols[Line].Last.Value;
            }

           

            Transform moveingOrb = GrabbedOrbs.GetChild(GrabbedOrbs.childCount-1);
            Debug.Log("Dropping " +  GrabbedOrbs.GetChild(GrabbedOrbs.childCount-1).ToString());
            Vector3 start = moveingOrb.position;
            moveingOrb.position = new Vector3(ancorOrb.transform.position.x, ancorOrb.transform.position.y - 1, ORB_VIEW_LAYER);
            moveingOrb.GetChild(0).position = start;
            moveingOrb.GetComponent<Orb>().curState = Orb.OrbState.Falling;
            moveingOrb.SetParent(orbs.transform);
            Cols[Line].AddLast(moveingOrb.gameObject);

            HeldObrs--;

            if (GrabbedOrbs.childCount == initchildcount)
            {
                LastOne = true;
            }
            IEnumerator MoveToSpot()
            {
                
                float elapsedTime = 0;
                float waitTime = 0.25f - 0.01f*Cols[Line].Count;
                Transform moving = moveingOrb.GetChild(0);
                moving.position = new Vector3(moveingOrb.transform.position.x + X_OFF_SET,-5 + Y_OFF_SET,0);
                Vector3 currentPos = moving.position;
                
                while (elapsedTime < waitTime)
                {


                    try
                    {
                        moving.position = Vector3.Lerp(currentPos,
                                                            new Vector3(moveingOrb.transform.position.x + X_OFF_SET,
                                                                        moveingOrb.position.y + Y_OFF_SET,
                                                                        moveingOrb.transform.position.z),
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
                    moving.position = Vector3.Lerp(currentPos,
                                                            new Vector3(moveingOrb.transform.position.x + X_OFF_SET,
                                                                        moveingOrb.position.y + Y_OFF_SET,
                                                                        moveingOrb.transform.position.z),
                                                            1);
                }
                catch
                {
                    // TODO add wait timer for a small amount of time before reechecking
                }

                moveingOrb.GetComponent<Orb>().curState = Orb.OrbState.Evaluating;
                if (!CurrentlyPoping)
                {
                    
                }
                Debug.Log("Coroutine is done");
                yield return null;
            }

            StartCoroutine(MoveToSpot());
        }
        if (fallenPopConter <= 0)
        {
            fallenPopConter = 0.26f;
        }
        HeldObrs =0;
        heldType = OrbType.ERROR;
        CheckPickerType();       
    }

    

    





    /// <summary>
    /// 
    /// </summary>
    /// <param name="orb"></param>
    private void StoreOrb(GameObject orb)
    {

            OrbsBeingGrabed++;
            Transform moving = orb.transform;
            Vector3 currentPos = moving.position;
            Debug.Log("picking up " + orb.name + " from position : " + moving.position.ToString());
            Vector3 EndPos = new Vector3(moving.position.x,
                                         OobCols[(int)Mathf.Floor(moving.localPosition.x)].Last.Value.transform.position.y - 13,
                                         moving.position.z);
            float elapsedTime = 0f, waitTime = 0.20f - 0.01f*Cols[(int)Mathf.Floor(moving.localPosition.x)].Count;
            moving.SetParent(GrabbedOrbs);
            HeldObrs++;

            

        IEnumerator PickUp()
        {
            while (elapsedTime <= waitTime)
            {
                try
                {
                    moving.position = Vector3.Lerp(currentPos,
                                                        EndPos,
                                                        Mathf.Clamp((elapsedTime / waitTime), 0, 1));
                }
                catch
                {

                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            moving.localPosition = new Vector3(0F, HeldObrs, 0F);
            OrbsBeingGrabed--;
            yield return null;
        }

        StartCoroutine(PickUp());


    }







    /// <summary>
    /// 
    /// </summary>
    /// <param name="Line"></param>
    public void AttemptGrabOrb(int Line)
    {
        if (Cols[Line].Count > 0 
        && ( Cols[Line].Last.Value.GetComponent<Orb>().curState == Orb.OrbState.Resting
        || Cols[Line].Last.Value.GetComponent<Orb>().curState == Orb.OrbState.Evaluating ) )
        {
            if (HeldObrs == 0)
            {
                GameObject temp = Cols[Line].Last.Value;

                Debug.Log("Pick Up : Picking up first orb " + temp.name);
                Cols[Line].RemoveLast();
                heldType = temp.GetComponent<Orb>().GetOrbType();
                StoreOrb(temp);
                CheckPickerType();
                AttemptGrabOrb(Line);
            }
            else if (Cols[Line].Last.Value.GetComponent<Orb>().checkOrbType(heldType))
            {
                Debug.Log("Pick Up : Picking up additional orb " + Cols[Line].Last.Value.name);
                StoreOrb(Cols[Line].Last.Value);
                Cols[Line].RemoveLast();
                AttemptGrabOrb(Line);
            
            }
        }
    }







    /// <summary>
    /// Checks the currently held orb and changs the player cursers sprite accordingly
    /// If run while not holding an orb the picker will be switched back to defult empty sprite 
    /// TODO : move this to Selector script
    /// </summary>
    private void CheckPickerType()
    {
        switch (heldType)
        {
            case OrbType.Green:
                switcher.GetComponentInChildren<SpriteRenderer>().sprite = pickers[5];
                break;
            case OrbType.Blue:
                switcher.GetComponentInChildren<SpriteRenderer>().sprite = pickers[3];
                break;
            case OrbType.Ornage:
                switcher.GetComponentInChildren<SpriteRenderer>().sprite = pickers[1];
                break;
            case OrbType.Red:
                switcher.GetComponentInChildren<SpriteRenderer>().sprite = pickers[6];
                break;
            case OrbType.Pink:
                switcher.GetComponentInChildren<SpriteRenderer>().sprite = pickers[2];
                break;
            case OrbType.Purple:
                switcher.GetComponentInChildren<SpriteRenderer>().sprite = pickers[4];
                break;
            case OrbType.ER:
                break;
            case OrbType.SR:
                break;
            case OrbType.BullshitPowerUp:
                break;
            case OrbType.ERROR:
                switcher.GetComponentInChildren<SpriteRenderer>().sprite = pickers[0];
                break;
        }
    }



    
    
    
    
    
    
    
    
    
    /// <summary>
    /// Fills ReadyToPop with all touching orbs of type colo. all orbs already marked popping will be ignored.
    /// XXX consider refactoring to remove ReadyToPop and replace it with a private var at class level.
    /// </summary>
    /// <param name="ReadyToPop">Where all touching orbs that are the same type as color will be stored</param>
    /// <param name="curOrb">The current orb to evaluate</param>
    /// <param name="color">the color of orbs we are looking for</param>
    /// <returns></returns>
    public bool EvaluateOrb(Queue<GameObject> ReadyToPop, Orb curOrb, OrbType color )
    {
        Debug.Log("orb " + curOrb.gameObject.name + " is attempting a pop check");
        if (color == curOrb.orbScript.orbType && curOrb.curState != Orb.OrbState.Evaluating && curOrb.curState != Orb.OrbState.Falling)
        {
            Debug.Log("orb " + curOrb.gameObject.name + "has passed the pop check ");

            ReadyToPop.Enqueue(curOrb.gameObject);
            curOrb.curState = Orb.OrbState.Evaluating;


            Vector2 temp = curOrb.GetRelPos();


            LinkedListNode<GameObject> node = Cols[(int)temp.x].First;


            for (int i = 0; i < (int)temp.y; i++)
            {
                node = node.Next;

            }
            if ((int)temp.y < (Cols[(int)temp.x].Count - 1))
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking down");
                EvaluateOrb(ReadyToPop, node.Next.Value.GetComponent<Orb>(), color);
            }
            if ((int)temp.y > 0)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking up");
                EvaluateOrb(ReadyToPop, node.Previous.Value.GetComponent<Orb>(), color);
            }
            if ((int)temp.x <  6 && Cols[(int)temp.x + 1].Count > (int)temp.y)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking right");
                node = Cols[(int)temp.x + 1].First;
                for (int i = 0; i < (int)temp.y; i++)
                {
                    node = node.Next;
                }
                EvaluateOrb(ReadyToPop, node.Value.GetComponent<Orb>(), color);
            }
            if ((int)temp.x >= 1 && Cols[(int)temp.x - 1].Count > (int)temp.y)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking left");
                node = Cols[(int)temp.x - 1].First;
                for (int i = 0; i < (int)temp.y; i++)
                {
                    node = node.Next;
                }
                EvaluateOrb(ReadyToPop, node.Value.GetComponent<Orb>(), color);
            }
        }
        return false;
    }




    public bool EvaluateOrbWoQue( Orb curOrb, OrbType color, Orb.OrbState CheckMakeType)
    {
        Debug.Log("orb " + curOrb.gameObject.name + " is attempting a pop check");
        if (color == curOrb.orbScript.orbType && curOrb.curState != CheckMakeType && curOrb.curState != Orb.OrbState.Falling)
        {
            Debug.Log("orb " + curOrb.gameObject.name + "has passed the pop check ");

            curOrb.curState = CheckMakeType;


            Vector2 temp = curOrb.GetRelPos();


            LinkedListNode<GameObject> node = Cols[(int)temp.x].First;


            for (int i = 0; i < (int)temp.y; i++)
            {
                node = node.Next;

            }
            if ((int)temp.y < (Cols[(int)temp.x].Count - 1))
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking down");
                EvaluateOrbWoQue(node.Next.Value.GetComponent<Orb>(), color,CheckMakeType);
            }
            if ((int)temp.y > 0)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking up");
                EvaluateOrbWoQue(node.Previous.Value.GetComponent<Orb>(), color,CheckMakeType);
            }
            if ((int)temp.x <  6 && Cols[(int)temp.x + 1].Count > (int)temp.y)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking right");
                node = Cols[(int)temp.x + 1].First;
                for (int i = 0; i < (int)temp.y; i++)
                {
                    node = node.Next;
                }
                EvaluateOrbWoQue(node.Value.GetComponent<Orb>(), color,CheckMakeType);
            }
            if ((int)temp.x >= 1 && Cols[(int)temp.x - 1].Count > (int)temp.y)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking left");
                node = Cols[(int)temp.x - 1].First;
                for (int i = 0; i < (int)temp.y; i++)
                {
                    node = node.Next;
                }
                EvaluateOrbWoQue(node.Value.GetComponent<Orb>(), color,CheckMakeType);
            }
        }
        return false;
    }
    
    
    
    
    
    
    /// <summary>
    /// Finds the orb at the relative board position specifyed.
    /// </summary>
    /// <param name="x">the x of the orb in relative board position</param>
    /// <param name="y">the y of the orb in relative board position</param>
    /// <returns>the Orb stored at (x,y) in relative board position, if no orb exists returns null, if negative values are given return null</returns>
    private GameObject GetOrbAtRelPos(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return null;
        }

        try
        {
            if (Cols[x].Count > 0)
            {
                LinkedListNode<GameObject> node = Cols[x].First;
                for (int i = 0; i < y; i++)
                {
                    node = node.Next;
                }
                return node.Value;
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
}
    }