using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINavMeshScipt : MonoBehaviour
{
    public Transform Playerpos;
    UnityEngine.AI.NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    void Update()
    {

        agent.destination = Playerpos.position;

    }
}
