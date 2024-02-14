using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scoreTMP : MonoBehaviour
{

    private int ScoreVal = 0; 
    private int incomingChange = 0;
    private int multiplyer = 100;
    private int chain = 0;

    public int level = 1;

    private int xpRequirement = 5000;

    //basic score gained form popoing a block
    private const int BaseBlockVal = 100;

    public TextMeshProUGUI Score;
    public TextMeshProUGUI addedScore;

    public TextMeshProUGUI Chain; 
    public TextMeshProUGUI levelTMP;
    public float timeSinceLastScore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateScoreView();
        timeSinceLastScore += Time.deltaTime;
        addedScore.alpha = 1.5f - (timeSinceLastScore);
        addedScore.transform.localScale = new Vector3(1.5f - (timeSinceLastScore), 1.5f - (timeSinceLastScore), 1.5f - (timeSinceLastScore));
    }


    public void resetChain()
    {
        chain = 1;
    }

    public void incressChain()
    {
        chain++;
    }

    public int getChain()
    {
        return chain;
    }

    public void scoreBlock(int SizeOfBlock)
    {
        incomingChange += SizeOfBlock * (chain) * multiplyer;
        Debug.Log("Score : adding " + SizeOfBlock * chain * multiplyer + " To player score");
        incressChain();
    }

    public void scoreBlock()
    {
        
        incomingChange += BaseBlockVal * (chain + 1) * multiplyer;
        Debug.Log("Score : adding " + BaseBlockVal * chain * multiplyer + " To player score");
    }

    public void resetScore()
    {
        ScoreVal = 0;
    }


    public void addRawScore(int add)
    {
        ScoreVal += add;
    }

    public void finializePop()
    {
        timeSinceLastScore = 0;
        ScoreVal += incomingChange;
        addedScore.text = "+" + incomingChange.ToString();
        Chain.text = (chain-1).ToString();

        if (ScoreVal > level * xpRequirement)
        {
            while (ScoreVal - (level-1) * xpRequirement > level * xpRequirement)
            {
                level++; 
                levelTMP.text = level.ToString();
            }
        }

        incomingChange = 0;
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

