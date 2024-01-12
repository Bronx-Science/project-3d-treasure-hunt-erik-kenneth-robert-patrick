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

    public CameraShake playerCameraShake;
    public StatComponent playerStats;

    public AudioSource ChaseSound;
    public AudioSource SeePlayerSound;

    private float lastTimeSinceChangeDestination;
    private bool bPrevChasingPlayer;
    private bool bChasingPlayer;
    private float distanceFromPlayer;

    public float fovDist;
    public float fovAngle;

    public float timeBetweenChangeDestination;
    public float randomDestinationArea;
    public float RandomDestinationXOffset;
    public float RandomDestinationZOffset;

    public float attackRange;
    public float attackCooldown;
    public float attackDamage;

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

        EnterRoam();

        ChaseSound.enabled = false;
    }
    void FixedUpdate()
    {
        if(Time.time < 2)
        {
            return;
        }

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
                playerCameraShake.shakeDuration = 5;

                playerCameraShake.shakeAmount = Mathf.Lerp(0.75f, 0, distanceFromPlayer / 75);

                agent.destination = Playerpos.position;

                break;

            case ChaserStates.Attacking:
                agent.destination = transform.position;

                break;
        }
    }

    void ChangeState()
    {
        bool CanSeePlayer = SeePlayer();
        Debug.DrawLine(transform.position, Playerpos.position); 

        if(CanSeePlayer) 
        {
            distanceFromPlayer = (transform.position - Playerpos.position).magnitude;

            if(distanceFromPlayer < attackRange && CurrentState == ChaserStates.Chasing)
            {
                EnterAttack();

                return;
            }

            if(CurrentState == ChaserStates.Roaming)
            {
                EnterChase();

                return;
            }
        }

        else
        {
            if(CurrentState == ChaserStates.Chasing)
            {
                EnterRoam();

                return;
            }
        }
    }

    void EnterAttack()
    {
        CurrentState = ChaserStates.Attacking;

        Invoke(nameof(ExitAttack), attackCooldown);

        Invoke(nameof(DamagePlayer), 1);
    }

    void DamagePlayer()
    {
        playerStats.Damage(attackDamage);
    }

    void ExitAttack()
    {
        EnterRoam();
    }

    void Roam()
    {
        if(Time.time - lastTimeSinceChangeDestination > timeBetweenChangeDestination)
        {
            lastTimeSinceChangeDestination = Time.time;

            if(bPrevChasingPlayer)
            {
                bPrevChasingPlayer = false;

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

        if (bChasingPlayer || bPrevChasingPlayer)
        {
            agent.destination = Playerpos.position;
        }
    }

    void EnterRoam()
    {
        CurrentState = ChaserStates.Roaming;

        bChasingPlayer = false;

        lastTimeSinceChangeDestination = Time.time;
    }

    void EnterChase()
    {
        CurrentState = ChaserStates.Chasing;

        if (!bPrevChasingPlayer)
        {
            SeePlayerSound.Play();

            ChaseSound.enabled = true;
            ChaseSound.Play();
        }

        bPrevChasingPlayer = true;
    }

    bool SeePlayer()
    {
        Vector3 direction = Playerpos.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (!Physics.Linecast(transform.position, Playerpos.position, whatIsObject) && direction.magnitude < fovDist && angle < fovAngle)
        {
            return true;
        }       

        return false;
    }
}
