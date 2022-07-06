using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWidget : MonoBehaviour
{
    public delegate void MenuInputDelegate(float inputAxis);

    [SerializeField] private float bufferTime = 0.5f;

    private float timer = -1;
    private int currentButtonIndex = 0;
    private MenuButton currentButton;

    public List<MenuButton> buttons { get; private set; } = new List<MenuButton> ();
    public MenuButton SelectedButton
    {
        get { return currentButton; }
        set
        {
            if(buttons.Contains(value) == true)
            {
                currentButton = value;
                currentButtonIndex = buttons.IndexOf(value);
            }
        }
    }

    public int CurrentButtonIndex
    {
        get { return currentButtonIndex; }
        set
        {
            currentButtonIndex = value;
            if(currentButtonIndex >= buttons.Count)
            {
                currentButtonIndex = 0;
            }
            else if(currentButtonIndex < 0)
            {
                currentButtonIndex = buttons.Count - 1;
            }
        }
    }

    private event MenuInputDelegate VerticalAxisInputEvent = delegate { };

    protected virtual void Awake()
    {
        foreach(MenuButton button in GetComponentsInChildren<MenuButton>())
        {
            buttons.Add(button);
        }
        VerticalAxisInputEvent = new MenuInputDelegate(SelectButton);
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(buttons.Count > 0)
        {
            SelectButton(0);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(timer < 0)
        {
            float axis = Input.GetAxisRaw("Vertical");
            if(axis != 0)
            {
                VerticalAxisInputEvent.Invoke(axis);
            }
        }
        else
        {
            timer += Time.deltaTime;
            if(timer >= bufferTime)
            {
                timer = -1;
            }
        }

        if(Input.GetButtonDown("Submit") == true)
        {
            SelectedButton.Activate();
        }
    }

    private void SelectButton(float axis)
    {
        if(axis < 0)
        {
            CurrentButtonIndex++;
        }
        else if(axis > 0)
        {
            CurrentButtonIndex--;
        }
        buttons[CurrentButtonIndex].Select();
        timer = 0;
    }
}
