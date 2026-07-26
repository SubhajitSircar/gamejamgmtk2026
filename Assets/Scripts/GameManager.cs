using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isDuelActive = false;


    [Header("UI References")]
    public TextMeshProUGUI centerText;
    //    public TMP_Text Player1RoundCount;
    //    public TMP_Text Player2RoundCount;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //GlobalScript g =GlobalScript.Instance;
        //Debug.Log(g.CurrentRound);
        //if (g != null) { 

        //    if (Player1RoundCount != null)
        //    {
        //        Player1RoundCount.text = g.Player1Wins.ToString();
        //    }

        //    if (Player2RoundCount != null)
        //    {
        //        Player2RoundCount.text = g.Player2Wins.ToString();
        //    }
        //}

        StartCoroutine(DuelCountdownSequence());
    }

    IEnumerator DuelCountdownSequence()
    {
        // Lock inputs
        isDuelActive = false;

        // The 3-2-1 Sequence
        centerText.text = "3";
        yield return new WaitForSeconds(1f);

        centerText.text = "2";
        yield return new WaitForSeconds(1f);

        centerText.text = "1";
        yield return new WaitForSeconds(1f);

        // Unlock inputs and start the duel
        centerText.text = "DRAW!!";
        isDuelActive = true;


        // Hide the DRAW text after 2 seconds
        yield return new WaitForSeconds(2f);
        centerText.text = "";
    }
}
