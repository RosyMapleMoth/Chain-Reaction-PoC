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


    public Text countdown;
    public Transform GrabbedOrbs;


    private Queue<Pattern> dropQue;

    // Start is called before the first frame update
    void Start()
    {
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

        timeUntilDrop = 5f;
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


    private void MakePattern(Pattern patt)
    {
        for (int c = 0; c < patt.lines.Length; c++)
        {
            for (int r = 0; r < patt.lines[c].orbs.Length; r++)
            {
                GameObject temp;

                temp = Instantiate(orbPrefab, new Vector3(transform.position.x + SPAWN_X_VAL + r + .1f, transform.position.y + SPAWN_Y_Val +  reserveLines - 0.1f, -1), Quaternion.identity);
                temp.GetComponent<Orb>().LoadOrb(patt.lines[c].orbs[r]);
                temp.transform.SetParent(orbs.transform);
                OobCols[r].AddLast(temp);
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
                orbs.transform.position = new Vector3(orbs.transform.position.x, orbs.transform.position.y - 1, orbs.transform.position.z);
                for (int c = 0; c < 7; c++)
                {
                    Cols[c].AddLast(OobCols[c].First.Value);
                    OobCols[c].RemoveFirst();
                }
            }
            reserveLines -= numLines;
        }
        catch (Exception e)
        {
            Debug.Log("Error Droping line sugjestion insure pattern pool is not empty., fucntion exited halted with exception : " + e);
        }
    }


    public void DropOrbs(int Line)
    {

        while (GrabbedOrbs.childCount > 0)
        {
            GameObject ancorOrb;
            if (Cols[Line].Count > 0)
            {
                 ancorOrb = Cols[Line].First.Value;
            }
            else
            {
                ancorOrb = OobCols[Line].First.Value;
            }


            Transform moveingOrb = GrabbedOrbs.GetChild(0);

            moveingOrb.position = new Vector3(ancorOrb.transform.position.x, ancorOrb.transform.position.y - 1, ancorOrb.transform.position.z);
            moveingOrb.SetParent(orbs.transform);

            Cols[Line].AddFirst(moveingOrb.gameObject);
            HeldObrs--;
        }
        heldType = OrbType.ERROR;
        CheckPickerType();
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
                GameObject temp = Cols[Line].First.Value;
                Cols[Line].RemoveFirst();
                heldType = temp.GetComponent<Orb>().GetOrbType();
                StoreOrb(temp);
                CheckPickerType();
                AttemptGrabOrb(Line);
            }

            else
            {
                if (Cols[Line].First.Value.GetComponent<Orb>().checkOrbType(heldType))
                {
                    StoreOrb(Cols[Line].First.Value);
                    Cols[Line].RemoveFirst();
                    AttemptGrabOrb(Line);
                }
            }
        }
    }

    public void updateSelectLineVert()
    {
        if (Cols[switcher.GetComponent<Selector>().GetCurCol()].Count > 0)
        {
            selectLine.SetPosition(0, new Vector3(-3f + switcher.GetComponent<Selector>().GetCurCol(), Cols[switcher.GetComponent<Selector>().GetCurCol()].First.Value.transform.position.y - 0.9f, selectLine.GetPosition(0).z));
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
}