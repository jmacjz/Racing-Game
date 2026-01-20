using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;

public class CarMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private float accelRate, maxSpeed, brakeStrength, deccelRate, turnStrength, knockbackForce, knockbackDuration, horizontal, strafeDirection;
    [SerializeField]
    private bool accelerating, braking, knockedBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Drive();
        Debug.Log(Mathf.Abs(rb.linearVelocity.z));
    }

    public void Drive()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.x = 0;
        rb.linearVelocity = transform.TransformDirection(localVelocity);

        if (accelerating && !braking)
            rb.AddRelativeForce(Vector3.forward * accelRate, ForceMode.Acceleration);
        else if (!accelerating && braking && localVelocity.z > 0)
            rb.AddRelativeForce(Vector3.back * brakeStrength, ForceMode.Acceleration);
        else if (!accelerating && !braking && localVelocity.z > 0)
            rb.AddRelativeForce(Vector3.back * deccelRate, ForceMode.Acceleration);

        if (horizontal > 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horizontal * turnStrength * Time.fixedDeltaTime, 0f));
        }
        else if (horizontal < 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horizontal * turnStrength * Time.fixedDeltaTime, 0f));
        }
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

    public void TakeKnockback(Collision collision)
    {
        knockedBack = true;

        rb.AddRelativeForce(transform.forward * -knockbackForce, ForceMode.Acceleration);

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
