using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuWidget : MenuWidget
{
    [SerializeField] private GameObject InfoPanel;


    protected override void Start()
    {
        base.Start();
        InfoPanel.SetActive(false);
    }
    public void ToggleInfoPanel()
    {
        InfoPanel.SetActive(!InfoPanel.activeSelf); // If the info panel is off, it will instead be turned on.
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

}
