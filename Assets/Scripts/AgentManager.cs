﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public int agentCount = 10;
    public float agentSpawnRadius = 20;
    public GameObject agentPrefab;
    public static Dictionary<GameObject, Agent> agentsObjs = new Dictionary<GameObject, Agent>();

    private static List<Agent> agents = new List<Agent>();
    private GameObject agentParent;
    public Vector3 destination;

    public static float UPDATE_RATE = 0.001f;
    private const int PATHFINDING_FRAME_SKIP = 25;

    public static AgentManager instance;

    public List<Camera> cams = new List<Camera>();

    #region Unity Functions

    void Awake()
    {
        instance = this;

        Random.InitState(0);

        agentParent = GameObject.Find("Agents");
        cams.Add(GameObject.Find("Main Camera").GetComponent<Camera>());
        for (int i = 0; i < agentCount; i++)
        {
            var randPos = new Vector3((Random.value - 0.5f) * agentSpawnRadius, (Random.value - 0.5f) * agentSpawnRadius, (Random.value - 0.5f) * agentSpawnRadius);

            GameObject agent = null;
            agent = Instantiate(agentPrefab, randPos, Quaternion.identity);
            agent.name = "Agent " + i;
            agent.transform.parent = agentParent.transform;
            var agentScript = agent.GetComponent<Agent>();

            cams.Add(agent.transform.Find("FlightCam").GetComponent<Camera>());

            agents.Add(agentScript);
            agentsObjs.Add(agent, agentScript);
        }

        StartCoroutine(Run());

    }


    void Update()
    {
        #region Visualization

        if (Input.GetMouseButtonDown(0))
        {
            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
            // var dir = point - Camera.main.transform.position;
            // RaycastHit rcHit;
            // if (Physics.Raycast(point, dir, out rcHit))
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rcHit;

            if (Physics.Raycast(ray, out rcHit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                point = rcHit.point;
                SetAgentDestinations(point);
                destination = point;
            }
        }

        #endregion
    }

    IEnumerator Run()
    {
        yield return null;

        for (int iterations = 0; ; iterations++)
        {
            if (iterations % PATHFINDING_FRAME_SKIP == 0)
            {
                // SetAgentDestinations(destination);
            }

            foreach (var agent in agents)
            {
                agent.ApplyThrust();
                agent.Steer();
            }

            if (UPDATE_RATE == 0)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(UPDATE_RATE);
            }
        }
    }

    #endregion

    #region Public Functions

    public static bool IsAgent(GameObject obj)
    {
        return agentsObjs.ContainsKey(obj);
    }

    public void SetAgentDestinations(Vector3 destination)
    {
        // NavMeshHit hit;
        // NavMesh.SamplePosition(destination, out hit, 10, NavMesh.AllAreas);
        foreach (var agent in agents)
        {
            if (agent.isSelected)
            {
                agent.ComputePath(destination);
            }

        }
    }

    public static void RemoveAgent(GameObject obj)
    {
        var agent = obj.GetComponent<Agent>();

        agents.Remove(agent);
        agentsObjs.Remove(obj);
    }

    #endregion

    #region Utility Classes

    private class Tuple<K, V>
    {
        public K Item1;
        public V Item2;

        public Tuple(K k, V v)
        {
            Item1 = k;
            Item2 = v;
        }
    }

    #endregion
}
