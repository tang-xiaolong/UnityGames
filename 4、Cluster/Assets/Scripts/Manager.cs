using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public bool useTargetForce = false;
    public Transform agent;
    public int agentNum;
    public List<Agent> agentList = new List<Agent>();
    public float bound = 300;//世界的边界
    public GameObject target;

    private void Start()
    {
        CreateAgent();
    }

    private void CreateAgent()
    {
        Transform tf;
        Agent ag;
        for (int i = 0; i < agentNum; i++)
        {
            tf = Instantiate(agent, new Vector3(Random.Range(-bound, bound*1.0f), Random.Range(-bound, bound*1.0f), Random.Range(-bound, bound*1.0f)),Quaternion.identity);
            ag = tf.GetComponent<Agent>();
            if (null != ag)
                agentList.Add(ag);
        }
    }

    public List<Agent> GetNeighbours(Agent agent,float radius)
    {
        List<Agent> agents = new List<Agent>();
        int count = agentList.Count;
        for (int i = 0; i < count; i++)
        {
            if (agentList[i] != agent && Vector3.Distance(agent.mPos, agentList[i].mPos) <= radius)
                agents.Add(agentList[i]);
        }
        return agents;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            SetTarget();
        }
    }

    private void SetTarget()
    {
        if (target == null)
            return;
        target.transform.position = new Vector3(Random.Range(-bound, bound * 1.0f), Random.Range(-bound, bound * 1.0f), Random.Range(-bound, bound * 1.0f));
        for (int i = 0; i < agentNum; i++)
        {
            agentList[i]._target = target;
        }
    }
}
