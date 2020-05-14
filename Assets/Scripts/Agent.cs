// using System.Collections;
// using System;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float maxThrust = 2500f;
    // public float liftForce = 8f;
    // public float dragForce = 0.2f;
    public float maxTorque = 100000;

    public float perceptionDistance;
    public float perceptionAngle;

    private List<Vector3> path;
    private Rigidbody rb;
    // private CapsuleCollider vision;

    private HashSet<GameObject> perceivedNeighbors = new HashSet<GameObject>();
    private HashSet<GameObject> perceivedObstacles = new HashSet<GameObject>();


    private GameObject goal = null;
    private List<Collider> obCol;
    private Vector3 center;
    private float radius;
    private Vector3 goal_pos;

    Tree tree;

    public float step;
    public int maxSteps;
    int curSteps;
    TreeNode finalNode;

    Stack<TreeNode> points;
    public float speed;

    public bool isSelected = false;

    void Start()
    {
        path = new List<Vector3>();
        rb = GetComponent<Rigidbody>();
        // foreach (CapsuleCollider cc in GetComponents<CapsuleCollider>())
        // {
        //     if (cc.isTrigger)
        //         vision = cc;
        // }

        // gameObject.transform.localScale = new Vector3(2 * radius, 1, 2 * radius);
        // GetComponent<CapsuleCollider>().height = perceptionDistance;
        // GetComponent<CapsuleCollider>().radius = perceptionAngle;


        step = 5f;
        maxSteps = 5000;
        speed = 10.0f;
        obCol = new List<Collider>();
        goal = GameObject.Find("Goal");
        goal_pos = goal.transform.position;

        var obs = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject ob in obs)
        {
            obCol.Add(ob.GetComponent<Collider>());
        }

        GameObject ship = GameObject.FindGameObjectWithTag("Agent").transform.GetChild(0).gameObject;
        Vector3 shipPos = ship.transform.position;

        center = (goal_pos + shipPos) * 0.5f;
        radius = Vector3.Distance(goal_pos, shipPos) * 0.75f;
    }

    private void FixedUpdate()
    {
        // if (path.Count > 1)
        //     print("vision: " + vision + ", " + vision.bounds + ", " + vision.bounds.Contains(path[0]));
        // if (path.Count > 1 && vision.bounds.Contains(path[0]))
        if (path.Count > 0)
        {
            Vector3 disp = path[0] - transform.position;
            // if (disp.magnitude > 100f)
            // {
            //     ComputePath(goal_pos);
            // }
            if (disp.magnitude < 20 || Vector3.Dot(transform.forward, disp.normalized) > 0.5)
            {
                if (path.Count > 1 && disp.magnitude < 50f)
                {
                    // print("test");
                    path.RemoveAt(0);
                }
                else if (path.Count == 1 && disp.magnitude < 50f)
                {
                    // path.RemoveAt(0);
                    // print("reached goal");
                    if (path.Count == 0)
                    {
                        // gameObject.SetActive(false);
                        // AgentManager.RemoveAgent(gameObject);
                    }
                }
            }
        }
    }

    private void Update()
    {
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
        //path = new List<Vector3>() { destination };
        // call rrt here
        path = FindPath(destination);
        path.Add(destination);
        //path = FindPath(goal_pos);
        //path.Add(goal_pos);
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    #endregion

    public Vector3 avoidObsTorque()
    {
        Vector3 torque = Vector3.zero;
        foreach (GameObject obs in perceivedObstacles)
        {
            Vector3 disp = obs.transform.position - transform.position;
            Vector3 xyDisp = Vector3.ProjectOnPlane(disp, transform.forward);

            torque += -Vector3.Cross(transform.forward, xyDisp / 5);
        }
        return torque;
    }
    public Vector3 avoidAgtTorque()
    {
        Vector3 torque = Vector3.zero;
        foreach (GameObject agt in perceivedNeighbors)
        {
            Vector3 disp = agt.transform.position - transform.position;
            Vector3 aVel = agt.GetComponent<Rigidbody>().velocity;
            Vector3 xyDisp = Vector3.ProjectOnPlane(disp, transform.forward);

            torque += -Vector3.Cross(transform.forward, xyDisp / 5);
        }
        return torque;
    }

    public Vector3 goalTorque()
    {
        if (path.Count < 1)
            return Vector3.zero;

        Vector3 disp = path[0] - transform.position;
        Vector3 zVel = Vector3.Project(rb.velocity, disp);
        Vector3 xyVel = rb.velocity - zVel;
        Vector3 goalVel = disp / 2;

        // return Vector3.Cross(transform.forward, disp.normalized).normalized;

        // if (xyVel.magnitude > goalVel.magnitude / 4 && Vector3.Dot(rb.velocity, disp.normalized) < Vector3.Cross(rb.velocity, disp.normalized).magnitude)
        if (rb.velocity.magnitude > 2.5 && 1.5 * Vector3.Dot(rb.velocity, disp.normalized) < Vector3.Cross(rb.velocity, disp.normalized).magnitude)
        {
            // float angle = Vector3.Angle(rb.velocity, transform.forward);
            // if (Mathf.Abs(angle) < 1) return Vector3.zero;
            return Vector3.Cross(transform.forward, -xyVel).normalized;
        }
        else
        {
            // float angle = Vector3.Angle(disp, transform.forward);
            // if (Mathf.Abs(angle) < 1) return Vector3.zero;
            return Vector3.Cross(transform.forward, disp.normalized).normalized;
        }
    }

    public float goalThrust()
    {
        if (path.Count < 1)
            return -0.0f;

        Vector3 disp = path[0] - transform.position;
        Vector3 zVel = Vector3.Project(rb.velocity, disp);
        Vector3 xyVel = rb.velocity - zVel;
        // Vector3 idealForce = (disp.normalized - xyVel); //> rb.mass;// / AgentManager.UPDATE_RATE;

        // if (xyVel.magnitude > 1.5 && Vector3.Dot(rb.velocity, disp.normalized) < Vector3.Cross(rb.velocity, disp.normalized).magnitude)
        // {
        //     // return Vector3.Dot(transform.forward, -xyVel);
        //     idealForce = (0.1f * disp - 1f * xyVel); //> rb.mass;// / AgentManager.UPDATE_RATE;
        // }
        // else
        // {
        //     idealForce = (0.1f * disp - 0f * xyVel); //> rb.mass;// / AgentManager.UPDATE_RATE;
        // }

        Vector3 goalVel = disp / 4;
        Vector3 idealForce = goalVel - 0.75f * zVel - xyVel;
        return Vector3.Dot(idealForce, transform.forward);
    }

    public void ApplyThrust()
    {
        float totalThrust = 500 * goalThrust();
        //print(totalThrust);
        rb.AddForce(Vector3.ClampMagnitude(totalThrust * transform.forward, maxThrust));
    }

    public void Steer()
    {
        Vector3 totalTorque = 10000 * goalTorque() + 50 * avoidObsTorque() + 15 * avoidAgtTorque();
        Debug.DrawLine(transform.position, transform.position + 5 * goalTorque().normalized, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + 5 * avoidObsTorque().normalized, Color.red);
        Debug.DrawLine(transform.position, transform.position + 5 * avoidAgtTorque().normalized, Color.blue);
        rb.AddTorque(Vector3.ClampMagnitude(totalTorque, maxTorque));
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

    public List<Vector3> FindPath(Vector3 destination)
    {
        tree = new Tree(transform.position);
        curSteps = 0;
        TreeNode curLeaf = null;

        while (curSteps < maxSteps)
        {
            curLeaf = NextLeaf(destination);
            //if (goal.GetComponent<Collider>().bounds.Contains(curLeaf.pos))
            if (Vector3.Distance(destination, curLeaf.pos) < 100)
            {
                finalNode = curLeaf;
                break;
            }
            curSteps++;
        }

        if (curSteps >= maxSteps)
        {
            finalNode = tree.FindClosest(destination, tree.root);
            Debug.DrawLine(curLeaf.pos, finalNode.pos, Color.green, 60f);

        }

        points = new Stack<TreeNode>();
        List<Vector3> positions = new List<Vector3>();

        TreeNode cur = finalNode;
        while (cur != null)
        {
            points.Push(cur);
            positions.Add(cur.pos);
            cur = cur.parent;
        }

        //curDest = points.Pop().pos;
        positions.Reverse();
        return positions;
    }

    TreeNode NextLeaf(Vector3 destination)
    {
        // Vector3 x;
        //MeshFilter m = floor.GetComponent<MeshFilter>();

        var dist = destination - transform.position;

        Vector3 point = destination;
        float direct = Random.Range(0.0f, 1.0f);
        if (direct < .9)
            point = (Vector3)Random.insideUnitSphere * radius + center;



        bool flag = false;

        //while (true)
        for (int q = 0; q < 1000; q++)
        {
            TreeNode closest = tree.FindClosest(point, tree.root);
            var nextStep = Vector3.Lerp(closest.pos, point, step / Vector3.Distance(closest.pos, point));
            flag = false;

            foreach (Collider col in obCol)
                if (col.bounds.Contains(nextStep))
                {
                    flag = true;
                    //Debug.Log(col.gameObject.transform.parent.transform.parent.name);
                    break;
                }

            if (flag)
            {
                point = destination;
                direct = Random.Range(0.0f, 1.0f);
                if (direct < .9)
                    point = (Vector3)Random.insideUnitSphere * radius + center;
                //Debug.DrawLine(closest.pos, nextStep, Color.red, 60f);
            }


            else
            {
                //Debug.DrawLine(closest.pos, nextStep, Color.white, 60f);
                TreeNode newLeaf = new TreeNode(nextStep);
                closest.AddChild(newLeaf);
                return newLeaf;
            }

            if (q == 999)
            {
                TreeNode newLeaf = new TreeNode(nextStep);
                closest.AddChild(newLeaf);
                return newLeaf;
            }
        }

        return null;
    }
}


public class TreeNode
{
    public Vector3 pos;
    public TreeNode parent;
    public List<TreeNode> children;

    public TreeNode(Vector3 v)
    {
        parent = null;
        children = new List<TreeNode>();
        pos = v;
    }

    public void AddChild(TreeNode t)
    {
        t.parent = this;
        children.Add(t);
    }
}

public class Tree
{
    public TreeNode root;

    public Tree(Vector3 v)
    {
        root = new TreeNode(v);
    }

    public TreeNode FindClosest(Vector3 v, TreeNode t)
    {
        TreeNode close = t;
        float curDist = Vector3.Distance(v, t.pos);

        foreach (TreeNode c in t.children)
        {
            TreeNode temp = FindClosest(v, c);
            float tempDist = Vector3.Distance(temp.pos, v);

            if (tempDist < curDist)
            {
                curDist = tempDist;
                close = temp;
            }
        }

        return close;
    }

}
