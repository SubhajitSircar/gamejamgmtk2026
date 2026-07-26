using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject creditsPage;
    [SerializeField] private GameObject menuPage;
    [SerializeField] private GameObject roundsSelect;
    [SerializeField] private GameObject creditsPageFirstButton;


    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        quitButton.SetActive(false);
#endif

    }

    private void OnEnable()
    {
        Time.timeScale = 1;
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

        Debug.Log("Quit Game");
    }

    public void Credits()
    {
        creditsPage.SetActive(true);
        menuPage.SetActive(false);
        if (creditsPageFirstButton)
        {
            EventSystem.current.SetSelectedGameObject(creditsPageFirstButton);
        }
    }

    public void RoundsSelect()
    {
        roundsSelect.SetActive(true);
        menuPage.SetActive(false);
    }

    public void MainMenu()
    {
        menuPage.SetActive(true);
        creditsPage.SetActive(false);
        roundsSelect.SetActive(false);
    }

    public void PlayGame(int r)
    {
        GlobalScript g = GlobalScript.Instance;
        g.StartMatch(r);
        SceneManager.LoadScene(g.GetNextRoundScene());
    }
}
