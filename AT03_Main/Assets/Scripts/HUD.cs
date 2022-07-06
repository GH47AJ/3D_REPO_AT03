using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image crosshairImg;
    [SerializeField] private Text objectiveTxt;
    [SerializeField] private string objectiveA;
    [SerializeField] private string objectiveB;
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private Text promptText;
    [SerializeField] [TextArea] private string victoryMessage;
    [SerializeField] [TextArea] private string gameOverMessage;

    public static HUD Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        promptPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        objectiveTxt.text = objectiveA;
        ObjectiveItem.ObjectiveActivatedEvent += delegate { objectiveTxt.text = objectiveB; };
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }
    }

    public void SetCrosshairColour(Color colour)
    {
        if(crosshairImg.color != colour)
        {
            crosshairImg.color = colour;
        }
    }

    public void ActivateEndPrompt(bool victory)
    {
        promptPanel.SetActive(true);
        if(victory == true)
        {
            promptText.text = victoryMessage;
        }
        else
        {
            promptText.text = gameOverMessage;
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void LoadFirstSceneInBuild()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
