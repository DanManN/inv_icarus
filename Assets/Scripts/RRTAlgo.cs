using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RRTAlgo : MonoBehaviour
{
    float startX;
    float startY;

    public GameObject goal;

    float goalX;
    float goalY;

    GameObject[] obs;
    List<Collider> obCol;

    public GameObject floor;

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

    public int priorityNum;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        startX = transform.position.x;
        startY = transform.position.y;

        goal = GameObject.FindWithTag("Exit");
        floor = GameObject.FindWithTag("floor");
        step = 1.5f;
        maxSteps = 10000;
        speed = 5.0f;
        obCol = new List<Collider>();

        obs = GameObject.FindGameObjectsWithTag("SpaceTrash");

        //offset = new Vector3(0, GetComponent<Collider>().bounds.extents.y, 0);
        offset = new Vector3(0, 0, 0);

        foreach (GameObject ob in obs)
        {
            obCol.Add(ob.GetComponent<Collider>());
        }

        FindPath(false);
        
    }

    void FindPath(bool blocked)
    {
        tree = new Tree(transform.position - offset);
        curSteps = 0;
        TreeNode curLeaf = null;

        while (curSteps < maxSteps)
        {
            curLeaf = NextLeaf(blocked);
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
            Debug.DrawLine(curLeaf.pos, finalNode.pos, Color.green, 2.5f);

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
                */
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
    }

    TreeNode NextLeaf(bool blocked)
    {
        Vector3 x;
        MeshFilter m = floor.GetComponent<MeshFilter>();

        Vector3 min = m.mesh.bounds.min;
        Vector3 max = m.mesh.bounds.max;
        Vector3 nextStep;

        bool flag = false;

        while (true)
        {
            flag = false;
            if (blocked && Random.Range(0.0f, 1.0f) < 0.25f)
                x = goal.transform.position;
            else
                x = floor.transform.position - new Vector3(Random.Range(min.x * 5, max.x * 5), 0f/*floor.transform.position.y*/, Random.Range(min.z * 15, max.z * 15));

            TreeNode closest = tree.FindClosest(x, tree.root);

            //if (Vector3.Distance(FindWithTag("Exit").transform, closest.pos) < step)
            //    nextStep = Vector3.Lerp(closest.pos, x, step / (closest - x).Length());

            // nextStep = Vector3.Lerp(closest.pos, x,  step / (closest.pos - x).Length());
            nextStep = Vector3.Lerp(closest.pos, x, step / Vector3.Distance(closest.pos, x));

            

            foreach (Collider col in obCol)
                if (col.bounds.Contains(nextStep))
                    flag = true;

            if (flag)
            {
                //Debug.DrawLine(closest.pos, nextStep, Color.red, 30f);
                continue;
            }

            else
            {
                //Debug.DrawLine(closest.pos, nextStep, Color.white, 30f);
                TreeNode newLeaf = new TreeNode(new Vector3(nextStep.x, 0f, nextStep.z));
                closest.AddChild(newLeaf);
                return newLeaf;
            }
        }

        
    }

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
  
