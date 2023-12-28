using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.XR;

public class ChaserAIScript : MonoBehaviour
{
    public Transform Playerpos;
    UnityEngine.AI.NavMeshAgent agent;
    public LayerMask whatIsObject;
    private ChaserStates CurrentState;

    public AudioSource ChaseSound;
    public AudioSource SeePlayerSound;

    private float lastTimeSinceChangeDestination;
    private bool bPrevChasingPlayer;
    private bool bChasingPlayer;
    public float timeBetweenChangeDestination;
    public float randomDestinationArea;
    public float RandomDestinationXOffset;
    public float RandomDestinationZOffset;

    enum ChaserStates
    {
        Roaming,
        Chasing,
        Attacking
    }


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        bChasingPlayer = true;

        CurrentState = ChaserStates.Chasing;
    }
    void FixedUpdate()
    {
        ChangeState();

        AIAction();
    }

    void AIAction()
    {
        switch(CurrentState)
        {
            case ChaserStates.Roaming:
                Roam();

                break;

            case ChaserStates.Chasing:
                agent.destination = Playerpos.position;

                break;

            case ChaserStates.Attacking:

                break;
        }
    }

    void ChangeState()
    {
        bool CanSeePlayer = !Physics.Linecast(transform.position, Playerpos.position, whatIsObject);
        Debug.DrawLine(transform.position, Playerpos.position); 

        if(CanSeePlayer) 
        {
            if(CurrentState == ChaserStates.Roaming)
            {
                EnterChase();
            }
        }

        else
        {
            if(CurrentState == ChaserStates.Chasing)
            {
                EnterRoam();
            }
        }
    }

    void Roam()
    {
        if(Time.time - lastTimeSinceChangeDestination > timeBetweenChangeDestination)
        {
            lastTimeSinceChangeDestination = Time.time;

            if(bPrevChasingPlayer)
            {
                bPrevChasingPlayer = false;
                bChasingPlayer = false;

                ChaseSound.enabled = false;

                agent.destination = new Vector3(RandomDestinationXOffset + Random.Range(-randomDestinationArea, randomDestinationArea), transform.position.y, RandomDestinationZOffset + Random.Range(-randomDestinationArea, randomDestinationArea));
            }

            else
            {
                if(bChasingPlayer)
                {
                    bChasingPlayer = false;

                    agent.destination = new Vector3(RandomDestinationXOffset + Random.Range(-randomDestinationArea, randomDestinationArea), transform.position.y, RandomDestinationZOffset + Random.Range(-randomDestinationArea, randomDestinationArea));
                }

                else
                {
                    bChasingPlayer = true;
                }
            }
        }

        if (bChasingPlayer && !bPrevChasingPlayer)
        {
            agent.destination = Playerpos.position;
        }
    }

    void EnterRoam()
    {
        CurrentState = ChaserStates.Roaming;

        lastTimeSinceChangeDestination = Time.time;
    }

    void EnterChase()
    {
        CurrentState = ChaserStates.Chasing;

        if(!bPrevChasingPlayer)
        {
            SeePlayerSound.Play();

            ChaseSound.enabled = true;
            ChaseSound.Play();
        }

        bPrevChasingPlayer = true;
    }
}