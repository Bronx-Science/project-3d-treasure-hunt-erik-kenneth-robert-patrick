using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.XR;

public class ChaserAIScript : AI
{
    public Transform Playerpos;
    UnityEngine.AI.NavMeshAgent agent;
    public LayerMask whatIsObject;
    private ChaserStates CurrentState;

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

    private int Num;

    public float Health;

    private bool bDead;

    enum ChaserStates
    {
        Roaming,
        Chasing,
        Attacking
    }


    // Start is called before the first frame update
    void Start()
    {
        Num = -1;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        defaultspeed = agent.speed;

        EnterRoam();

        ChaseSound.enabled = false;

        bNoStart = true;

        Invoke(nameof(StartMovement), 2);
    }
    void FixedUpdate()
    {
        if (bNoStart || bDead)
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
        //Debug.DrawLine(transform.position, Playerpos.position);

        if (CanSeePlayer)
        {
            distanceFromPlayer = (transform.position - Playerpos.position).magnitude;

            if (distanceFromPlayer < attackRange && CurrentState == ChaserStates.Chasing && agent.speed != 0)
            {
                EnterAttack();

                return;
            }

            if (CurrentState == ChaserStates.Roaming)
            {
                EnterChase();

                return;
            }
        }

        else
        {
            if (CurrentState == ChaserStates.Chasing)
            {
                EnterRoam();

                return;
            }
        }
    }

    void EnterAttack()
    {
        CurrentState = ChaserStates.Attacking;

        PlayScream();

        AIAnimator.SetBool("Attacking", true);

        Invoke(nameof(ExitAttack), attackCooldown);

        Invoke(nameof(DamagePlayer), 1);
    }

    void DamagePlayer()
    {
        playerStats.Damage(attackDamage);
    }

    void ExitAttack()
    {
        AIAnimator.SetBool("Attacking", false);

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
        CurrentState = ChaserStates.Roaming;

        bChasingPlayer = false;

        lastTimeSinceChangeDestination = Time.time;
    }

    void EnterChase()
    {
        CurrentState = ChaserStates.Chasing;

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
        Num++;

        if (Num == 0)
        {
            SeePlayerSound.Play();

            return;
        }

        if (Num == 1)
        {
            SeePlayerSound1.Play();

            return;
        }

        Num = -1;

        SeePlayerSound2.time = 1.5f;
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

    public void Damage(float damage)
    {
        Health -= damage;

        if(Health < 0)
        {
            Die();
        }
    }

    void Die()
    {
        PlayScream();

        agent.speed = 0;

        bDead = true;

        Invoke(nameof(DelayDeath), 2);
    }

    void DelayDeath()
    {
        Destroy(gameObject);
    }
}
