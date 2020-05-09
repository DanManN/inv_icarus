// using System.Collections;

// using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public float radius;
    public float mass;
    public float perceptionRadius;

    private List<Vector3> path;
    private NavMeshAgent nma;
    private Rigidbody rb;

    private HashSet<GameObject> perceivedNeighbors = new HashSet<GameObject>();
    private HashSet<GameObject> perceivedWalls = new HashSet<GameObject>();

    private bool isCrowded = false;

    void Start()
    {
        path = new List<Vector3>();
        nma = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        gameObject.transform.localScale = new Vector3(2 * radius, 1, 2 * radius);
        nma.radius = radius;
        rb.mass = mass;
        GetComponent<SphereCollider>().radius = perceptionRadius / 2;
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

            if (path.Count == 0 )
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
        nma.enabled = true;
        var nmPath = new NavMeshPath();
        nma.CalculatePath(destination, nmPath);
        path = nmPath.corners.Skip(1).ToList();
        //path = new List<Vector3>() { destination };
        //nma.SetDestination(destination);
        nma.enabled = false;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    #endregion

    #region Incomplete Functions

    private Vector3 ComputeForce()
    {
        Vector3 force;

        force = CalculateGoalForce();
        force += CalculateAgentForce() + CalculateWallForce();


        force += Damping(0.1f);

        if (force != Vector3.zero)
        {
            return force.normalized * Mathf.Min(force.magnitude, Parameters.maxSpeed);
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3 Damping(float damp)
    {
        return -damp * rb.velocity;
    }

    private Vector3 CalculateGoalForce()
    {
        Vector3 goal = path.FirstOrDefault();
        return (2 * Parameters.prefSpeed * (goal - transform.position).normalized - rb.velocity) * (mass / Parameters.T);
    }

    private Vector3 CalculateAgentForce()
    {
        Vector3 taForce = Vector3.zero;
        foreach (var a in perceivedNeighbors)
        {
            Vector3 nij = transform.position - a.transform.position;
            float dij = nij.magnitude;
            nij.Normalize();
            float rij = radius + a.GetComponent<Agent>().radius;
            float g_rd = rij - dij;
            if (g_rd < 0) g_rd = 0;
            Vector3 tij = Vector3.ProjectOnPlane(rb.velocity, nij);
            float del_vjit = tij.magnitude;
            tij.Normalize();
            del_vjit += Vector3.Dot(a.GetComponent<Rigidbody>().velocity, -tij);
            // print(rij - dij + ", " + g_rd);

            taForce += (Parameters.A * Mathf.Exp((rij - dij) / Parameters.B) + Parameters.k * g_rd) * nij - Parameters.Kappa * g_rd * del_vjit * tij;
        }
        return taForce;
    }

    private Vector3 CalculateWallForce()
    {
        Vector3 taForce = Vector3.zero;
        foreach (var a in perceivedWalls)
        {
            Vector3 niW = transform.position - a.GetComponent<BoxCollider>().ClosestPoint(transform.position);
            float diW = niW.magnitude;
            niW = Vector3.Project(transform.position - a.transform.position, niW);
            niW.Normalize();
            float ri = radius;
            float g_rd = ri - diW;
            if (g_rd < 0) g_rd = 0;
            Vector3 vtt = Vector3.ProjectOnPlane(rb.velocity, niW);
            // print(ri - diW + ", " + g_rd);

            taForce += (Parameters.WALL_A * Mathf.Exp((ri - diW) / Parameters.WALL_B) + Parameters.WALL_k * g_rd) * niW - Parameters.WALL_Kappa * g_rd * vtt;
        }
        return taForce;
    }

    public void ApplyForce()
    {
        var force = ComputeForce();
        force.y = 0;

        rb.AddForce(force * 10, ForceMode.Force);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent"))
            perceivedNeighbors.Add(other.gameObject);
        else if (other.CompareTag("Wall"))
            perceivedWalls.Add(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Agent"))
            perceivedNeighbors.Remove(other.gameObject);
        else if (other.CompareTag("Wall"))
            perceivedWalls.Remove(other.gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {

    }

    public void OnCollisionExit(Collision collision)
    {

    }

    #endregion
}
