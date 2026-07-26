using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    GlobalScript g;
    [SerializeField] private TMP_Text winnerText;

    // Start is called before the first frame update
    void Start()
    {
        g = GlobalScript.Instance;
        GlobalScript.Player winner = g.GetMatchWinner();
        if(winner == GlobalScript.Player.Draw)
        {
            winnerText.text = "Draw";
        }
        else if(winner == GlobalScript.Player.Player1)
        {
            winnerText.text = "Player 1 Wins";
        }
        else
        {
            winnerText.text = "Player 2 Wins";

        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif

        Debug.Log("Quit Game");
    }



    
}
