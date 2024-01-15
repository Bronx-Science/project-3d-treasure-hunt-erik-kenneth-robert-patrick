using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.XR;

public class CrawlerAIScript : AI
{
    public Transform Playerpos;
    UnityEngine.AI.NavMeshAgent agent;
    public LayerMask whatIsObject;
    private CrawlerStates CurrentState;

    public Animator AIAnimator;
    public CameraShake playerCameraShake;
    public StatComponent playerStats;

    public AudioSource ChaseSound;
    public AudioSource SeePlayerSound;
    public AudioSource SeePlayerSound1;
    public AudioSource SeePlayerSound2;

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

    private bool bNoStart;

    private float defaultspeed;

    enum CrawlerStates
    {
        Roaming,
        Chasing,
        Attacking
    }


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        defaultspeed = agent.speed;

        EnterRoam();

        ChaseSound.enabled = false;

        bNoStart = true;

        Invoke(nameof(StartMovement), 2);
    }
    void FixedUpdate()
    {
        if (bNoStart)
        {
            return;
        }

        ChangeState();

        AIAction();
    }

    void AIAction()
    {
        switch (CurrentState)
        {
            case CrawlerStates.Roaming:
                Roam();

                break;

            case CrawlerStates.Chasing:
                playerCameraShake.shakeDuration = 5;

                playerCameraShake.shakeAmount = Mathf.Lerp(0.75f, 0, distanceFromPlayer / 75);

                agent.destination = Playerpos.position;

                break;

            case CrawlerStates.Attacking:

                break;
        }
    }

    void ChangeState()
    {
        bool CanSeePlayer = SeePlayer();

        if (CanSeePlayer)
        {
            distanceFromPlayer = (transform.position - Playerpos.position).magnitude;

            if (distanceFromPlayer < attackRange && CurrentState == CrawlerStates.Chasing && agent.speed != 0)
            {
                EnterAttack();

                return;
            }

            if (CurrentState == CrawlerStates.Roaming)
            {
                EnterChase();

                return;
            }
        }

        else
        {
            if (CurrentState == CrawlerStates.Chasing)
            {
                EnterRoam();

                return;
            }
        }
    }

    void EnterAttack()
    {
        CurrentState = CrawlerStates.Attacking;

        PlayScream();

        AIAnimator.SetBool("Attacking", true);

        Invoke(nameof(ResetSpeed), 1);

        Invoke(nameof(ExitAttack), attackCooldown);

        Invoke(nameof(DamagePlayer), 1);
    }
    
    void ResetSpeed()
    {
        agent.speed = 0;
    }

    void DamagePlayer()
    {
        playerStats.Damage(attackDamage);

        AIAnimator.SetBool("Attacking", false);
    }

    void ExitAttack()
    {
        agent.speed = defaultspeed;

        EnterRoam();
    }

    void Roam()
    {
        if (Time.time - lastTimeSinceChangeDestination > timeBetweenChangeDestination)
        {
            lastTimeSinceChangeDestination = Time.time;

            if (bPrevChasingPlayer)
            {
                bPrevChasingPlayer = false;

                ChaseSound.enabled = false;

                agent.destination = new Vector3(RandomDestinationXOffset + Random.Range(-randomDestinationArea, randomDestinationArea), transform.position.y, RandomDestinationZOffset + Random.Range(-randomDestinationArea, randomDestinationArea));
            }

            else
            {
                if (bChasingPlayer)
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
        CurrentState = CrawlerStates.Roaming;

        bChasingPlayer = false;

        lastTimeSinceChangeDestination = Time.time;
    }

    void EnterChase()
    {
        CurrentState = CrawlerStates.Chasing;

        if (!bPrevChasingPlayer)
        {
            PlayScream();

            ChaseSound.enabled = true;
            ChaseSound.Play();
        }

        bPrevChasingPlayer = true;
    }

    void PlayScream()
    {
        int Num = Random.Range(0, 3);

        if (Num == 0)
        {
            SeePlayerSound.Play();

            return;
        }

        if (Num == 1)
        {
            SeePlayerSound1.time = 2;

            SeePlayerSound1.Play();

            return;
        }

        SeePlayerSound2.Play();
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

    void StartMovement()
    {
        bNoStart = false;
    }

    public void Stun(float StunDuration)
    {
        agent.speed = 0;

        PlayScream();

        AIAnimator.SetBool("Stunned", true);

        Invoke(nameof(ExitStun), StunDuration);
    }

    void ExitStun()
    {
        agent.speed = defaultspeed;

        AIAnimator.SetBool("Stunned", false);
    }
}
