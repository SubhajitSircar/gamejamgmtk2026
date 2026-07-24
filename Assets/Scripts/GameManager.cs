using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isDuelActive = false;

    [Header("UI References")]
    public TextMeshProUGUI centerText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
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