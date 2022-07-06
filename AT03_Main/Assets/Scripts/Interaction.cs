using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private float distance = 4f;
    [SerializeField] private bool debug = false;
    [SerializeField] private LayerMask interactionMask; //Determines what layers the ray will interact with.

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance, interactionMask) == true)
        {
            HUD.Instance.SetCrosshairColour(Color.green);
            if (debug == true) { Debug.DrawRay(transform.position, transform.forward * distance, Color.green); }
            if (Input.GetButtonDown("Fire1") == true)
            {
                if (hit.transform.TryGetComponent(out IInteractable interaction) == true)
                {
                    interaction.Activate();
                }
            }
        }
        else
        {
            HUD.Instance.SetCrosshairColour(Color.red);
            if (debug == true) { Debug.DrawRay(transform.position, transform.forward * distance, Color.red); }
        }
    }
}

public interface IInteractable
{
    void Activate();
}
