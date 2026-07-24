using UnityEngine;

public class DarknessManager : MonoBehaviour
{
    public GameObject darknessPanel;

    void Start()
    {
        darknessPanel.SetActive(false);
    }

    public void EnableDarkness()
    {
        darknessPanel.SetActive(true);
    }

    public void DisableDarkness()
    {
        darknessPanel.SetActive(false);
    }
}