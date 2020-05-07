// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class Manual : MonoBehaviour
{
    public float thrustForce = 2000f;
    public float liftForce = 8f;
    public float dragForce = 0.2f;
    public float rollTorque = 500;
    public float pitchTorque = 5000f;

    public GameObject detonator;
    public float criticalSpeed = 50;

    private Rigidbody rb;
    // private float zAngle = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // zAngle = transform.eulerAngles.z;
    }

    public void resetRoll()
    {
        Quaternion rot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        transform.rotation = rot;
    }

    private Vector3 thrust(float force)
    {
        return Input.GetAxis("Thrust") * force * transform.forward;
    }

    private Vector3 lift(float force)
    {
        return Vector3.Dot(rb.velocity, transform.forward) * force * transform.up;
    }

    private Vector3 drag(float force)
    {
        return -force * rb.velocity.sqrMagnitude * rb.velocity.normalized;
    }

    private Vector3 roll(float torque)
    {
        return -Input.GetAxis("Roll") * torque * transform.forward;
    }

    private Vector3 pitch(float torque)
    {
        return Input.GetAxis("Pitch") * torque * transform.right;
    }

    void FixedUpdate()
    {
        Vector3 totalForce = thrust(thrustForce) + lift(liftForce) + drag(dragForce);
        if (Input.GetKey(KeyCode.LeftControl))
        {
            totalForce += drag(8.0f * dragForce);
            totalForce += lift(0.5f * liftForce);
        }
        Vector3 totalTorque = roll(rollTorque) + pitch(pitchTorque);

        rb.AddForce(totalForce);
        rb.AddTorque(totalTorque);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        float speed = rb.velocity.magnitude;
        float hitSpeed = Vector3.Dot(rb.velocity, collisionInfo.contacts[0].normal);
        print("Speed: " + speed);
        print("hitSpeed: " + hitSpeed);
        if (speed > criticalSpeed || Mathf.Abs(hitSpeed) > criticalSpeed / 4)
        {
            Transform cam = gameObject.transform.Find("FlightCam");
            Vector3 pos = cam.position;
            Quaternion rot = cam.rotation;
            Detonator dTemp = detonator.GetComponent<Detonator>();
            // dTemp.detail = 1.0f;
            GameObject exp = (GameObject)Instantiate(detonator, transform.position, Quaternion.identity);
            cam.parent = exp.transform;
            cam.SetPositionAndRotation(pos, rot);
            Destroy(exp, 20.0f);
            Destroy(gameObject);
            cam.parent = null;
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        // print("Speed: " + rb.velocity.magnitude);
    }
}
