using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public delegate void VictoryDelegate();

    private bool active = false;

    public static event VictoryDelegate VictoryEvent = delegate { };
    private void Awake()
    {
        ObjectiveItem.ObjectiveActivatedEvent += delegate { active = true; };
        VictoryEvent += ResetEvents;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(active == true && other.CompareTag("Player") == true)
        {
            VictoryEvent.Invoke();
            HUD.Instance.ActivateEndPrompt(true);
        }
    }

    public static void ResetEvents()
    {
        VictoryEvent = delegate { };
    }

}
