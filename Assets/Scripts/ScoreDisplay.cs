using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public TMP_Text Player1RoundCount;
    public TMP_Text Player2RoundCount;
    // Start is called before the first frame update
    void Start()
    {
        GlobalScript g = GlobalScript.Instance;
        //Debug.Log(g.CurrentRound);
        if (g != null)
        {

            if (Player1RoundCount != null)
            {
                Player1RoundCount.text = g.Player1Wins.ToString();
            }

            if (Player2RoundCount != null)
            {
                Player2RoundCount.text = g.Player2Wins.ToString();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
