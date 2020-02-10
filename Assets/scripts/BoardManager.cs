using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    private float timeUntilDrop = 0f;
    private int incomingLines = 1;
    private int reserveLines = 0;
    public Pattern[] StartPatt;
    public GameObject orbPrefab;
    public GameObject orbs;
    static private int SPAWN_Y_Val = 7;
    static private int SPAWN_X_VAL = -4;
    public LineRenderer selectLine;

    public GameObject switcher;
    public int HeldObrs = 0;
    public OrbType heldType = OrbType.ERROR;
    public SpriteRenderer HeldOrbRep;

    public LinkedList<GameObject>[] Cols;
    public LinkedList<GameObject>[] OobCols;

    public Sprite[] pickers;
    public Queue<GameObject> toPopOrbs;
    public Queue<Orb> evaluateQue;


    public Text countdown;
    public Transform GrabbedOrbs;
    public int orbIDNext;

    public static int ORB_VIEW_LAYER = -3;

    private Queue<Pattern> dropQue;

    // Start is called before the first frame update
    void Start()
    {
        orbIDNext = 0;
        Cols = new LinkedList<GameObject>[7];
        for (int i = 0; i<7; i++)
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
        toPopOrbs = new Queue<GameObject>();
        evaluateQue = new Queue<Orb>();

    }


    // Update is called once per frame
    void Update()
    {
        if (reserveLines < incomingLines)
        {
            MakePattern(dropQue.Dequeue());
        }

        timeUntilDrop -= Time.deltaTime;
        if (timeUntilDrop <= 0)
        {
            Debug.Log("Lines Moving down");
            DropLine(1);
            timeUntilDrop = 5f;
        }
        countdown.text = timeUntilDrop.ToString("F2");
        updateSelectLineVert();
    
    }


    private bool VertThree(Orb orb, OrbType color, int vertCount)
    {



        return false;
    }

    private void vertThreeAux()
    {

    }
    private void checkForFalling()
    {
        for (int i = 0; i <= 6; i++)
        {
            LinkedListNode<GameObject> node = Cols[i].First;
            if (node != null)
            {
                checkNode(node.Value, OobCols[i].Last.Value.transform.position);


                while (node.Next != null)
                {
                    node = node.Next;
                    checkNode(node.Value, node.Previous.Value.transform.position);
                }
            }
        }
    }


    /// <summary>
    /// Checks a single orb and isure it's relative position is exctly one unit below ancor in the y axie,
    /// if this is not ture it will move orb to relative board position immedidly below ancor; 
    /// </summary>
    /// <param name="Orb">the game object of the orb to be evaluated</param>
    /// <param name="ancor">The position to position derectly below: the last orb to be in the correct relative position in the same row</param>
    private void checkNode(GameObject orb, Vector3 ancor)
    {
        Vector2 node_eval = orb.GetComponent<Orb>().GetRelPos();

        if (orb.transform.position.y != ancor.y - 1)
        {
            orb.transform.position = new Vector3(orb.transform.position.x, ancor.y - 1, ORB_VIEW_LAYER);
        }

    }



    private void MakePattern(Pattern patt)
    {
        for (int c = 0; c < patt.lines.Length; c++)
        {
            for (int r = 0; r < patt.lines[c].orbs.Length; r++)
            {
                GameObject temp;

                temp = Instantiate(orbPrefab, new Vector3(transform.position.x + SPAWN_X_VAL + r + .1f, transform.position.y + SPAWN_Y_Val +  reserveLines - 0.1f, ORB_VIEW_LAYER), Quaternion.identity);
                temp.name = "orb " + orbIDNext.ToString();
                temp.transform.GetChild(0).GetComponentInChildren<Text>().text = temp.name;
                orbIDNext = orbIDNext+1;
                temp.GetComponent<Orb>().LoadOrb(patt.lines[c].orbs[r]);
                temp.transform.SetParent(orbs.transform);
                OobCols[r].AddFirst(temp);
            }
            reserveLines += 1;
        }
    }


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





    public void DropOrbs(int Line)
    {
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


            Transform moveingOrb = GrabbedOrbs.GetChild(0);

            moveingOrb.position = new Vector3(ancorOrb.transform.position.x, ancorOrb.transform.position.y - 1, ORB_VIEW_LAYER);
            moveingOrb.SetParent(orbs.transform);

            Cols[Line].AddLast(moveingOrb.gameObject);

            HeldObrs--;

            if (HeldObrs == 0)
            {
                EvaluateOrb(toPopOrbs, moveingOrb.GetComponent<Orb>(), moveingOrb.GetComponent<Orb>().orbScript.orbType);
                if (toPopOrbs.Count >= 3)
                {
                    while (toPopOrbs.Count > 0)
                    {
                        toPopOrbs.Peek().SetActive(false);
                        Vector2 temp = toPopOrbs.Peek().GetComponent<Orb>().GetRelPos();
                        Cols[(int)temp.x].Remove(Cols[(int)temp.x].Find(toPopOrbs.Peek()));
                        Destroy(toPopOrbs.Dequeue());
                        checkForFalling();
                    }
                }
                else
                {
                    while (toPopOrbs.Count > 0)
                    {
                        toPopOrbs.Dequeue().GetComponent<Orb>().curState = Orb.OrbState.Resting;
                    }
                }
            }
        }

        heldType = OrbType.ERROR;
        CheckPickerType();        
    }


    public void CheckFall()
    {

    }


    private void StoreOrb(GameObject orb)
    {
        orb.transform.SetParent(GrabbedOrbs);
        orb.transform.localPosition = new Vector3(0F, HeldObrs, 0F);
        HeldObrs++;

    }


    public void AttemptGrabOrb(int Line)
    {
        if (Cols[Line].Count > 0 )
        {
            if (HeldObrs == 0)
            {
                GameObject temp = Cols[Line].Last.Value;
                Cols[Line].RemoveLast();
                heldType = temp.GetComponent<Orb>().GetOrbType();
                StoreOrb(temp);
                CheckPickerType();
                AttemptGrabOrb(Line);
            }

            else
            {
                if (Cols[Line].Last.Value.GetComponent<Orb>().checkOrbType(heldType))
                {
                    StoreOrb(Cols[Line].Last.Value);
                    Cols[Line].RemoveLast();
                    AttemptGrabOrb(Line);
                }
            }
        }
    }

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




    public bool EvaluateOrb(Queue<GameObject> localGroup, Orb curOrb, OrbType color )
    {
        Debug.Log("orb " + curOrb.gameObject.name + " is attempting a pop check");
        if (color == curOrb.orbScript.orbType && curOrb.curState != Orb.OrbState.Poping)
        {
            Debug.Log("orb " + curOrb.gameObject.name + "has passed the pop check ");

            localGroup.Enqueue(curOrb.gameObject);
            curOrb.curState = Orb.OrbState.Poping;


            Vector2 temp = curOrb.GetRelPos();


            LinkedListNode<GameObject> node = Cols[(int)temp.x].First;


            for (int i = 0; i < (int)temp.y; i++)
            {
                node = node.Next;

            }
            if ((int)temp.y < (Cols[(int)temp.x].Count - 1))
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking down");
                EvaluateOrb(localGroup, node.Next.Value.GetComponent<Orb>(), color);
            }
            else
            {
                VertThree(curOrb, color, 0);
            }



            if ((int)temp.y > 0)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking up");
                EvaluateOrb(localGroup, node.Previous.Value.GetComponent<Orb>(), color);
            }

            if ((int)temp.x <  6 && Cols[(int)temp.x + 1].Count > (int)temp.y)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking right");
                node = Cols[(int)temp.x + 1].First;
                for (int i = 0; i < (int)temp.y; i++)
                {
                    node = node.Next;
                }
                EvaluateOrb(localGroup, node.Value.GetComponent<Orb>(), color);
            }
            if ((int)temp.x >= 1 && Cols[(int)temp.x - 1].Count > (int)temp.y)
            {
                Debug.Log("orb " + curOrb.gameObject.name + " is pop checking left");
                node = Cols[(int)temp.x - 1].First;
                for (int i = 0; i < (int)temp.y; i++)
                {
                    node = node.Next;
                }
                EvaluateOrb(localGroup, node.Value.GetComponent<Orb>(), color);
            }
        }
        return false;
    }
}