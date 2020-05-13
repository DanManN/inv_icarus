using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RRTAlgo : MonoBehaviour
{
    private GameObject goal = null;
    private List<Collider> obCol;
    private Vector3 center;
    private float radius; 

    Tree tree;

    public float step;
    public int maxSteps;
    int curSteps;
    TreeNode finalNode;

    Stack<TreeNode> points;
    Vector3 curDest;
    public float speed;
    Vector3 offset;
    bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        step = 20f;
        maxSteps = 10000;
        speed = 5.0f;
        obCol = new List<Collider>();
        goal = GameObject.Find("Goal");

        //var ast = GameObject.FindGameObjectsWithTag("SpaceTrash");
        var obs = GameObject.FindGameObjectsWithTag("Planet");
        //var obs = planets.Concat(ast);

        //offset = new Vector3(0, GetComponent<Collider>().bounds.extents.y, 0);
        offset = new Vector3(0, 0, 0);

        foreach (GameObject ob in obs)
        {
            obCol.Add(ob.GetComponent<Collider>());
        }
        
        Vector3 goal_pos = GameObject.Find("Goal").transform.position;
        GameObject ship = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        Vector3 shipPos = ship.transform.position;

        center = (goal_pos + shipPos) * 0.5f;
        radius = Vector3.Distance(goal_pos, shipPos) * 0.75f;

        FindPath();
        
    }

    void FindPath()
    {
        tree = new Tree(transform.position - offset);
        curSteps = 0;
        TreeNode curLeaf = null;

        while (curSteps < maxSteps)
        {
            curLeaf = NextLeaf();
            if (goal.GetComponent<Collider>().bounds.Contains(curLeaf.pos))
            {
                finalNode = curLeaf;
                break;
            }
            curSteps++;
        }

        if (curSteps >= maxSteps)
        {
            finalNode = tree.FindClosest(goal.transform.position, tree.root);
            Debug.DrawLine(curLeaf.pos, finalNode.pos, Color.green, 60f);

        }

        points = new Stack<TreeNode>();

        TreeNode cur = finalNode;
        while (cur != null)
        {
            points.Push(cur);
            cur = cur.parent;
        }

        curDest = points.Pop().pos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        if (canMove)
        {
            float dis = Vector3.Distance(transform.position - offset, curDest);
            if (dis > .01f)
            {

                if (points.Count == 0)
                {
                    //enable navmesh agent and get to the end
                }

                //transform.LookAt(new Vector3(0, 0, curDest.z));

                /*
                Vector3 targetDirection = curDest - transform.position;

                // The step size is equal to speed times frame time.
                float singleStep = 1.0f * Time.deltaTime;

                // Rotate the forward vector towards the target direction by one step
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

                // Draw a ray pointing at our target in
                Debug.DrawRay(transform.position, newDirection, Color.red);

                // Calculate a rotation a step closer to the target and applies rotation to this object
                transform.rotation = Quaternion.LookRotation(newDirection);
                *//*
                float walk = speed * Time.fixedDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, curDest + offset, walk);
                //print("still in this pos");

            }
            else if (points.Count > 0)
            {
                curDest = points.Pop().pos;
                //print("moving to next pos");
            }
        }
        else
        {
            float walk = -0.6f * speed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, curDest + offset, walk);
        }
        
        */
    }

    TreeNode NextLeaf()
    {
        Vector3 x;
        //MeshFilter m = floor.GetComponent<MeshFilter>();

        var dist = goal.transform.position - transform.position;

        Vector3 point = (Vector3)Random.insideUnitSphere * radius + center;
        
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
                    Debug.Log(col.gameObject.transform.parent.transform.parent.name);
                    break;
                }

            if (flag)
            {
                point = (Vector3)Random.insideUnitSphere * radius + center;
                Debug.DrawLine(closest.pos, nextStep, Color.red, 60f);
            }


            else
            {
                Debug.DrawLine(closest.pos, nextStep, Color.white, 60f);
                TreeNode newLeaf = new TreeNode(nextStep);
                closest.AddChild(newLeaf);
                return newLeaf;
            }
        }

        return null;
    }
/*
    void OnTriggerEnter(Collider col)
    {
        Vector3 otherPos = col.gameObject.transform.position;
        Vector3 goalPos = goal.transform.position;
        Vector3 myPos = transform.position;
        
        if(col.tag == "Player" && col.gameObject != gameObject && Vector3.Distance(otherPos, goalPos) < Vector3.Distance(myPos, goalPos))
        {
            Debug.Log(gameObject.name);
            StartCoroutine(Stopper());
        }
        
        //Debug.Log(col.name);
    }

    IEnumerator Stopper()
    {
        //Debug.Log("fkldsja;flsdjf");
        canMove = false;
        //FindPath(true);
        yield return new WaitForSeconds(0.4f);
        canMove = true;
        //yield return null;
    }
*/
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
  
