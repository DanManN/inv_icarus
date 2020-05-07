using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;

public class MoveCamera : MonoBehaviour
{

    public float zoomRate = 10f;
    public float pitchRate = 1f;
    public float panRate = 2f;
    public float speed = 5f;
    public GameObject targetObj = null;

    public void setTarget(GameObject target)
    {
        targetObj = target;
    }


    private float distance = 10f;
    private float yDisp = 0f;
    private float xAngle = 30f;
    private float yAngle = 0f;

    private float getAxis(string axis)
    {
        return Input.GetAxis(axis);
    }

    private void moveTarget(float dist)
    {
        Vector3 dir = getAxis("ForwardBack") * forward() + getAxis("LeftRight") * right() + getAxis("Up") * Vector3.up + getAxis("Down") * Vector3.down;
        targetObj.transform.Translate(dir.normalized * Time.fixedDeltaTime * dist);
    }

    void Start()
    {
        if (targetObj != null)
        {
            Vector3 diff = transform.position - targetObj.transform.position;
            distance = diff.magnitude;
            xAngle = transform.eulerAngles.x;
            yAngle = transform.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        if (targetObj != null)
        {
            distance -= Input.mouseScrollDelta.y * zoomRate * Time.deltaTime;
            xAngle -= getAxis("Pitch") * pitchRate;
            yAngle += getAxis("Pan") * panRate;

            Quaternion rot = Quaternion.Euler(xAngle, yAngle, 0);
            Vector3 point = rot * Vector3.forward;
            Vector3 pos = targetObj.transform.position - distance * point + yDisp * Vector3.up;
            transform.SetPositionAndRotation(pos, rot);
        }
    }

    void FixedUpdate()
    {
        moveTarget(speed);
    }
}
