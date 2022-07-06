using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void MenuButtonAction();

    [SerializeField] private Color defaultColour;
    [SerializeField] private Color selectedColour;
    [SerializeField] private Color hoverColour;
    [SerializeField] private UnityEvent onActivate;

    private bool mouseOver = false;
    private Image image;
    private MenuWidget instance;

    public event MenuButtonAction ActivateEvent = delegate { };
    public event MenuButtonAction SelectEvent = delegate { };

    private void OnEnable()
    {
        ActivateEvent += OnActivate;
        SelectEvent += OnSelect;
    }

    private void OnDisable()
    {
        ActivateEvent -= OnActivate;
        SelectEvent -= OnSelect;
    }

    private void Awake()
    {
        TryGetComponent(out image);
        instance = GetComponentInParent<MenuWidget>();
        image.color = defaultColour;
    }

    // Update is called once per frame
    void Update()
    {
        if(mouseOver == true && Input.GetButtonDown("Fire1") == true)
        {
            if(instance.SelectedButton == this)
            {
                Activate();
            }
            else
            {
                Select();
            }
        }
    }

    public void Select()
    {
        SelectEvent.Invoke();
    }

    public void Activate()
    {
        ActivateEvent.Invoke();
    }

    private void OnActivate()
    {
        onActivate.Invoke();
    }

    private void OnSelect()
    {
        if(instance.SelectedButton != null)
        {
            instance.SelectedButton.image.color = instance.SelectedButton.defaultColour;
        }
        instance.SelectedButton = this;
        image.color = selectedColour;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        if(instance.SelectedButton != this)
        {
            image.color = hoverColour;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        if(instance.SelectedButton != this)
        {
            image.color = defaultColour;
        }
    }
}
