using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private float accelerationSpeed, maxSpeed, brakeStrength, deccelerationSpeed;
    [SerializeField]
    private bool accelerating, braking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (rb.linearVelocity.z > maxSpeed)
        {
            rb.linearVelocity = new Vector3(0, 0, maxSpeed);
        }
        else if (rb.linearVelocity.z <= 0)
        { 
            rb.linearVelocity = new Vector3(0, 0, 0);
            braking = false;
        }

        Movement();
    }

    public void Movement()
    {
        if (accelerating)
        {
            rb.AddForce(new Vector3(0, 0, accelerationSpeed), ForceMode.Acceleration);
        }

        if (braking)
        {
            rb.AddForce(new Vector3(0, 0, -accelerationSpeed), ForceMode.Acceleration);
        }

        if (accelerating == false)
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, deccelerationSpeed * Time.fixedDeltaTime);
    }

    public void Drive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            accelerating = true;
        }

        if (context.canceled)
        {
            accelerating = false;
        }

    }

    public void Brake(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            braking = true;
        }
        if (context.canceled)
        {
            braking = false;
        }

    }
}
