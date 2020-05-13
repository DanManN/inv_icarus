// using System.Collections;
// using System;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float maxThrust = 5000f;
    // public float liftForce = 8f;
    // public float dragForce = 0.2f;
    public float maxTorque = 500000;

    public float perceptionDistance;
    public float perceptionAngle;

    private List<Vector3> path;
    private Rigidbody rb;

    private HashSet<GameObject> perceivedNeighbors = new HashSet<GameObject>();
    private HashSet<GameObject> perceivedObstacles = new HashSet<GameObject>();

    void Start()
    {
        path = new List<Vector3>();
        rb = GetComponent<Rigidbody>();

        // gameObject.transform.localScale = new Vector3(2 * radius, 1, 2 * radius);
        // GetComponent<CapsuleCollider>().height = perceptionDistance;
        // GetComponent<CapsuleCollider>().radius = perceptionAngle;
    }

    private void Update()
    {
        if (path.Count > 1 && Vector3.Distance(transform.position, path[0]) < 1.1f)
        {
            path.RemoveAt(0);
        }
        else if (path.Count == 1 && Vector3.Distance(transform.position, path[0]) < 2f)
        {
            path.RemoveAt(0);

            if (path.Count == 0)
            {
                gameObject.SetActive(false);
                AgentManager.RemoveAgent(gameObject);
            }
        }

        #region Visualization

        if (true)
        {
            if (path.Count > 0)
            {
                Debug.DrawLine(transform.position, path[0], Color.green);
            }
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.yellow);
            }
        }

        if (true)
        {
            foreach (var neighbor in perceivedNeighbors)
            {
                Debug.DrawLine(transform.position, neighbor.transform.position, Color.yellow);
            }
        }

        #endregion
    }

    #region Public Functions

    public void ComputePath(Vector3 destination)
    {
        path = new List<Vector3>() { destination };
        // call rrt here
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    #endregion

    public Vector3 goalTorque()
    {
        Vector3 disp = path[0] - transform.position;
        float angle = Vector3.Angle(disp, transform.forward);
        if (Mathf.Abs(angle) < 1) return Vector3.zero;
        return -Vector3.Cross(disp.normalized, transform.forward).normalized;
    }

    public float goalThrust()
    {
        Vector3 disp = path[0] - transform.position;
        Vector3 zVel = Vector3.Project(rb.velocity, disp);
        Vector3 xyVel = rb.velocity - zVel;
        Vector3 idealForce = (2 * disp.normalized - xyVel); //* rb.mass;// / AgentManager.UPDATE_RATE;
        float mag = Vector3.Dot(idealForce, transform.forward);
        return mag;
    }

    public void ApplyThrust()
    {
        float totalThrust = 500 * goalThrust();
        print(totalThrust);
        if (totalThrust > maxThrust) totalThrust = maxThrust;
        rb.AddForce(totalThrust * transform.forward);
    }

    public void Steer()
    {
        Vector3 totalTorque = 10000 * goalTorque();
        Debug.DrawLine(transform.position, transform.position + 5 * goalTorque(), Color.yellow);

        if (totalTorque.magnitude > maxTorque) totalTorque = maxTorque * totalTorque.normalized;
        rb.AddTorque(totalTorque);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent"))
            perceivedNeighbors.Add(other.gameObject);
        else if (other.CompareTag("Obstacle"))
            perceivedObstacles.Add(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Agent"))
            perceivedNeighbors.Remove(other.gameObject);
        else if (other.CompareTag("Obstacle"))
            perceivedObstacles.Remove(other.gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {

    }

    public void OnCollisionExit(Collision collision)
    {

    }

}
