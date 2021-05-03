using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/*
 * This class will hold the board and maintain its entegrity 
 *
 *
 *
 *
 *
 */
public class GameBoard : MonoBehaviour
{
    public static int SPAWN_Y_Val = 7;
    public static int SPAWN_X_VAL = -4;
    public static float X_OFF_SET = 0.5f;
    public static float Y_OFF_SET = -0.5f;
    public static int ORB_VIEW_LAYER = -3;
    public static int BOARD_WIDTH = 7;
    public static int BOARD_HIGHT = 13;

    public GameObject orbPrefab;
    public GameObject phsyicalBoard;
    public Line DEBUGline;
    private int orbIDNext = 0;
    public LinkedList<GameObject>[] board;
    public LinkedList<GameObject>[] incomingLines;
    public Text debug;
    public bool ContenctRequiresEval;

    void Start()
    {
        Debug.Log("WuW");
        initGameBoard();
        createLine(DEBUGline);
        createLine(DEBUGline);
        createLine(DEBUGline);
        dropLines(3);
        ContenctRequiresEval = false;
    }


    void Update()
    {
        updateDebug();
    }


    /// <summary>
    /// 
    /// </summary>
    public void initGameBoard()
    {
        Debug.Log("Initing Gameaboard");
        board = new LinkedList<GameObject>[7];
        for (int i = 0; i < 7; i++)
        {
            board[i] = new LinkedList<GameObject>();
        }
        incomingLines = new LinkedList<GameObject>[7];
        for (int i = 0; i < 7; i++)
        {
            incomingLines[i] = new LinkedList<GameObject>();
        }

    }


    /// <summary>
    /// Makes a line in the incoming lines boards based on passed in scriptable object line
    /// </summary>
    /// <param name="incomingLine">Scriptable object line</param>
    private void createLine(Line incomingLine)
    {
        for (int i = 0; i < incomingLine.orbs.Length; i++)
        {
            GameObject temp;

            temp = Instantiate(orbPrefab, new Vector3(transform.position.x + SPAWN_X_VAL + i, transform.position.y + SPAWN_Y_Val +  incomingLines[i].Count, ORB_VIEW_LAYER), Quaternion.identity);
            temp.name = "orb " + orbIDNext.ToString();
            temp.transform.GetChild(1).GetComponentInChildren<Text>().text = temp.name;
            orbIDNext = orbIDNext+1;
            temp.GetComponent<Orb>().LoadOrb(incomingLine.orbs[i]);
            temp.transform.SetParent(phsyicalBoard.transform);
            incomingLines[i].AddLast(temp);
        }
    }


    /// <summary>
    /// adds a specificed numeber of lines to board
    /// </summary>
    /// <param name="linesToDrop">Number of lines to drop</param>
    public void dropLines(int linesToDrop)
    {
        while (incomingLines[0].Count < linesToDrop)
        {
            createLine(DEBUGline);
        }
        for (int i = 0; i < linesToDrop; i++)
        {
            MoveLineToBoard();
        }
        dropLinesAnimation(linesToDrop);
    }


    /// <summary>
    /// Adds a line from incomingOrbs to board  
    /// </summary>
    private void MoveLineToBoard()
    {
        for (int i = 0; i < incomingLines.Length; i++)
        {
            board[i].AddFirst(incomingLines[i].First.Value);
            incomingLines[i].RemoveFirst();
        }
    }


    /// <summary>
    /// handles the viewable drop of lines
    /// </summary>
    /// <param name="linesToDrop">number of lines to drop </param>
    private void dropLinesAnimation(int linesToDrop)
    {
        phsyicalBoard.transform.position = (new Vector3(0f,-linesToDrop,0f) + phsyicalBoard.transform.position);
    }


    /// <summary>
    /// This fucntion returns the orb at postiong [x,y] or null if no orb exists
    /// </summary>
    /// <param name="x">int value between 0 and BOARD_SIZE</param>
    /// <param name="y">int value between 0 and BOARD_SIZE</param>
    /// <returns></returns>
    public GameObject At(int x, int y)
    {
        Debug.Log("evaluating orb at " + x + " , " + y);
        try 
        {
            LinkedListNode<GameObject> temp = board[x].First;
            for (int i = y; i > 0; i--)
            {
                temp = temp.Next;
            }
            return temp.Value;
        }
        catch
        {
            return null;
        }
    }


    /// <summary>
    /// removes the bottom orb from the collum represented by the int passed in and passes a refrence to the object
    /// </summary>
    /// <param name="Col">The col to get the orb from</param>
    /// <returns>a refrence to the orb removed from the board</returns>
    public GameObject GrabOrb(int Col)
    {
        // get refrence to orb
        GameObject orb = board[Col].Last.Value;
        
        // remove orb from boarded orbs and data structure 
        orb.transform.SetParent(null);
        board[Col].RemoveLast();

        Debug.Log("Pick Up : Picking up orb " + orb.name);   

        return orb;
    }


    /// <summary>
    /// places an orb at the end of passed in Col
    /// </summary>
    /// <param name="orb">The orb to be placed</param>
    /// <param name="Col">The Col to place orb in</param>
    public void PlaceOrb(GameObject orb, int Col)
    {
        board[Col].AddLast(orb);
        orb.transform.SetParent(phsyicalBoard.transform);
        ContenctRequiresEval = true;
    }


    /// <summary>
    /// Get the amount of orbs in a given collum 
    /// </summary>
    /// <param name="col">the col to get the size of</param>
    /// <returns>gives the amount of orbs in a col</returns>
    public int GetColSize(int col)
    {
        return board[col].Count;
    }  


    /// <summary>
    /// 
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    public OrbType GetOrbType(int col)
    {
        return board[col].Last.Value.GetComponent<Orb>().orbScript.orbType;
    }


    /// <summary>
    /// peeks the top orb on any give line
    /// </summary>
    /// <param name="col">the line that will be peeked</param>
    /// <returns>returns a refrence to the orb on the refrenced line</returns>
    public GameObject peekOrb(int col)
    {
        return board[col].Last.Value;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector3 getRelativeOragin()
    {
        return transform.position + new Vector3(SPAWN_X_VAL, SPAWN_Y_Val, 0f);
    }


    /// <summary>
    /// 
    /// </summary>
    private void updateDebug()
    {
        string temp = ""; 

        for (int i = 0; i < 7; i++)
        {
            temp += " col " + i + " |";
        }
        temp += "\n";
        for (int i = 0; i < 7; i++)
        {
            temp += "  s " + board[i].Count + "  |";
        }

        debug.text = temp;
    }


    /// <summary>
    /// Returns true if contence has been changed in a way that would require an evaluation since last evlauation
    /// </summary>
    /// <returns></returns>
    public bool IsEvalRequired()
    {
        return ContenctRequiresEval;
    }


    public void evaluated()
    {
        ContenctRequiresEval = false;
    } 
}