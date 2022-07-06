using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    [SerializeField] public float sensitivity = 2.5f; //sensitivity of mouse input
    [SerializeField] public float drag = 1.5f; //continued mouse movement after input steps

    
    private Vector2 result; //resulting cursor position
    private Vector2 smoothing; //smoothed cursor value
    private Transform character; //this references character transform component

    public static bool mouseLookEnabled = true;

    void Start()
    {
        mouseLookEnabled = true;
        character = transform.root;
    }

    void Update()
    {
        if(mouseLookEnabled == true)
        {
            Vector2 mouseDir = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y") * sensitivity);

            smoothing = Vector2.Lerp(smoothing, mouseDir, 1f / drag);
            result += smoothing;
            result.y = Mathf.Clamp(result.y, -70, 70);

            transform.localRotation = Quaternion.AngleAxis(-result.y, Vector3.right);
            character.rotation = Quaternion.AngleAxis(result.x, character.transform.up);
        }
    }
}
