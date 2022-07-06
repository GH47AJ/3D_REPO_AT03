using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveItem : MonoBehaviour, IInteractable
{
    public delegate void ObjectiveDelegate();

    private static bool active = false;

    public static event ObjectiveDelegate ObjectiveActivatedEvent = delegate { };

    public AudioSource AudioSource { get; private set; }

    [SerializeField] private AudioClip alarmClip;

    private void Awake()
    {
        active = false;
        ObjectiveActivatedEvent += ResetEvents;
        if (TryGetComponent(out AudioSource aSrc) == true)
        {
            AudioSource = aSrc;
        }
    }

    public void Activate()
    {
        if (active == false)
        {
            active = true;
            ObjectiveActivatedEvent.Invoke();
            AudioSource.PlayOneShot(alarmClip); //this will allow for other sounds to play.
            AudioSource.clip = alarmClip;
            AudioSource.Play();
        }
    }

    public static void ResetEvents()
    {
        ObjectiveActivatedEvent = delegate { };
    }
}
