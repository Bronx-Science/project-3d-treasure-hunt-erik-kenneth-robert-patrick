using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerAIScript : AI
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

    public void Stun(float StunDuration)
    {
        agent.speed = 0;

        //PlayScream();

        Invoke(nameof(ExitStun), StunDuration);
    }

    void ExitStun()
    {
        //agent.speed = defaultspeed;
    }
}
