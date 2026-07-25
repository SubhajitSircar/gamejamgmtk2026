using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject firstButton;

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        quitButton.SetActive(false);
#endif

    }

    private void OnEnable()
    {
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


    public void PlayGame()
    {
        SceneManager.LoadScene("NewTest");
    }
}
