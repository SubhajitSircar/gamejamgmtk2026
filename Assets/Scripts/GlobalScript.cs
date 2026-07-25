using System.Collections.Generic;
using UnityEngine;

public class GlobalScript : MonoBehaviour
{
    public enum Player
    {
        Player1 = 0, Player2 = 1, Draw =2
    }

    public static GlobalScript Instance { get; private set; }

    public int TotalRounds { get; private set; }
    public int CurrentRound { get; private set; }

    public int Player1Wins { get; private set; }
    public int Player2Wins { get; private set; }

    public int DrawRounds { get; private set; }


    [Header("Round Scenes")]
    [SerializeField] private List<string> roundScenes = new();
    [SerializeField] private string gameOverScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartMatch(int rounds)
    {
        TotalRounds = rounds;
        CurrentRound = 1;

        Player1Wins = 0;
        Player2Wins = 0;
    }

    public void RecordRoundWinner(Player winner)
    {
        if (winner == Player.Player1)
            Player1Wins++;
        else
            Player2Wins++;

        CurrentRound++;
    }

    public bool IsMatchOver()
    {
        return CurrentRound > TotalRounds;
    }

    public Player GetMatchWinner()
    {
        if(Player1Wins == Player2Wins)
        {
            return Player.Draw;
        }
        return Player1Wins > Player2Wins
            ? Player.Player1
            : Player.Player2;
    }

    public string GetCurrentRoundScene()
    {
        return roundScenes[(CurrentRound - 1) % roundScenes.Count];
    }

    public string GetNextRoundScene()
    {
        if (CurrentRound > TotalRounds)
            return gameOverScene;

        return roundScenes[(CurrentRound - 1) % roundScenes.Count];
    }
}