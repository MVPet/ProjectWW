using UnityEngine;
using System.Collections.Generic;

public abstract class AI_Base_Script : MonoBehaviour
{

    public CharacterInfo cInfo;
    public TextAsset charData;

    public GameObject hitSpark;

    protected const float DEATH_TIME = 1f;
    protected const float FLASH_TIME = 0.2f;
    protected const float MOVE_INTERVAL = 0.5f;
    protected const float MIN_X_RANGE = 0.3f;
    protected const float MAX_X_RANGE = 3.0f;

    protected float ATTACK_INTERVAL = 555.0f;

    protected Dictionary<string, DamageInfo> attackInfo = new Dictionary<string, DamageInfo>();
    protected Dictionary<string, ProjectileInfo> projectiles = new Dictionary<string, ProjectileInfo>();

    protected string comboString = "";

    protected int flashAlpha = 1;
    protected float health = 6;
    public bool isHurt = false;
    protected bool isDead = false;
    public bool inAir = true;
    public bool attacking = false;
    protected bool horizDampEnable = true;
    protected bool isMissionObjective = false;
    protected float hitStun = 0;
    protected float timer = 0;
    protected float flashingTimer = 0.0f;
    public float attackTimer = 0.0f;
    public float randomMoveTimer = 0.0f;
    public int moveLeft = 0;
    public int consecutiveMovement = 0;
    protected float facingPlayerX = 1.0f;
    protected float facingPlayerY = 1.0f;
    protected float styleTimer = 0.0f;
    protected float STYLE_INTERVAL = 7.0f;
    public bool debug = false;

    public bool timeToAttack = false;
    public bool closeAttacking = false;

    protected Animator animator;
    protected Vector2 startPosition = new Vector2(0, 0);
    public Vector2 velocity = new Vector2(0, 0);

    protected BaseScript inBase = null;
    protected MissionScript mission;

    public GameObject player;

    public GameObject target;

    //public float rayCenterOffset = 0.3f;
    //public Transform player;
    //public Vector2 feetOffset; // Used to determine y location of feet

    protected void BaseStart()
    {
        //footOffset = -0.95f;
        //attackInterval = Random.Range(6, 7.1f);
        //attackInterval = 1.0f; // Not a bad interval, but needs some work

        LoadInfo();

        GetComponent<Rigidbody2D>().gravityScale = 0f;
        //startPosition.x = transform.localPosition.x;
        animator = GetComponent<Animator>();
        //transform.position = new Vector3(transform.position.x, transform.position.y - cInfo.footOffset, transform.position.z);
        //startPosition.y = transform.localPosition.y - 6f; // 6f is the hard coded position above the room

        health = cInfo.maxHealth;
    }

    protected void RandomAIMovement()
    {
        randomMoveTimer += Time.deltaTime;

        if (randomMoveTimer > MOVE_INTERVAL)
        {
            int lastMove = moveLeft;
            moveLeft = (Random.Range(0, 11) > (5 + consecutiveMovement)) ? 1 : -1;

            if (lastMove != moveLeft)
                consecutiveMovement = 0;
            else
                consecutiveMovement += moveLeft;

            randomMoveTimer = 0;
        }

        velocity.x = cInfo.maxSpeed * moveLeft;
    }

    protected void BaseUpdate()
    {
        //feetOffset.x = transform.position.x;
        //Debug.DrawRay(feetOffset, Vector2.down, Color.red);

        CheckDeath();

        if (!inAir)
        {
            if (player)
            {
                float deltaX = Mathf.Abs(player.transform.position.x - transform.position.x);
                float deltaZ = Mathf.Abs(player.transform.position.z - transform.position.z);

                WhichWayShouldWeFace();

                IsItTimeToAttack();

                if (!isHurt && !isDead && !attacking && !MoveToPlayer(deltaX, deltaZ))
                {
                    if (timeToAttack)
                        AttackPlayer(deltaX, deltaZ);
                    else
                        AIMovement();
                        //RandomAIMovement();
                }

            }
            else
            {
                attackTimer = 0;
                randomMoveTimer = 0;
            }
        }
        else
            InAirUpdate();

        CheckStageCollision();

        GetComponent<Rigidbody2D>().velocity = this.velocity;
        animator.SetFloat("XVelocity", Mathf.Abs(velocity.x));
        animator.SetFloat("YVelocity", Mathf.Abs(velocity.y));
        animator.SetBool("inAir", inAir);

        UpdateScales();
        transform.position = new Vector3(transform.position.x, transform.localPosition.y, transform.localPosition.y - cInfo.footOffset);
    }

    protected void CheckDeath()
    {
        if (isDead)
        {
            timer += Time.deltaTime;
            flashingTimer += Time.deltaTime;

            if (flashingTimer >= FLASH_TIME)
                flashAlpha = System.Convert.ToInt16((0 == flashAlpha)); // Too cool to get rid of

            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 * flashAlpha);

            if (timer >= DEATH_TIME)
                Destroy(gameObject);
        }
    }

    protected void WhichWayShouldWeFace()
    {
        if (player.transform.position.x > transform.position.x)
            facingPlayerX = 1;
        else if (player.transform.position.x < transform.position.x)
            facingPlayerX = -1;

        if (player.transform.position.z > transform.position.z)
            facingPlayerY = 1;
        else if (player.transform.position.z < transform.position.z)
            facingPlayerY = -1;
    }

    protected virtual bool MoveToPlayer(float deltaX, float deltaZ)
    {
        if (deltaZ > 0.2f)
            velocity.y = (cInfo.maxSpeed * 0.5f) * facingPlayerY;
        else
            velocity.y = 0;

        if (timeToAttack) return false; // Still want the Y to be checked, just not the X

        if (deltaX > MAX_X_RANGE)
            velocity.x = cInfo.maxSpeed * facingPlayerX;
        else if (deltaX < MIN_X_RANGE)
            velocity.x = cInfo.maxSpeed * -facingPlayerX;
        else
            velocity.x = 0;

        // NEED TO SET WHEN A BOUND IS HIT SO THE RANDOM MOVEMENT DOESN'T BLOW EVERYTHING UP
        return (0 != velocity.x || 0 != velocity.y);
    }

    protected void InAirUpdate()
    {
        if (velocity.y > -10)
            velocity.y -= 25f * Time.deltaTime;

        if (velocity.y < 0 && transform.position.z > (startPosition.y - 0.5f) && transform.position.z < (startPosition.y + 0.1f)) // Changed from 0.1 to 0.5 might fix the fall through floor thing
        {
            inAir = false;
            transform.position = new Vector3(transform.position.x, startPosition.y + cInfo.footOffset, startPosition.y);
            velocity.y = 0;
            velocity.x = 0;
            animator.SetBool("inAir", inAir);
        }
    }

    protected void CheckStageCollision()
    {
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.z);
        float xDir = 0;
        float yDir = 0;
        int layerMask = 1 << 8; // Check for collisions only on the stage collision layer

        if (!inAir)
        {
            if (velocity.y != 0)
            {
                yDir = Mathf.Abs(velocity.y) / velocity.y;
                rayStart.y += 0.05f * yDir;
            }

            // Might need to use .RaycastAll instead and poll through all collisions in case base detection breaks due to .Raycast only returning first collision (i.e. the base)
            // Right now I only set the boundarys on the StageCollision layer.
            RaycastHit2D yHit = Physics2D.Raycast(rayStart, new Vector2(0, 1 * yDir), (velocity.y * Time.deltaTime), layerMask);
            //Debug.DrawRay(rayStart, new Vector2(0, (velocity.y * Time.deltaTime)), Color.red);

            if (yHit.collider != null && yHit.collider.tag == Tags.Y_BOUDNARY)
                velocity.y = 0;
        }

        if (velocity.x != 0)
        {
            xDir = Mathf.Abs(velocity.x) / velocity.x;
            rayStart.x += 0.2f * xDir;
        }

        RaycastHit2D xHit = Physics2D.Raycast(rayStart, new Vector2(1 * xDir, 0), (velocity.x * Time.deltaTime), layerMask);

        if (xHit.collider != null && xHit.collider.tag == Tags.X_BOUNDARY)
            velocity.x = 0;
    }

    protected void UpdateScales()
    {
        //Vector3 scaleToSet = new Vector3(transform.localScale.x * facingPlayerX, transform.localScale.y, transform.localScale.z);
        Vector3 scaleToSet = new Vector3(facingPlayerX, transform.localScale.y, transform.localScale.z);

        transform.localScale = scaleToSet;
    }

    public virtual void TakeDamage(DamageInfo info)
    {
        if (!isDead)
        {
            health -= info.damage;
            //hitStun = info.hitStun;
            velocity.x = info.knockback.x;
            velocity.y = info.knockback.y;

            isHurt = true;

            if (0 >= health)
            {
                isDead = true;
                animator.SetBool("isDead", isDead);
                //health = 0;

                //GameObject.Find(Tags.MAIN_CAMERA).GetComponent<CameraScript>().SetFocus(null);

                if (null != inBase)
                    inBase.TakeDamage(tag);

                if (null != mission)
                    mission.TargetDefeated();

                //if (!isDead)
                LevelScript.enemiesKOd++;

                if (0 == velocity.x)
                    velocity.x = 2f * -transform.localScale.x;
                if (0 == velocity.y)
                {
                    if (!inAir)
                        //startPosition.y = transform.position.y + cInfo.footOffset;
                        startPosition.y = transform.position.z;

                    velocity.y = 5f;  // Generic Death Knockback need to send it which direction it came in

                    inAir = true;
                    horizDampEnable = false;
                    animator.SetTrigger("AirHurt");
                }
            }

            if (velocity.y > 0)
            {
                if (!inAir)
                    startPosition.y = transform.position.z;

                inAir = true;
                horizDampEnable = false;
                animator.SetTrigger("AirHurt");
            }
            else
                animator.SetTrigger("Hurt");

            animator.SetBool("inAir", inAir);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (Tags.PLAYER == col.tag)
        {
            if ("" != comboString)
            {
                DamageInfo info = new DamageInfo(attackInfo[comboString]);

                if (col.transform.position.z <= (transform.position.z + info.range) && col.transform.position.z >= (transform.position.z - info.range))
                {
                    info.knockback.x *= transform.localScale.x;

                    col.gameObject.SendMessage("TakeDamage", info);

                    SpawnHitSpark(col);
                    styleTimer = 0;
                }
            }
        }
    }

    private void SpawnHitSpark(Collider2D col)
    {
        BoxCollider2D pHitBox = GetComponent<BoxCollider2D>();

        Vector3 hitSparkSpawn = pHitBox.bounds.center + col.bounds.center;

        hitSparkSpawn.x *= 0.5f;
        hitSparkSpawn.y *= 0.5f;
        hitSparkSpawn.z *= 0.5f;

        Instantiate(hitSpark, hitSparkSpawn, Quaternion.identity);
    }

    public void StopMovement()
    {
        velocity.x = 0;
        velocity.y = 0;
    }

    public void ResetAttacks()
    {
        attacking = false;
        horizDampEnable = true;
        comboString = "";
        isHurt = false;
    }

    private void LoadInfo()
    {
        int index = 0;
        string[] dataLines = charData.text.Split("\n"[0]);

        StoreCharacterInfo(dataLines[index++]);

        string line;
        while (!(line = dataLines[index++]).Contains("--"))
            StoreAttackInfo(line);

        while (index < (dataLines.Length - 1))
        {
            index = StoreProjectileStrings(dataLines, index);
        }

        //startPosition.y -= cInfo.footOffset;
    }

    private void StoreCharacterInfo(string jsonData)
    {
        this.cInfo = JsonUtility.FromJson<CharacterInfo>(jsonData);
    }

    private void StoreAttackInfo(string jsonData)
    {
        DamageInfo info = new DamageInfo();

        info = JsonUtility.FromJson<DamageInfo>(jsonData);

        attackInfo.Add(info.combo, info);
    }

    private int StoreProjectileStrings(string[] dataLines, int index)
    {
        string line;
        DamageInfo info;
        ProjectileInfo projectile = new ProjectileInfo();
        projectile = JsonUtility.FromJson<ProjectileInfo>(dataLines[index++]);

        while (!(line = dataLines[index++]).Contains("--"))
        {
            info = new DamageInfo();
            info = JsonUtility.FromJson<DamageInfo>(line);

            projectile.attackInfo.Add(info.combo, info);
        }

        projectiles.Add(projectile.name, projectile);
        return index;
    }

    public void SetInAir(bool inAir, float yEnd)
    {
        this.inAir = inAir;
        startPosition.y = yEnd;
    }

    public void SetInBase(BaseScript baseScript)
    {
        inBase = baseScript;
    }

    public void SetNotInBase()
    {
        inBase = null;
    }

    public void SetAsMission(MissionScript mission)
    {
        this.mission = mission;
    }

    public void Attack()
    {
        attacking = true;
        StopMovement();
    }

    protected abstract void IsItTimeToAttack();

    protected abstract void AttackPlayer(float deltaX, float deltaY);

    protected abstract void AIMovement();

    public abstract void FoundPlayer(bool found);
}
