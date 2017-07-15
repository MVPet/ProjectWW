using UnityEngine;

public class MookScript : AI_Base_Script
{
    void Start()
    {
        /*
        footOffset = 0.30f;
        BaseStart();
        startPosition.y = transform.localPosition.y - 6f; // 6f is the hard coded position above the room
        SetUpAttacks();
        */

        base.BaseStart();

        ATTACK_INTERVAL = Random.Range(7, 12.1f);
    }

    protected override void IsItTimeToAttack()
    {
        if (0 == velocity.x && 0 == velocity.y)
            attackTimer += Time.deltaTime;
        else
            attackTimer = 0;

        if (attackTimer > ATTACK_INTERVAL)
        {
            //attacking = true;
            timeToAttack = true;
            attackTimer = 0;
        }
    }

    
    void Update()
    {
        /*
        BaseUpdate();

        if (null != player && (0 == velocity.x && 0 == velocity.y))
            attackTimer += Time.deltaTime;
        else
            attackTimer = 0;

        ShouldWeAttack();
        */

        base.BaseUpdate();
    }

    //public void FoundPlayer(GameObject player)
    public override void FoundPlayer(bool found)
    {
        if (found)
            player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
        else
            player = null;

        if (!isHurt && !attacking && !isDead && null == player)
            velocity = new Vector2(0, 0);
    }

    protected override bool MoveToPlayer(float deltaX, float deltaZ)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.z - transform.position.z, 2));
        //float deltaX = Mathf.Abs(player.transform.position.x - transform.position.x);

        /*
        if (player.transform.position.x > transform.position.x)
            facingPlayerX = 1;
        else if (player.transform.position.x < transform.position.x)
            facingPlayerX = -1;

        if (player.transform.position.z > transform.position.z)
            facingPlayerY = 1;
        else if (player.transform.position.z < transform.position.z)
            facingPlayerY = -1;
            */

        //transform.localScale = new Vector3(facingPlayerX, transform.localScale.y, transform.localScale.z);

        if (0.8f < distance && 1.1f < deltaX)
        {
            velocity.x = cInfo.maxSpeed * facingPlayerX;
            velocity.y = cInfo.maxSpeed * facingPlayerY;
            //attackTimer = 0;
        }
        else if (0.6f > distance && 0.6 > deltaX)
        {
            if (player.transform.position.x < transform.position.x)
                velocity.x = cInfo.maxSpeed;
            else if (player.transform.position.x > transform.position.x)
                velocity.x = cInfo.maxSpeed * -facingPlayerX;

            if (player.transform.position.z < transform.position.z)
                velocity.y = cInfo.maxSpeed * 0.5f;
            else if (player.transform.position.z > transform.position.z)
                velocity.y = cInfo.maxSpeed * 0.5f * -facingPlayerY;

            //attackTimer = 0;
        }
        else
        {
            //attackTimer += Time.deltaTime;
            velocity.x = 0;
            velocity.y = 0;
        }

        /*if (0.5f < distance)
        {
            velocity.x = maxSpeed.x * facingPlayerX;
            velocity.y = maxSpeed.y * facingPlayerY;
        }
        else
            velocity = new Vector2(0, 0);*/

        return (0 != velocity.x || 0 != velocity.y);
    }

    protected override void AttackPlayer(float deltaX, float deltaY)
    {
        animator.SetTrigger("Attack");
    }

    protected override void AIMovement()
    { }

    //public void Normal1()
    public void FarAttack()
    {
        //velocity.x = 3f * transform.localScale.x;
        //velocity.y = 1.5f * facingPlayerY;

        comboString = "F";
        velocity.x = (Mathf.Abs(player.transform.position.x - transform.position.x) * 2.0f) * transform.localScale.x;
        velocity.y = (Mathf.Abs(player.transform.position.z - transform.position.z) * 2.0f) * facingPlayerY;
    }

    /*
 protected void BaseStart()
 {
     GetComponent<Rigidbody2D>().gravityScale = 0f;
     startPosition.x = transform.localPosition.x;
     startPosition.y = transform.localPosition.y;
     animator = GetComponent<Animator>();
     transform.position = new Vector3(transform.position.x, transform.position.y - footOffset, transform.position.z);

     maxHealth = health;
 }
 */

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

    /*
    void InAirUpdate()
    {
        if (velocity.y > -10)
            velocity.y -= 25f * Time.deltaTime;

        if ((velocity.y < 0) && transform.position.y > (startPosition.y - 0.3) && transform.position.y < (startPosition.y + 0.3))
        {
            inAir = false;
            transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
            velocity.y = 0;
            velocity.x = 0;
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
public void ResetAttacks()
{
    attacking = false;
    horizDampEnable = true;
    comboString = "";
    isHurt = false;
}
*/
    /*
        protected void BaseUpdate()
        {
            if (inAir)
            {
                InAirUpdate();
            }
            else
            {
                //feetOffset.x = transform.position.x;
                //Debug.DrawRay(feetOffset, Vector2.up);
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.localPosition.y + footOffset);

                CheckDeath();

                if (!isHurt && !attacking && !isDead && null != player)
                {
                    MoveToPlayer();
                    //ShouldWeAttack();
                }

            }

            CheckXCollision();

            GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y);
            animator.SetFloat("XVelocity", Mathf.Abs(velocity.x));
            animator.SetFloat("YVelocity", Mathf.Abs(velocity.y));
            animator.SetBool("inAir", inAir);
        }
        */

    /*
    private void CheckXCollision()
    {
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y + footOffset);
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
    */

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

        if (null != inBase && !isDead)
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
                startPosition.y = transform.position.y;

            velocity.y = 5f;  // Generic Death Knockback need to send it which direction it came in

            inAir = true;
            horizDampEnable = false;
            animator.SetTrigger("AirHurt");
        }
    }

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

    /*
    public void StopMovement()
    {
        velocity.x = 0;
        velocity.y = 0;
    }
    */

    /*
public void Normal()
{
    StopMovement();
    comboString += "L";
}

public void Special()
{
    StopMovement();
}
*/
}
