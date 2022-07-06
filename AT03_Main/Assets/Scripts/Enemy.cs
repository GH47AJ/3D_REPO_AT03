using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : FiniteStateMachine, IInteractable
{
    [SerializeField] private bool debug = false;
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float stunCooldown = 3f;
    [SerializeField] private IdleState idleState;
    [SerializeField] private WanderState wanderState;
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private StunState stunState;

    private float cooldownTimer = -1;


    public NavMeshAgent Agent { get; private set; }
    public PlayerController Player { get; private set; }
    public float ViewRadius { get { return viewRadius; } }
    public float DefaultSpeed { get; private set; }
    public float DistanceToPlayer
    {
        get
        {
            if(Player != null)
            {
                return Vector3.Distance(transform.position, Player.transform.position);
            }
            else
            {
                return -1;
            }
        }
    }
    public bool ForceChasePlayer { get; private set; }
    public Animator Anim { get; private set; }
    public AudioSource AudioSource { get; private set; }
    public IdleState Idle { get { return idleState; } }
    public WanderState Wander { get { return wanderState; } }
    public ChaseState Chase { get { return chaseState; } }
    public StunState Stun { get { return stunState; } }

    private void Awake()
    {
        if(TryGetComponent(out NavMeshAgent agent) == true)
        {
            Agent = agent;
        }
        if (TryGetComponent(out AudioSource aSrc) == true)
        {
            AudioSource = aSrc;
        }
        if (transform.GetChild(0).TryGetComponent(out Animator anim) == true)
        {
            Anim = anim;
        }
        Player = FindObjectOfType<PlayerController>();
        idleState = new IdleState(this, idleState);
        wanderState = new WanderState(this, wanderState);
        chaseState = new ChaseState(this, chaseState);
        stunState = new StunState(this, stunState);
        ObjectiveItem.ObjectiveActivatedEvent += TriggerForceChasePlayer;
        EndTrigger.VictoryEvent += delegate { SetState(new GameOverState(this)); };
    }
    // Start is called before the first frame update
    void Start()
    {
        DefaultSpeed = Agent.speed;
        SetState(idleState);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(DistanceToPlayer <= viewRadius)
        {
            if(CurrentState.GetType() != typeof(ChaseState) && CurrentState.GetType() != typeof(GameOverState) && CurrentState.GetType() != typeof(StunState))
            {
                SetState(Chase);
            }
        }

        if(cooldownTimer >= 0)
        {
            cooldownTimer += Time.deltaTime;
            if(cooldownTimer >= stunCooldown)
            {
                cooldownTimer = -1;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(debug == true)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
            wanderState.DrawStateGizmos();
        }
    }

    private void TriggerForceChasePlayer()
    {
        if(ForceChasePlayer == false)
        {
            ForceChasePlayer = true;
            SetState(Chase);
        }
    }

    public void Activate()
    {
        if(cooldownTimer < 0 && CurrentState.GetType() != typeof(StunState))
        {
            StartCoroutine(TriggerStun());
        }
    }

    private IEnumerator TriggerStun()
    {
        SetState(Stun);
        yield return new WaitForSeconds(Stun.StunTime);
        cooldownTimer = 0;
    }
}

public abstract class EnemyBehaviour : IState
{
    protected Enemy Instance { get; private set; }

    public EnemyBehaviour(Enemy instance)
    {
        Instance = instance;
    }
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public virtual void OnStateExit() { }
    public virtual void DrawStateGizmos() { }
}

[System.Serializable]
public class IdleState : EnemyBehaviour
{
    [SerializeField] private Vector2 idleRange;
    [SerializeField] private AudioClip idleClip;

    private float idleTime = 0;
    private float timer = -1;

    public IdleState(Enemy instance, IdleState idle) : base(instance)
    {
        idleRange = idle.idleRange;
        idleClip = idle.idleClip;
    }
    public override void OnStateEnter()
    {
        Instance.Agent.isStopped = true;
        idleTime = Random.Range(idleRange.x, idleRange.y);
        timer = 0;
        Instance.AudioSource.PlayOneShot(idleClip); //this will allow for other sounds to play.
        Instance.Anim.SetBool("isMoving", false);
        Instance.Anim.SetBool("isChasing", false);
        Instance.Anim.SetBool("isStunned", false);
        Instance.Anim.SetBool("isIdle", true);
    }

    public override void OnStateUpdate()
    {
        if(timer >= 0)
        {
            timer += Time.deltaTime;
            if(timer >= idleTime)
            {
                timer = -1;
                Instance.SetState(Instance.Wander);
            }
        }
    }
}

[System.Serializable]
public class WanderState : EnemyBehaviour
{
    [SerializeField] private Bounds boundBox;
    [SerializeField] private AudioClip wanderClip;

    private Vector3 targetPosition;

    public WanderState(Enemy instance, WanderState wander) : base(instance)
    {
        boundBox = wander.boundBox;
        wanderClip = wander.wanderClip;
    }
    public override void OnStateEnter()
    {
        targetPosition = GetRandomPositionInBounds();
        while(NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas) == false)
        {
            targetPosition = GetRandomPositionInBounds();
        }
        Instance.Agent.isStopped = false;
        Instance.Agent.SetDestination(targetPosition);
        Instance.Anim.SetBool("isChasing", false);
        Instance.Anim.SetBool("isStunned", false);
        Instance.Anim.SetBool("isIdle", false);
        Instance.Anim.SetBool("isMoving", true);
        Instance.AudioSource.PlayOneShot(wanderClip);
    }

    public override void OnStateUpdate()
    {
        if(Vector3.Distance(Instance.transform.position, targetPosition) <= Instance.Agent.stoppingDistance)
        {
            Instance.SetState(Instance.Idle);
        }
    }

    public override void DrawStateGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boundBox.center, boundBox.size);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(targetPosition, 0.3f);
    }

    private Vector3 GetRandomPositionInBounds()
    {
        Vector3 randomPos = boundBox.center + new Vector3(
            Random.Range(-boundBox.extents.x, boundBox.extents.x), 
            Random.Range(-boundBox.extents.y, boundBox.extents.y),
            Random.Range(-boundBox.extents.z, boundBox.extents.z));
        return randomPos;
    }
}

public class GameOverState : EnemyBehaviour
{
    public GameOverState(Enemy instance) : base(instance)
    {

    }
    public override void OnStateEnter()
    {
        if (Instance != null) 
        {
            if (Instance.DistanceToPlayer <= Instance.Agent.stoppingDistance)
            {
                HUD.Instance.ActivateEndPrompt(false);
            }
            Instance.Agent.isStopped = true;
        }
        PlayerController.canMove = false;
        MouseLook.mouseLookEnabled = false;
    }

    public override void OnStateUpdate()
    {
        
    }
}

[System.Serializable]
public class ChaseState : EnemyBehaviour
{
    [SerializeField] private float chaseSpeed = 7f;
    [SerializeField] private AudioClip chaseClip;

    public ChaseState(Enemy instance, ChaseState chase) : base(instance)
    {
        chaseSpeed = chase.chaseSpeed;
        chaseClip = chase.chaseClip;
    }

    public override void OnStateEnter()
    {
        Instance.Agent.isStopped = false;
        Instance.Agent.speed = chaseSpeed;
        Instance.Anim.SetBool("isMoving", false);
        Instance.Anim.SetBool("isStunned", false);
        Instance.Anim.SetBool("isIdle", false);
        Instance.Anim.SetBool("isChasing", true);
        Instance.AudioSource.PlayOneShot(chaseClip);
    }

    public override void OnStateUpdate()
    {
        if(Instance.DistanceToPlayer <= Instance.ViewRadius)
        {
            if(Instance.DistanceToPlayer <= Instance.Agent.stoppingDistance)
            {
                Instance.SetState(new GameOverState(Instance));
                EndTrigger.ResetEvents();
                ObjectiveItem.ResetEvents();
            }
            else
            {
                Instance.Agent.SetDestination(Instance.Player.transform.position);
            }
        }
        else
        {
            if (Instance.ForceChasePlayer == false)
            {
                Instance.SetState(Instance.Wander);
            }
            else
            {
                Instance.Agent.SetDestination(Instance.Player.transform.position);
            }
        }
    }

    public override void OnStateExit()
    {
        Instance.Agent.speed = Instance.DefaultSpeed;
    }
}

[System.Serializable]
public class StunState : EnemyBehaviour
{
    [SerializeField] private float stunTime;
    [SerializeField] private AudioClip stunClip;

    private float timer = -1;

    public float StunTime { get { return stunTime; } }

    public StunState(Enemy instance, StunState stun) : base(instance)
    {
        stunTime = stun.stunTime;
        stunClip = stun.stunClip;
    }
    public override void OnStateEnter()
    {
        Instance.Agent.isStopped = true;
        timer = 0;
        Instance.Anim.SetBool("isMoving", false);
        Instance.Anim.SetBool("isIdle", false);
        Instance.Anim.SetBool("isChasing", false);
        Instance.Anim.SetBool("isStunned", true);
        Instance.AudioSource.PlayOneShot(stunClip);
    }

    public override void OnStateUpdate()
    {
        if(timer >= 0)
        {
            timer += Time.deltaTime;
            if(timer >= stunTime)
            {
                timer = -1;
                if(Instance.ForceChasePlayer == false)
                {
                    Instance.SetState(Instance.Wander);
                }
                else
                {
                    Instance.SetState(Instance.Chase);
                }
            }

        }
    }
}
