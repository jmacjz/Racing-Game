using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;

public class CarMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private float accelRate, maxSpeed, brakeStrength, deccelRate, turnStrength, strafeSpeed, knockbackForce, knockbackDuration, horizontal, strafeDirection;
    [SerializeField]
    private bool accelerating, braking, knockedBack;

    [SerializeField]
    private LayerMask groundLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        GroundCheck();

        if (!knockedBack)
            Drive();
        Debug.Log(Mathf.Abs(rb.linearVelocity.z));
    }

    public void Drive()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        if (horizontal > 0 && strafeDirection == 0)
        {
            if (localVelocity.x < 0.2)
                localVelocity.x += 0.01f;
        }
        else if (horizontal < 0 && strafeDirection == 0)
        {
            if (localVelocity.x > -0.2)
                localVelocity.x -= 0.01f;
        }
        else
            localVelocity.x = 0;

            rb.linearVelocity = transform.TransformDirection(localVelocity);

        if (accelerating && !braking)
            rb.AddRelativeForce(Vector3.forward * accelRate, ForceMode.Acceleration);
        else if (!accelerating && braking && localVelocity.z > 0)
            rb.AddRelativeForce(Vector3.back * brakeStrength, ForceMode.Acceleration);
        else if (!accelerating && !braking && localVelocity.z > 0)
            rb.AddRelativeForce(Vector3.back * deccelRate, ForceMode.Acceleration);

        if (horizontal > 0 && localVelocity.z >= 0.01f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horizontal * turnStrength * Time.fixedDeltaTime, 0f));
        }
        else if (horizontal < 0 && localVelocity.z >= 0.01f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horizontal * turnStrength * Time.fixedDeltaTime, 0f));
        }

        if (strafeDirection < 0 && localVelocity.z >= 0.01f)
            rb.AddRelativeForce(Vector3.left * strafeSpeed * 100);
        else if (strafeDirection > 0 && localVelocity.z >= 0.01f)
            rb.AddRelativeForce(Vector3.right * strafeSpeed * 100);
    }
    
    public void Accelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
            accelerating = true;
        else if (context.canceled)
            accelerating = false;
    }

    public void Brake(InputAction.CallbackContext context)
    {
        if (context.performed)
            braking = true;
        else if (context.canceled)
            braking = false;
    }

    public void Turn(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Strafe(InputAction.CallbackContext context)
    {
        strafeDirection = context.ReadValue<Vector2>().x;
    }

    public void TakeKnockback(Collision collision)
    {
        knockedBack = true;
        
        Vector3 knockDir = collision.contacts[0].normal; // opposite direction of collision 

        rb.AddRelativeForce(knockDir * knockbackForce, ForceMode.Impulse);

        StartCoroutine(EndKnockback());
    }

    public void GroundCheck()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, groundLayer))

        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
        }
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
