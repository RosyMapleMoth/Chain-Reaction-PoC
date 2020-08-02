using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class score : MonoBehaviour
{

    private int ScoreVal = 0; 
    private int multiplyer = 0;
    private int chain = 0;

    //basic score gained form popoing a block
    private const int BaseBlockVal = 100;

    public Text Score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void resetChain()
    {
        chain = 0;
    }

    public void incressChain()
    {
        chain++;
    }

    public int getChain()
    {
        return chain;
    }


    public void scoreBlock()
    {
        if (chain == 0)
        {
            ScoreVal += BaseBlockVal * multiplyer;
            Debug.Log("Score : adding " + BaseBlockVal * multiplyer + " To player score");
        }
        else
        {
            ScoreVal += BaseBlockVal * chain * multiplyer;
            Debug.Log("Score : adding " + BaseBlockVal * chain * multiplyer + " To player score");
        }
    }

    public void resetScore()
    {
        ScoreVal = 0;
    }


    public void addRawScore(int add)
    {
        ScoreVal += add;
    }

    private void updateScoreView()
    {
        Score.text = getVisableScore();
    }


    // Returns score in 9 digit veiw
    public string getVisableScore()
    {
        return ScoreVal.ToString("D9");
    }

    // returns raw score as an int avalue 
    public int getRawScore()
    {
        return ScoreVal;
    }

}
