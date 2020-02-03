using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    private float timeUntilDrop = 0f;
    private int incomingLines;
    private int reserveLines = 0;
    public Pattern[] StartPatt;
    public GameObject orbPrefab;
    public GameObject orbs;
    static private int SPAWN_Y_Val = 7;
    static private int SPAWN_X_VAL = -4;

    public GameObject switcher;
    public int HeldObrs = 0;
    public OrbType heldType = OrbType.ERROR;
    public SpriteRenderer HeldOrbRep;

    public Queue<GameObject>[] Cols;
    public Queue<GameObject>[] OobCols;

    public Sprite[] pickers;


    public Text countdown;


    private Queue<Pattern> dropQue;

    // Start is called before the first frame update
    void Start()
    {
        Cols = new Queue<GameObject>[7];
        for (int i = 0; i<7; i++)
        {
            Cols[i] = new Queue<GameObject>();
        }


        OobCols = new Queue<GameObject>[7];
        for (int i = 0; i < 7; i++)
        {
            OobCols[i] = new Queue<GameObject>();
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
        if (incomingLines > reserveLines)
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
                OobCols[r].Enqueue(temp);
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
                    Cols[c].Enqueue(OobCols[c].Dequeue());
                }
            }
        } catch 
        {
            Debug.Log("Exception throw likly that pattern que is empty");
        }
    }

    public void AttemptGrabOrb(int Line)
    {
        if (Cols[Line].Count > 0)
        {
            if (HeldObrs == 0)
            {
                GameObject temp = Cols[Line].Dequeue();
                heldType = temp.GetComponent<Orb>().GetOrbType();
                //HeldOrbRep.gameObject.SetActive(true);
                //HeldOrbRep.sprite = temp.GetComponent<Orb>().orbScript.orbColor;
                Destroy(temp);
                HeldObrs = 1;

                switch (heldType)
                {
                    case OrbType.Green:
                        switcher.GetComponent<SpriteRenderer>().sprite = pickers[5];
                        break;
                    case OrbType.Blue:
                        switcher.GetComponent<SpriteRenderer>().sprite = pickers[3];
                        break;
                    case OrbType.Ornage:
                        switcher.GetComponent<SpriteRenderer>().sprite = pickers[1];
                        break;
                    case OrbType.Red:
                        switcher.GetComponent<SpriteRenderer>().sprite = pickers[6];
                        break;
                    case OrbType.Pink:
                        switcher.GetComponent<SpriteRenderer>().sprite = pickers[2];
                        break;
                    case OrbType.Purple:
                        switcher.GetComponent<SpriteRenderer>().sprite = pickers[4];
                        break;
                    case OrbType.ER:
                        break;
                    case OrbType.SR:
                        break;
                    case OrbType.BullshitPowerUp:
                        break;
                    case OrbType.ERROR:
                        break;
                }
            }

            else
            {
                if (Cols[Line].Peek().GetComponent<Orb>().checkOrbType(heldType))
                {
                    Destroy(Cols[Line].Dequeue());
                    HeldObrs++;
                }
            }
            }
        }
    }