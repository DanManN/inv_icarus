using UnityEngine;

public class Follow : MonoBehaviour
{
    // public float distance = 10f;
    // public float yDisp = 0.5f;
    public float zoomRate = 10f;

    public float xAngle = 30f;
    public float yAngle = 0f;

    public float pitchRate = 1f;
    public float panRate = 2f;

    // public GameObject targetObj = null;

    // private Vector3 deltaPos = Vector3.zero;

    void Start()
    {
        // if (targetObj != null)
        // {
        //     Vector3 diff = transform.position - targetObj.transform.position;
        //     distance = diff.magnitude;
        //     xAngle = transform.eulerAngles.x;
        //     yAngle = transform.eulerAngles.y;
        // }
        xAngle = transform.localEulerAngles.x;
        yAngle = transform.localEulerAngles.y;
    }

    void LateUpdate()
    {
        // if (targetObj != null)
        // {
        //     distance -= Input.mouseScrollDelta.y * zoomRate * Time.deltaTime;
        //     xAngle -= Input.GetAxis("CamPitch") * pitchRate;
        //     yAngle += Input.GetAxis("Pan") * panRate;

        //     Quaternion rot = Quaternion.Euler(xAngle, yAngle, 0);
        //     Vector3 point = rot * Vector3.forward;
        //     Vector3 pos = targetObj.transform.position - distance * point + yDisp * Vector3.up;
        //     transform.SetPositionAndRotation(pos, rot);
        // }
        transform.position += Input.mouseScrollDelta.y * zoomRate * transform.forward;

        xAngle -= Input.GetAxis("CamPitch") * pitchRate;
        yAngle += Input.GetAxis("Pan") * panRate;

        Quaternion rot = Quaternion.Euler(xAngle, yAngle, 0);
        transform.localRotation = rot;
    }
}
