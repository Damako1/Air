using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    #region 变量

    [SerializeField] StatsBar_HDR statsBar_HDR;

    [Header("是否再生生命值")]
    [SerializeField] bool regenrateHealth = true;

    [Header("生命值再生时间")]
    [SerializeField] float healthRegenerateTime;

    [Header("生命值再生百分比")]
    [SerializeField, Range(0f, 1f)] float healthRegeneratePercent;


    [SerializeField] PlayerInput input;
    [SerializeField] float moveSpeed = 10f;

    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;

    [SerializeField] float moveRotationAngle = 50f;

    [SerializeField] float paddingX = 0.2f;
    [SerializeField] float paddingY = 0.2f;


    /// <summary>
    /// 要生成的子弹1
    /// </summary>
    [SerializeField] GameObject projectile1;
    /// <summary>
    /// 要生成的子弹2
    /// </summary>
    [SerializeField] GameObject projectile2;
    
    [Header("要生成的子弹3")]
    [SerializeField] GameObject projectile3;

    [Header("能量爆发时所用的子弹")]
    [SerializeField] GameObject projectileOverdrive;


    // 中间子弹生成的位置
    [SerializeField] Transform muzzleMiddle;

    // 上面子弹生成的位置
    [SerializeField] Transform muzzleTop;

    // 下面子弹生成的位置
    [SerializeField] Transform muzzleBottom;


    // 武器的威力(控制子弹的数量)
    [Header("武器的威力(控制子弹的数量)")]
    [SerializeField, Range(0, 2)] int weaponPower = 0;


    // 子弹生成协程等待的时间
    [Header("子弹生成协程等待的时间")]
    [SerializeField] float fireInterval;

    WaitForSeconds waitForFireInterval;

    // 等待生命值再生时间
    WaitForSeconds waitHealthRegenerateTime;


    new Rigidbody2D rigidbody;

    Coroutine moveCoroutine;

    Coroutine healthRegenerateCoroutine;

    new Collider2D collider;


    [Header("闪避消耗能量值")]
    [SerializeField, Range(0, 100)] int dodgeEnergyCost = 25;

   
    [Header("最大滚转角")]
    [SerializeField] float maxRoll = 720f;

    [Header("翻滚速度")]
    [SerializeField] float rollSpeeed = 360;

    float currentRoll;

    [Header("最小缩放值")]
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    float dodgeDuration;

    bool isDodging = false;

    [Header("子弹音效")]
    [SerializeField] AudioData projectileLaunchSFX;

    [Header("闪避音效")]
    [SerializeField] AudioData dodgeSFX;

    [Header("移动端摇杆")]
    public Joystick joystick;

    [Header("移动端开火按钮(A键)")]
    public Button btn_Fire;

    [Header("移动端闪避按钮(B键)")]
    public Button btn_Dodge;

    // 是否处于能量爆发
    bool isOverdriving = false;

    [SerializeField] float overdriving_FireSpeed = 1.2f;
    [SerializeField] float overdriving_moveSpeed = 1.2f;
    [SerializeField] float overdriving_Dodgtime = 2f;
    WaitForSeconds waitForOverdriveFireInterval;

    #endregion

    #region 生命周期


    protected override void OnEnable()
    {
        base.OnEnable();
        //移动
        input.onMove += Move;
        input.onStapMove += StopMove;

        //开火
        input.onFire += Fire;
        input.onStopFire += StopFire;

        //闪避
        input.onDodge += Dodge;

        //能量爆发
        input.onOverdrive += Overdrive;
        PlayerOverdrive.on += Overdrive_On;
        PlayerOverdrive.off += Overdrive_Off;


    }


    private void OnDisable()
    {
        input.onMove -= Move;
        input.onStapMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;

        input.onOverdrive -= Overdrive;
        PlayerOverdrive.on -= Overdrive_On;
        PlayerOverdrive.off -= Overdrive_Off;

    }

    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        dodgeDuration = maxRoll / rollSpeeed;
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriving_FireSpeed);

    }


    void Start()
    {
        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        statsBar_HDR.Initialized(health, maxHealth);

        input.EnableGameplayInput();

        PlayerEnergy.Instance.Obtain(100);

        //TakeDamage(50f);
        Fire1();
    }
    #endregion

    #region 方法

    //public override void RestoreHealth(float value)
    //{
    //    base.RestoreHealth(value);
    //    Debug.Log("恢复生命值！当前生命值：" + health + "\n时间：" + Time.time);
    //}

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        statsBar_HDR.UpdateStats(health, maxHealth);

        if (gameObject.activeSelf)
        {
            if (regenrateHealth)
            {
                if (healthRegenerateCoroutine != null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }
                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        statsBar_HDR.UpdateStats(health, maxHealth);
    }

    public override void Die()
    {
        statsBar_HDR.UpdateStats(0f, maxHealth);
        base.Die();
    }
    #endregion

    #region 移动
    void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        Quaternion moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right);

        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed, moveRotation));
        StartCoroutine(nameof(MovePositionLimitCoroutine));

    }

    void StopMove()
    {
        //rigidbody.velocity = Vector2.zero;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero, Quaternion.identity));
        StopCoroutine(nameof(MovePositionLimitCoroutine));
    }

    /// <summary>
    /// 移动协程
    /// </summary>
    /// <param name="time"></param>
    /// <param name="moveVelocity"></param>
    /// <param name="moveRotation"></param>
    /// <returns></returns>
    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
    {
        float t = 0f;
        Vector2 previousVelocity = rigidbody.velocity;
        Quaternion previousRotation = transform.rotation;


        while (t < 1f)
        {
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t);
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t);
            yield return new WaitForFixedUpdate();

        }
    }


    IEnumerator MovePositionLimitCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);
            yield return null;
        }
    }
    #endregion

    #region 开火
    void Fire()
    {
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire()
    {
        StopCoroutine(nameof(FireCoroutine));
    }

    // 持续开火
    IEnumerator FireCoroutine()
    {
        while (true)
        {
            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position, Quaternion.identity);
                    break;
                case 1:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleTop.position, Quaternion.identity);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleBottom.position, Quaternion.identity);
                    break;
                case 2:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position, Quaternion.identity);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleTop.position, Quaternion.identity);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile3, muzzleBottom.position, Quaternion.identity);
                    break;
                default:
                    break;
            }
            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);

            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;

        }
    }

    #endregion

    #region 闪避
    private void Dodge()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;

        StartCoroutine(nameof(DodgeCoroutine));
        //闪避主要功能
        //1.消耗能量
        //2.让玩家无敌
        //3.让玩家沿着X轴旋转
        //4.改变玩家的缩放值
    }

    //让玩家无敌//实现方法有多种：1.改变玩家的Layer图层 2.更改玩家碰撞体的触发器开关
    /// <summary>
    /// 闪避协程
    /// </summary>
    /// <returns></returns>
    IEnumerator DodgeCoroutine()
    {

        isDodging = true;

        //播放闪避音效
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);

        //闪避消耗能量
        PlayerEnergy.Instance.Use(dodgeEnergyCost);

        //让玩家无敌//这里用方法二：更改玩家碰撞体的触发器开关
        collider.isTrigger = true;

        //让玩家沿着X轴旋转
        currentRoll = 0f;

        var t1 = 0f;
        var t2 = 0f;
        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

            if (currentRoll < maxRoll / 2f)
            {
                t1 += Time.deltaTime / dodgeDuration;
                transform.localScale = Vector3.Lerp(transform.lossyScale, dodgeScale, t1);
            }
            else
            {
                t2 += Time.deltaTime / dodgeDuration;
                transform.localScale = Vector3.Lerp(transform.lossyScale, Vector3.one, t2);
            }
            yield return null;
        }

        collider.isTrigger = false;
        isDodging = false;
    }
    #endregion

    #region 手机

    /// <summary>
    /// 手机上移动
    /// </summary>
    void Move2()
    {
#if UNITY_DEIDTO
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        if (h != 0 || v != 0)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            Vector2 moveInput = new Vector2(h, v);

            Quaternion moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right);


            moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed, moveRotation));
            StartCoroutine(nameof(MovePositionLimitCoroutine));
        }
        else
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero, Quaternion.identity));
            StopCoroutine(nameof(MovePositionLimitCoroutine));
        }
#endif
    }

    public void Fire1()
    {
        StartCoroutine(nameof(FireCoroutine));
    }

    public void StopFire1()
    {
        StopCoroutine(nameof(FireCoroutine));
    }

    public void Dodge1()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;

        StartCoroutine(nameof(DodgeCoroutine));
    }

    public void StopDodge1()
    {

    }

    private void Update()
    {

        Move2();
        

    }
    #endregion

    #region 能量爆发
    private void Overdrive()
    {
        if (PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX))
        {
            PlayerOverdrive.on.Invoke();
        }
    }

    private void Overdrive_On()
    {
        isOverdriving = true;
        moveSpeed *= overdriving_moveSpeed;
        dodgeDuration *= overdriving_Dodgtime;
        
    }

    private void Overdrive_Off()
    {
        isOverdriving = false;
        moveSpeed /= overdriving_moveSpeed;
        dodgeDuration /= overdriving_Dodgtime;

    }
    #endregion

}
