using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float gravity = 5;

    private bool isGrounded = false;
    private float yVelocity = 0;
    private float currentSpeed = 0;
    private CharacterController controller;

    public static bool canMove = true;

    private void Awake()
    {
        TryGetComponent(out controller);
    }

    void Start()
    {
        currentSpeed = speed;
        canMove = true;
    }

    private void FixedUpdate()
    {
        if(isGrounded == true)
        {
            yVelocity = -gravity * Time.deltaTime;
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;
        }
        isGrounded = controller.isGrounded;
    }


    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        Vector3 step = Vector3.zero;

        if (canMove == true)
        {
            step += transform.forward * inputY * currentSpeed;
            step += transform.right * inputX * currentSpeed;
            step.y += yVelocity;
            controller.Move(step * Time.deltaTime);
        }
    }
}
