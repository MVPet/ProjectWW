using UnityEngine;

public class Big_Mook_Script : AI_Base_Script
{
    public GameObject knife;

    bool isLeader = false;
    BaseScript baseScript;

    // Use this for initialization
    void Start()
    {
        /*
        //footOffset = -0.95f;
        //attackInterval = Random.Range(6, 7.1f);
        //attackInterval = 1.0f; // Not a bad interval, but needs some work

        LoadInfo();

        GetComponent<Rigidbody2D>().gravityScale = 0f;
        //startPosition.x = transform.localPosition.x;
        startPosition.y += cInfo.footOffset;
        animator = GetComponent<Animator>();
        transform.position = new Vector3(transform.position.x, transform.position.y - cInfo.footOffset, transform.position.z);
        //startPosition.y = transform.localPosition.y - 6f; // 6f is the hard coded position above the room

        health = cInfo.maxHealth;
        */

        base.BaseStart();

        ATTACK_INTERVAL = 5.0f;
    }

    /*
    void CheckDeath()
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
    */

    
    protected override void IsItTimeToAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer > ATTACK_INTERVAL)
        {
            closeAttacking = (Random.Range(0, 11) < 7.5f);
            //closeAttacking = true;
            timeToAttack = true;
            attackTimer = 0;
        }
    }

    /*
    void WhichWayShouldWeFace()
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
    */

    /*
    bool MoveToPlayer(float deltaX, float deltaZ)
    {
        if (!attacking)
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
        }

        // NEED TO SET WHEN A BOUND IS HIT SO THE RANDOM MOVEMENT DOESN'T STUTTER
        return (0 != velocity.x || 0 != velocity.y);
    }
    */

    protected override void AttackPlayer(float deltaX, float deltaZ)
    {
        if (closeAttacking)
        {
            if (deltaX > 0.6f)
                velocity.x = cInfo.maxSpeed * facingPlayerX;
            else if (deltaZ < 0.3f)
            {
                attacking = true;
                timeToAttack = false;
                animator.SetTrigger("CloseAttack");
                comboString = "C";
            }
        }
        else
        {
            if (deltaX < 2.0f)
                velocity.x = cInfo.maxSpeed * -facingPlayerX;
            else if (deltaZ < 0.3f)
            {
                attacking = true;
                timeToAttack = false;
                animator.SetTrigger("FarAttack");
            }
        }
    }

    //void RandomAIMovement()
    protected override void AIMovement()
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

    /*
    void InAirUpdate()
    {
        if (velocity.y > -10)
            velocity.y -= 25f * Time.deltaTime;

        if (velocity.y < 0 && transform.position.z > (startPosition.y - 0.5f) && transform.position.z < (startPosition.y + 0.1f)) // Changed from 0.1 to 0.5 might fix the fall through floor thing
        {
            inAir = false;
            transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
            velocity.y = 0;
            velocity.x = 0;
            //animator.SetBool("inAir", inAir);
        }
    }
    */

    /*
    private void CheckStageCollision()
    {
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y + cInfo.footOffset);
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

    void UpdateScales()
    {
        //Vector3 scaleToSet = new Vector3(transform.localScale.x * facingPlayerX, transform.localScale.y, transform.localScale.z);
        Vector3 scaleToSet = new Vector3(facingPlayerX, transform.localScale.y, transform.localScale.z);

        transform.localScale = scaleToSet;
        transform.FindChild("LifeBar").localScale = scaleToSet;
        transform.FindChild("LeaderText").localScale = scaleToSet;
    }
    */

    void Update()
    {

        /*
        //feetOffset.x = transform.position.x;
        //Debug.DrawRay(feetOffset, Vector2.up);

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
                        RandomAIMovement();
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
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.localPosition.y + cInfo.footOffset);
        */

        base.BaseUpdate();

        transform.FindChild("LifeBar").localScale = transform.localScale;
        transform.FindChild("LeaderText").localScale = transform.localScale;
    }

    void FarAttack()
    {
        Vector3 projectileOffset = new Vector3(transform.position.x + 1.0f * transform.localScale.x, transform.position.y + 0.30f, transform.position.z);
        GameObject instance = Instantiate(knife, projectileOffset, Quaternion.identity) as GameObject;
        instance.GetComponent<ProjectileBaseScript>().SetProjectileInfo(projectiles["Knife"], this.tag, transform.localScale.x);
    }

    /*
    public void TakeDamage(DamageInfo info)
    {
        health -= info.damage;
        //hitStun = info.hitStun;
        velocity.x = info.knockback.x;
        velocity.y = info.knockback.y;

        isHurt = true;

        if (velocity.y > 0)
        {
            if (!inAir)
                startPosition.y = transform.position.y;

            inAir = true;
            horizDampEnable = false;
            animator.SetTrigger("AirHurt");
        }
        else
            animator.SetTrigger("Hurt");

        animator.SetBool("inAir", inAir);

        if (0 >= health)
        {
            health = 0;

            if (isLeader)
                baseScript.LeaderDied();
            else if (null != inBase && !isDead)
                inBase.TakeDamage(tag);

                if (null != mission && !isDead)
                    mission.TargetDefeated();

            if (!isDead)
                LevelScript.enemiesKOd++;

            isDead = true;
            animator.SetBool("isDead", isDead);

            if (0 == velocity.x)
                velocity.x = 2f * -transform.localScale.x;
            if (0 == velocity.y)
            {
                if (!inAir)
                    startPosition.y = transform.position.y + cInfo.footOffset;

                velocity.y = 5f;  // Generic Death Knockback need to send it which direction it came in

                inAir = true;
                horizDampEnable = false;
                animator.SetTrigger("AirHurt");
            }
        }
    }
    */

    /*
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
                }
            }
        }
    }
    */

    /*
    void SpawnHitSpark(Collider2D col)
    {
        BoxCollider2D pHitBox = GetComponent<BoxCollider2D>();

        Vector3 hitSparkSpawn = pHitBox.bounds.center + col.bounds.center;

        hitSparkSpawn.x *= 0.5f;
        hitSparkSpawn.y *= 0.5f;
        hitSparkSpawn.z *= 0.5f;

        Instantiate(hitSpark, hitSparkSpawn, Quaternion.identity);
    }
    */

    /*
    public void SetInBase(BaseScript baseScript)
    {
        inBase = baseScript;
    }
    */

    /*
    public void SetNotInBase()
    {
        inBase = null;
    }
    */

    /*
    public void SetAsMission(MissionScript mission)
    {
        this.mission = mission;
    }
    */

    public override void FoundPlayer(bool found)
    {
        if (found)
            player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
        else
            player = null;

        if (!isHurt && !attacking && !isDead && null == player)
            velocity = new Vector2(0, 0);
    }

    public override void TakeDamage(DamageInfo info)
    {
        if (!isDead)
        {
            base.TakeDamage(info);

            if (isLeader && isDead)
                baseScript.LeaderDied();
        }
    }

    /*
    public void StopMovement()
    {
        velocity.x = 0;
        velocity.y = 0;
    }
    */

    /*
    public void ResetAttacks()
    {
        attacking = false;
        horizDampEnable = true;
        comboString = "";
        isHurt = false;
    }
    */

    /*
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

    }
    */

    /*
    private void StoreCharacterInfo(string jsonData)
    {
        this.cInfo = JsonUtility.FromJson<CharacterInfo>(jsonData);
    }
    */

    /*
    private void StoreAttackInfo(string jsonData)
    {
        DamageInfo info = new DamageInfo();

        info = JsonUtility.FromJson<DamageInfo>(jsonData);

        attackInfo.Add(info.combo, info);
    }
    */

    /*
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
    */

    /*
    public void SetInAir(bool inAir, float yDistance)
    {
        this.inAir = inAir;
        startPosition.y = transform.position.y + yDistance;
    }
    */

    public void SetAsLeader(BaseScript baseScript)
    {
        Debug.Log("Called");
        isLeader = true;
        this.baseScript = baseScript;

        Color leaderColor = Color.red;

        Transform leaderText = transform.Find("LeaderText");
        leaderText.gameObject.SetActive(true);
        leaderText.GetComponent<SpriteRenderer>().color = leaderColor;
    }

    /*
    public void Attack()
    {
        attacking = true;
        StopMovement();
    }
    */
}
