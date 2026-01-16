using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private float accelerationSpeed, maxSpeed, brakeStrength, deccelerationSpeed, turnSpeed, knockbackForce, knockbackDuration, horizontal;
    [SerializeField]
    private bool accelerating, braking, knockedBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (rb.linearVelocity.z > maxSpeed && !knockedBack)
        {
            rb.linearVelocity = new Vector3(0, 0, maxSpeed);
        }
        else if (rb.linearVelocity.z <= 0 && !knockedBack)
        { 
            rb.linearVelocity = new Vector3(0, 0, 0);
            braking = false;
        }

        Movement();
    }

    public void Movement()
    {
        Vector3 direction = transform.forward;
        transform.Rotate(new Vector3(0, horizontal, 0) * turnSpeed * Time.deltaTime);

        if (accelerating)
        {
            rb.AddForce(direction * accelerationSpeed, ForceMode.Acceleration);
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

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void TakeKnockback(Collision collision)
    {
        knockedBack = true;
        Vector3 direction = -transform.forward;

        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

        StartCoroutine(EndKnockback());
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 6)
        {
            TakeKnockback(col);
        }
    }

    IEnumerator EndKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);
        knockedBack = false;
    }
}
