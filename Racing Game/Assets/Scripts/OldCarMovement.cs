using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;

public class OldCarMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private float accelerationSpeed, maxSpeed, brakeStrength, deccelerationSpeed, turnSpeed, knockbackForce, knockbackDuration, horizontal, strafeDirection;
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
            rb.linearVelocity = transform.InverseTransformDirection(Vector3.zero);
            braking = false;
        }

        if (horizontal < 0)
        {
            //rb.AddForce(Vector3.left * turnSpeed);
            rb.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horizontal * turnSpeed * Time.fixedDeltaTime, 0f));
        }
        else if (horizontal > 0)
        {
            //rb.AddForce(Vector3.right * turnSpeed);
            rb.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horizontal * turnSpeed * Time.fixedDeltaTime, 0f));
        }

        Movement();
        Debug.Log(rb.linearVelocity);
    }

    public void Movement()
    {
        //Vector3 direction = transform.forward;
        //transform.Rotate(new Vector3(0, horizontal, 0) * turnSpeed * Time.deltaTime);

        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.x = 0;
        rb.linearVelocity = transform.TransformDirection(localVelocity);

        if (accelerating)
        {
            rb.AddRelativeForce(transform.forward * accelerationSpeed, ForceMode.Acceleration);
        }

        if (braking)
        {
            rb.AddRelativeForce(-transform.forward * brakeStrength, ForceMode.Acceleration);
        }

        if (accelerating == false && localVelocity.z > 0)
            rb.AddRelativeForce(-transform.forward * deccelerationSpeed, ForceMode.Acceleration);
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

    public void Strafe(InputAction.CallbackContext context)
    {
        strafeDirection = context.ReadValue<Vector2>().x;

        if (strafeDirection < 0)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
            rb.linearVelocity = new Vector3(-10, rb.linearVelocity.y, rb.linearVelocity.z);
        }
        else if (strafeDirection > 0)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
            rb.linearVelocity = new Vector3(10, rb.linearVelocity.y, rb.linearVelocity.z);
        }
        else
            rb.linearVelocity = rb.linearVelocity;
            
    }

    public void TakeKnockback(Collision collision)
    {
        knockedBack = true;
        Vector3 direction = -transform.forward;

        rb.AddForce(direction * knockbackForce, ForceMode.Acceleration);

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
