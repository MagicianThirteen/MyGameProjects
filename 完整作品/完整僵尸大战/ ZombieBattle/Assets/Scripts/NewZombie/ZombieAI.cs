using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieAI : MonoBehaviour {
    private Transform zombieTransform;
    //用来记录当前状态的
    public FSMState currentState;
    //僵尸感知范围内的玩家
    private Transform targetPlayer;
    //zombiesensor组件
    private ZombieSensor zombieSensor;
    //zombieHealth组件
    private ZombieHealth zombieHealth;
    //ZombieRender组件
    private ZombieRender zombieRender;
    //导航代理组件
    private NavMeshAgent agent;
    //游荡状态下随机选择目标位置的范围
    public float wanderScope = 15.0f;
    //中枪后的搜索距离
    public float seekDistance = 25.0f;
    //僵尸攻击的距离
    public float attackRange = 1.5f;
    //僵尸攻击的夹角
    public float attackFieldOfView = 60.0f;
    //僵尸攻击的间隔
    public float attackInterval = 0.8f;
    //僵尸的攻击力
    public int attackDamage = 10;
    //僵尸的攻击音效
    public AudioClip zombieAttackAudio;
    //游荡的速度
    public float wanderSpeed = 0.9f;
    //奔跑速度
    public float runSpeed = 4.0f;
    //僵尸当前的速度
    public float currentSpeed = 0.0f;
    //动画控制器
    private Animator animator;
    //僵尸上一次停留位置
    private Vector3 previousPos = Vector3.zero;
    //僵尸的停留时间
    private float stopTime = 0;
    //僵尸攻击计时器
    private float attackTimer = 0.0f;
    //僵尸尸体消失计时器
    private bool disappeared = false;
    //僵尸是否已经消失
    private float disappearTimer = 0.0f;
    //是否是首次进入死亡
    private bool firstInDead = true;
    //僵尸消失的时间
    public float disappearTime = 3.0f;
    //是否自动初始化僵尸状态
    public bool autoInit = false;

    public enum FSMState
    {
        Wander,
        Seek,
        Chase,
        Attack,
        Dead,
    }

    private void OnEnable()
    {
        //组件，状态，都要初始化
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        zombieSensor = GetComponentInChildren<ZombieSensor>();
        zombieRender = GetComponent<ZombieRender>();
        zombieHealth = GetComponent<ZombieHealth>();
        zombieTransform = transform;
        targetPlayer = null;
        currentState = FSMState.Dead;
        agent.enabled = false;
        if (autoInit)
        {
            Born();
        }
       
    }
    //在指定位置初始化僵尸
    public void Born(Vector3 pos)
    {
        zombieTransform.position = pos;
        Born();
    }

    public void Born()
    {
        //初始化，感知玩家，初始状态，生命值，导航代理组件，动画控制器，死亡初始设置
        targetPlayer = null;
        currentState = FSMState.Wander;
        zombieHealth.currentHP = zombieHealth.maxHP;
        agent.enabled = true;
        agent.ResetPath();
        animator.applyRootMotion = false;
        GetComponent<CapsuleCollider>().enabled = true;
        animator.SetTrigger("toReborn");
        disappearTimer = 0;
        disappeared = false;
        firstInDead = true;
        currentState = FSMState.Wander;
    }

     void Disable()
    {
        zombieTransform.gameObject.SetActive(false);
    }

    private void FixedUpdate()//每隔一段时间根据情况更新状态
    {
        FSMUpdate();
    }

    void FSMUpdate()
    {
        switch (currentState)
        {
            case FSMState.Wander:
                UpdateWanderState();
                break;
            case FSMState.Seek:
                UpdateSeekState();
                break;
            case FSMState.Chase:
                UpdateChaseState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;

        }

        if (currentState != FSMState.Dead && !zombieHealth.IsAlive)
        {
            currentState = FSMState.Dead;
        }

    }
    //判断上次是否到达目的地
    protected bool AgentDone()
    {
        return !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance;

    }

    //限制僵尸当前移动速度，更新动画状态机
    //传过来不同速度，是为了不同状态使用不同速度
    //desiredVelocity期望速度，是代理的最大速度，包括了潜在因素
    private void setMaxAgentSpeed(float maxSpeed)
    {
        Vector3 targetVelocity = Vector3.zero;
        if (agent.desiredVelocity.magnitude > maxSpeed)
        {
            targetVelocity = agent.desiredVelocity.normalized * maxSpeed;

        }
        else
        {
            targetVelocity = agent.desiredVelocity;
        }
        agent.velocity = targetVelocity;
        currentSpeed = agent.velocity.magnitude;//找出长度赋给当前速度
        //设置动画状态
        animator.SetFloat("Speed", currentSpeed);

    }

    //计算僵尸在某个位置附近停留的时间
    private void caculateStopTime()
    {
        //初始的时候
        if (previousPos == Vector3.zero)
        {
            previousPos = zombieTransform.position;
        }
        else
        {
            Vector3 posDiff = zombieTransform.position - previousPos;
            if (posDiff.magnitude > 0.5)
            {
                previousPos = zombieTransform.position;
                stopTime = 0.0f;

            }
            else
            {
                stopTime += Time.deltaTime;
            }
        }

    }

    //游荡状态处理函数
    void UpdateWanderState()
    {
        //感知到周围有活着的玩家(注意zombiesensor是组件
        targetPlayer = zombieSensor.getNearbyPlayer();
        if (targetPlayer != null)
        {
            currentState = FSMState.Chase;
            agent.ResetPath();//切换别的状态，要重新设置位置
            return;
        }
        //受到伤害变成进入搜索状态
        if (zombieHealth.getDamage)
        {
            currentState = FSMState.Seek;
            agent.ResetPath();
            return;
        }
        if (AgentDone())
        {
            Vector3 randomRange = new Vector3(
                (Random.value - 0.5f) * 2 * wanderScope,
                0,
                (Random.value - 0.5f) * 2 * wanderScope

                );
            Vector3 nextDestination = zombieTransform.position + randomRange;
            agent.destination = nextDestination;
        }
        setMaxAgentSpeed(wanderSpeed);
        //如果计算的位置不能到达，或者卡住了，要计算卡住的时间
        //再选择背后一个位置做下一个目标
        caculateStopTime();
        if (stopTime > 1.0f)
        {
            Vector3 nextDestination =
                zombieTransform.position
                - zombieTransform.forward * (Random.value) * wanderScope;
            agent.destination = nextDestination;
        }

        //
        if (zombieRender != null)
        zombieRender.SetNormal();




    }

    //搜索状态处理函数
    void UpdateSeekState()
    {
        /*感知有玩家进入追踪状态，如果遭遇开枪，向所在位置移动，
         达到最大搜索距离或卡住时，后面还要计算停留时间。变成随机游荡状态，搜索设置最大速度
         runspeed*/
        targetPlayer = zombieSensor.getNearbyPlayer();
        if (targetPlayer != null)
        {
            currentState = FSMState.Chase;
            agent.ResetPath();
            return;//return是返回当前函数
        }
        if (zombieHealth.getDamage)
        {
            Vector3 seekDirection =
                zombieHealth.damageDirection;
            agent.destination = zombieTransform.position +
                seekDirection * seekDistance;//方向*最大搜索距离
            zombieHealth.getDamage = false;//表示已经处理了这次中枪

        }

        if (AgentDone() || stopTime > 1.0f)
        {
            currentState = FSMState.Wander;
            agent.ResetPath();
            return;
        }

        setMaxAgentSpeed(runSpeed);
     
        if (zombieRender != null)
        {
            zombieRender.SetCrazy();
        }
        caculateStopTime();
        
    }

    //追踪状态处理函数
    void UpdateChaseState()
    {
        /*如果僵尸的感知范围内没有玩家，进入游荡状态
         * 如果玩家与僵尸的距离，小于僵尸的攻击距离，进入攻击状态
         * 如果玩家与僵尸的距离，大于僵尸攻击距离，将追踪目标设置
         * 成玩家
         * 限制追踪速度
         * 进入狂暴状态
         * 计算停留时间
         */
        targetPlayer = zombieSensor.getNearbyPlayer();
        if (targetPlayer == null)
        {
            currentState = FSMState.Wander;
            agent.ResetPath();
            return;
        }
        if(Vector3.Distance(targetPlayer.position,zombieTransform.position)
            <= attackRange)
        {
            currentState = FSMState.Attack;
            agent.ResetPath();
            return;
        }

        agent.SetDestination(targetPlayer.position);
        setMaxAgentSpeed(runSpeed);
        //todo:进入狂暴状态

        if (zombieRender != null)
            zombieRender.SetCrazy();
        caculateStopTime();
        //？为什么这里处理不要if（stoptime<1)?
        //自己加的


    }

    //攻击状态处理函数
    void UpdateAttackState()
    {
        /*感知范围内没有玩家，进入游荡状态
         * 玩家与僵尸的距离大于攻击距离进入追踪状态
         * 只有玩家在僵尸前方才能攻击，（要算夹角,还要注意攻击间隔）如果不在前方要转向玩家再攻击
         * 攻击状态下的敌人应当连续追踪玩家（即边攻击边追踪）
         * 僵尸进入狂暴状态
         * 限制追踪速度
         */
        targetPlayer = zombieSensor.getNearbyPlayer();
        if (targetPlayer == null)
        {
            currentState = FSMState.Wander;
            agent.ResetPath();
            animator.SetBool("isAttack", false);
        }
        if(Vector3.Distance(targetPlayer.position,zombieTransform.position)>
            attackRange)
        {
            currentState = FSMState.Chase;
            agent.ResetPath();
            animator.SetBool("isAttack", false);
            return;
        }

        PlayerHealth ph = targetPlayer.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            Vector3 direction =
                targetPlayer.position - zombieTransform.position;
            float degree = Vector3.Angle(direction, zombieTransform.forward);
            if (degree < attackFieldOfView / 2 && degree > -attackFieldOfView / 2)
            {
                animator.SetBool("isAttack", true);
                if (attackTimer > attackInterval)
                {
                    attackTimer = 0;
                    if (zombieAttackAudio != null)
                    {
                        AudioSource.PlayClipAtPoint(
                            zombieAttackAudio, zombieTransform.position);
                        ph.TakeDamage(attackDamage);
                    }
                }
                attackTimer += Time.deltaTime;
            }
            else
            {
                animator.SetBool("isAttack", false);
                zombieTransform.LookAt(targetPlayer);
            }
        }

        agent.SetDestination(targetPlayer.position);
        //进入狂暴状态

        if (zombieRender != null)
            zombieRender.SetCrazy();
        setMaxAgentSpeed(runSpeed);
        
    }

    //死亡状态处理函数
    void UpdateDeadState()
    {
        /*死亡状态是要不断检测的在fixupdate中
         * 如果不是第一次死亡，统计僵尸死亡后经过的时间，如果超过了
         * 尸体消失时间，就禁用该僵尸对象
         * 如果是第一次死亡，需要禁用一些组件（agent，collider）
         * 
         */
        if (!disappeared)
        {
            if (disappearTimer > disappearTime)
            {
                zombieTransform.gameObject.SetActive(false);
                disappeared = true;
            }
            disappearTimer += Time.deltaTime;
        }


        if (firstInDead)
        {
            firstInDead = false;
            agent.ResetPath();
            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;

            animator.applyRootMotion = true;
            animator.SetTrigger("toDie");

            disappearTime = 0;
            disappeared = false;

            if (zombieRender != null)
            {
                zombieRender.SetNormal();
            }

            animator.ResetTrigger("toReborn");
        }

    }
}