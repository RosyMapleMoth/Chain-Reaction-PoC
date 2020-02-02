using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private float timeUntilDrop;
    private int incomingLines;
    private int topLine = 7;
    private int reserveLines;
    public Pattern[] StartPatt;
    public GameObject orbPrefab;
    static private int SPAWN_X_VAL = -4;


    private Queue<Pattern> dropQue;

    // Start is called before the first frame update
    void Start()
    {
        dropQue = new Queue<Pattern>();
        //XXX

        for (int i = 0; i < StartPatt.Length; i++)
        {
            dropQue.Enqueue(StartPatt[i]);
        }

        MakePattern(dropQue.Dequeue());
    }

    // Update is called once per frame
    void Update()
    {
        if (incomingLines > reserveLines)
        {
            MakePattern(dropQue.Dequeue());
        }
    }


    private void MakePattern(Pattern patt)
    {
        for (int c = 0; c < patt.lines.Length; c++)
        {
            for (int r = 0; r < patt.lines[c].orbs.Length; r++)
            {
                GameObject temp;

                temp = Instantiate(orbPrefab, new Vector3(SPAWN_X_VAL + r + .1f, topLine - 0.1f, -1), Quaternion.identity);
                temp.GetComponent<Orb>().LoadOrb(patt.lines[c].orbs[r]);
            }
            topLine += 1;
            reserveLines += 1;
        }


    }
}
