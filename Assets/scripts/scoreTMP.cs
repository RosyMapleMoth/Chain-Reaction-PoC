using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scoreTMP : MonoBehaviour
{

    private int ScoreVal = 0; 
    private int incomingChange = 0;
    private int multiplyer = 1;
    private int chain = 0;

    public int level = 1;

    private int xpRequirement = 5000;

    //basic score gained form popoing a block
    private const int BaseBlockVal = 100;

    public TextMeshProUGUI Score;
    public TextMeshProUGUI addedScore;

    public TextMeshProUGUI Chain; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateScoreView();
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

        ScoreVal += incomingChange;
        addedScore.text = incomingChange.ToString();
        Chain.text = (chain + 1).ToString();

        if (ScoreVal > level * xpRequirement)
        {
            level++;
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

