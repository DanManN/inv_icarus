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

    void Start()
    {
        path = new List<Vector3>();
        rb = GetComponent<Rigidbody>();

        // gameObject.transform.localScale = new Vector3(2 * radius, 1, 2 * radius);
        // GetComponent<CapsuleCollider>().height = perceptionDistance;
        // GetComponent<CapsuleCollider>().radius = perceptionAngle;
        
        
        step = 70f;
        maxSteps = 1000;
        speed = 5.0f;
        obCol = new List<Collider>();
        goal = GameObject.Find("Goal");
        goal_pos = goal.transform.position;

        var obs = GameObject.FindGameObjectsWithTag("Planet");

        foreach (GameObject ob in obs)
        {
            obCol.Add(ob.GetComponent<Collider>());
        }
        
        GameObject ship = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        Vector3 shipPos = ship.transform.position;

        center = (goal_pos + shipPos) * 0.5f;
        radius = Vector3.Distance(goal_pos, shipPos) * 0.75f;
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
        //path = new List<Vector3>() { destination };
        // call rrt here
        //path = FindPath(destination);
        //path.Add(destination);
        path = FindPath(goal_pos);
        path.Add(goal_pos);
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    #endregion

    public Vector3 avoidTorque()
    {
        foreach (GameObject agent in perceivedNeighbors) {
            Vector3 disp = agent.transform.position - transform.position;
            Vector3 aVel = agent.GetComponent<Rigidbody>().velocity;


        }
        return Vector3.zero;
    }

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
        //print(totalThrust);
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
    
    public List<Vector3> FindPath(Vector3 destination)
    {
        tree = new Tree(transform.position);
        curSteps = 0;
        TreeNode curLeaf = null;

        while (curSteps < maxSteps)
        {
            curLeaf = NextLeaf();
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
            finalNode = tree.FindClosest(goal_pos, tree.root);
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
    
TreeNode NextLeaf()
    {
        Vector3 x;
        //MeshFilter m = floor.GetComponent<MeshFilter>();

        var dist = goal_pos - transform.position;

        Vector3 point = goal_pos;
        float direct = Random.Range(0.0f, 1.0f);
        if (direct < .9)
            point = (Vector3)Random.insideUnitSphere * radius + center;
        
        
        
        bool flag = false;

        //while (true)
        for(int q = 0; q < 1000; q++)
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
