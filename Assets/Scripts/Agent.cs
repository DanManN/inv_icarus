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

            if (path.Count == 0 && !AgentManager.instance.leader_following && !AgentManager.instance.spiral && !AgentManager.instance.pursue_evade)
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
        if (AgentManager.instance.crowd_following)
        {
            force = CalculateCrowdForce();
        }
        else if (AgentManager.instance.leader_following)
        {
            force = CalculateLeaderFollowing();
        }
        else if (AgentManager.instance.pursue_evade)
        {
            force = CalculatePursueEvade();
            force += CalculateAgentForce() + CalculateWallForce();
        }
        else if (AgentManager.instance.spiral)
        {
            force = CalculateSpiralForce();
            force += CalculateAgentForce() + CalculateWallForce();
        }
        else
        {
            force = CalculateGoalForce();
            force += CalculateAgentForce() + CalculateWallForce();
        }

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

    private Vector3 CalculatePursueEvade()
    {
        Vector3 agentForce = Vector3.zero;

        //AgentManager.instance.destination = Vector3.forward * 100;

        GetComponent<SphereCollider>().radius = 20;

        bool isEvader = int.Parse(name.Split(' ')[1]) < AgentManager.instance.agentCount / 2;

        Vector3 located = transform.position;

        if (isEvader)
        {
            foreach (var n in perceivedNeighbors)
            {
                var neighbor = AgentManager.agentsObjs[n];
                var neighLocated = neighbor.transform.position;
                var dist = (located - neighLocated);

                var multiplier = Mathf.Min(2 / dist.magnitude, 5);

                var dir = multiplier * dist.normalized;

                var overlap = (radius + neighbor.radius) - Vector3.Distance(located, neighLocated);

                bool otherIsEvader = int.Parse(n.name.Split(' ')[1]) < AgentManager.instance.agentCount / 2;

                Vector3 tan = Vector3.Cross(Vector3.up, dir) * 0.3f;
                agentForce += tan;

                if (otherIsEvader)
                {
                    agentForce += 0.3f * Mathf.Exp(overlap) * dir;
                }
                else agentForce += dir * 0.3f;
            }
        }
        else
        {
            GetComponent<MeshRenderer>().material = AgentManager.instance.pursuers;

            foreach (var n in perceivedNeighbors)
            {
                var neighbor = AgentManager.agentsObjs[n];
                var neighLocated = neighbor.transform.position;
                var dist = (located - neighLocated);
                var multiplier = Mathf.Min(2 / dist.magnitude, 5);
                var dir = multiplier * dist.normalized;

                var overlap = (radius + neighbor.radius) - Vector3.Distance(located, neighLocated);
                bool otherIsEvader = int.Parse(n.name.Split(' ')[1]) < AgentManager.instance.agentCount / 2;

                if (otherIsEvader)
                {
                    agentForce -= dir * 0.3f;
                }
                else agentForce += (0.3f * Mathf.Exp(overlap) * dir);
            }
        }

        return agentForce.normalized * Parameters.prefSpeed;
    }


    private Vector3 CalculateLeaderFollowing()
    {
        foreach (var neighbor in perceivedNeighbors)
        {
            Debug.DrawLine(transform.position, neighbor.transform.position, Color.yellow);
        }
        Debug.Log(perceivedNeighbors.Count);
        Vector3 force = Vector3.zero;

        int current_id = int.Parse(name.Split(' ')[1]);

        if (current_id == 0)
        {
            // Require Fix: arouse per 5s?
            // Vector3 dest = new Vector3(Random.Range(-15.0f, 15.0f), 0, Random.Range(-15.0f, 15.0f));

            GetComponent<MeshRenderer>().material = AgentManager.instance.leader_mat;
            force = CalculateGoalForce() + CalculateWallForce();
        }
        else
        {
            var Leader = GameObject.Find("Agent 0");
            var goal = Leader.transform.position;
            ComputePath(goal);
            force = CalculateGoalForce() + CalculateAgentForce() + CalculateWallForce();

        }

        return force;
    }

    private Vector3 CalculatePanicGoalForce()
    {
        Vector3 goal = path.FirstOrDefault();
        var temp = goal - transform.position;
        var ei = (goal - transform.position).normalized;
        var eji =( AgentManager.instance.crowd_center - transform.position).normalized;
        var e0i = (1-Parameters.P)*ei + Parameters.P * eji;
        return (2 * Parameters.prefSpeed * e0i.normalized - rb.velocity) * (mass / Parameters.T);
    }

    private Vector3 CalculateCrowdForce()
    {

        var n_crowd = AgentManager.instance.n_crowd;
        Vector3 crowd_center = Vector3.zero;
        
        if (perceivedNeighbors.Count >= n_crowd)
        {
            isCrowded = true;
        }

        if (perceivedNeighbors.Count >= n_crowd)
        {
            AgentManager.instance.n_crowd = perceivedNeighbors.Count;
            n_crowd = AgentManager.instance.n_crowd;

            foreach (var neighbor in perceivedNeighbors)
            {
                crowd_center += neighbor.transform.position;
            }
            crowd_center = crowd_center / n_crowd;
            AgentManager.instance.crowd_center = crowd_center;
        }

        foreach (var neighbor in perceivedNeighbors)
        {
            Debug.DrawLine(transform.position, neighbor.transform.position, Color.yellow);
        }
        Debug.Log(perceivedNeighbors.Count);

        // bool isCrowded = false;

        // int current_id = int.Parse(name.Split(' ')[1]);

        // ComputePath(crowd_center);

        if (isCrowded)
        {
            // Require Fix: arouse per 5s?
            // Vector3 dest = new Vector3(Random.Range(-15.0f, 15.0f), 0, Random.Range(-15.0f, 15.0f));
            GetComponent<MeshRenderer>().material = AgentManager.instance.leader_mat;
        }
        else
        {
            GetComponent<MeshRenderer>().material = AgentManager.instance.normal_mat;
        }
        Vector3 force = CalculatePanicGoalForce() + CalculateAgentForce() + CalculateWallForce();

        return force;
    }

    private Vector3 CalculateSpiralForce()
    {
        //AgentManager.instance.destination = Vector3.forward * 100;

        Vector3 agentForce = Vector3.zero;

        Vector3 centerDir = Vector3.zero - transform.position;

        if (centerDir.magnitude > 0)
        {
            agentForce += Vector3.Cross(Vector3.up, centerDir).normalized * 0.05f;
            agentForce += centerDir.normalized * 0.005f;
        }

        return agentForce.normalized * Parameters.prefSpeed;
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
