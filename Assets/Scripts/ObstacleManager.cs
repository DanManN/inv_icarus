using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{

    public float obstacleNum = 10;
    public GameObject obstaclePrefab;
    public float radius = 500;

    private List<Agent> obstacles = new List<Agent>();
    private GameObject obstacleParent;
    private static HashSet<GameObject> obstacleObjs = new HashSet<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(0);

        obstacleParent = GameObject.Find("Obstacles");
        for (int i = 0; i < obstacleNum; i++)
        {
            SpawnRandom(i);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Generate random obstacle in radius of midpoint between player and goal
    public void SpawnRandom(int i)
    {
        //Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane+5)); //will get the middle of the screen
        GameObject obstacle = null;
        //Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0,Screen.width), Random.Range(0,Screen.height), Camera.main.farClipPlane/2));

        Vector3 goal = GameObject.Find("Goal").transform.position;
        GameObject ship = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        Vector3 shipPos = ship.transform.position;

        Vector3 center = (goal + shipPos) * 0.5f;

        Vector3 point = (Vector3)Random.insideUnitSphere * radius + center;
        
        var player = GameObject.FindGameObjectsWithTag("PlayCol");
        var planets = GameObject.FindGameObjectsWithTag("Planet");
        var obs = player.Concat(planets);
        var obCol = new List<Collider>();

        bool flag = false;

        foreach (GameObject ob in obs)
        {
            obCol.Add(ob.GetComponent<Collider>());
        }

        for (int q = 0; q < 1000; q++)
        {
            foreach (Collider col in obCol)
                if (col.bounds.Contains(point))
                {
                    flag = true;
                    break;
                }

            if (flag)
                point = (Vector3)Random.insideUnitSphere * radius + center;

            else break;
        }
            


        //Debug.Log("screen position" + point);

        // TODO: Add a random choice from different prefabs?
        obstacle = Instantiate(obstaclePrefab,point,Quaternion.identity);
 
        // This is not working?
        // obstacle.transform.localScale = new Vector3(10, 10, 10);

        obstacle.name = "Obstacle " + i;
        obstacle.transform.parent = obstacleParent.transform;
        var obstacleScript = obstacle.GetComponent<Agent>();

        obstacles.Add(obstacleScript);
        obstacleObjs.Add(obstacle);
    }


    #region Public Functions

    public static bool IsObstacle(GameObject obj)
    {
        return obstacleObjs.Contains(obj);
    }

    #endregion

}
