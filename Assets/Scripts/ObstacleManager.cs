using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{

    public float obstacleNum = 10;
    public GameObject obstaclePrefab;

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

    // Generate random obstacle in Camera View
    public void SpawnRandom(int i)
    {
        //Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane+5)); //will get the middle of the screen
        GameObject obstacle = null;
        Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0,Screen.width), Random.Range(0,Screen.height), Camera.main.farClipPlane/2));

        // TODO: Add a random choice from different prefabs?
        obstacle = Instantiate(obstaclePrefab,screenPosition,Quaternion.identity);
 
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
