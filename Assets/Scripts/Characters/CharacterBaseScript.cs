using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Temporary

public class CharacterBaseScript : MonoBehaviour
{
    public CharacterInfo cInfo;
    public TextAsset charData;

    public Texture2D healthBarFrame;
    public Texture2D healthBarForeground;
    public Texture2D healthBarBackground;
    public Texture2D specialBarFrame;
    public Texture2D specialBarForeground;
    public Texture2D specialBarBackground;
    public Texture2D portrait;
    public Texture2D transportDown;
    public Texture2D transportUp;
    public AudioClip themeMusic;
    public GameObject hitSpark;

    protected Dictionary<string, DamageInfo> attackInfo = new Dictionary<string, DamageInfo>();
    protected Dictionary<string, ProjectileInfo> projectiles = new Dictionary<string, ProjectileInfo>();
    protected List<string> comboList = new List<string>();

    protected string comboString = "";
    protected float dodge_timer = 9f;
    protected float run_timer = 0f;
    protected float xMove;
    protected float yMove;
    protected float lastGroundYPos;
    protected bool move_held = false;
    protected bool normalHeld = false;
    protected bool specialHeld = false;
    protected bool superHeld = false;
    protected bool exAttack = false;
    protected bool submitHeld = false;
    protected bool dodgeHeld = false;
    protected bool isDead = false;
    protected bool isHurt = false;
    public bool inAir = false;
    public bool isPaused = false;
    protected bool is_guarding = false;
    protected bool is_attacking = false;
    public bool is_dodging = false;
    protected bool horizDampEnable = true;

    protected bool isRunning = false;
    protected float runDirection = 0;
    protected bool canRun = false;
    //protected float runTimer = 0;

    protected bool canTransport = false;
    protected bool isTransportDown = false; // Used only to display Texture above player's head
    protected Vector2 endPoint;

    public Vector2 velocity;
    protected Vector2 facing;
    public Vector2 startPosition = new Vector2(0, 0);

    protected Animator animator;
    protected GameObject spawnedItem = null;

    bool inputDisabled = false;
    float curHealth;
    int maxSpecial = 100;
    public int curSpecial;

    protected float exFlashTimer = 0;

    private Vector2 spawn;


    //public Vector2 feetOffset;      // Used to determine y location of feet

    protected void BaseGUI()
    {
        float percentHealth = healthBarForeground.width * ((float)curHealth / (float)cInfo.maxHealth);
        float percentSpecial = (specialBarForeground.width) * ((float)curSpecial / (float)maxSpecial);

        GUI.DrawTexture(new Rect(10, 14, portrait.width, portrait.height), portrait, ScaleMode.ScaleToFit);

        GUI.DrawTexture(new Rect(10 + portrait.width, 38, healthBarBackground.width, healthBarBackground.height), healthBarBackground, ScaleMode.ScaleToFit);
        GUI.DrawTexture(new Rect(10 + portrait.width, 38, percentHealth, healthBarForeground.height), healthBarForeground, ScaleMode.ScaleAndCrop);
        GUI.DrawTexture(new Rect(10 + portrait.width, 38, healthBarFrame.width, healthBarFrame.height), healthBarFrame, ScaleMode.ScaleToFit);

        //GUI.DrawTexture(new Rect(10, 10, healthBarBackground.width, healthBarBackground.height), healthBarBackground, ScaleMode.ScaleToFit);
        //GUI.DrawTexture(new Rect(10, 10, percentHealth, healthBarForeground.height), healthBarForeground, ScaleMode.ScaleAndCrop);


        GUI.DrawTexture(new Rect(10 + portrait.width, 20, specialBarBackground.width, specialBarBackground.height), specialBarBackground, ScaleMode.ScaleToFit);
        GUI.DrawTexture(new Rect(10 + portrait.width, 20, percentSpecial, specialBarForeground.height), specialBarForeground, ScaleMode.ScaleAndCrop);
        GUI.DrawTexture(new Rect(10 + portrait.width, 20, specialBarFrame.width, specialBarFrame.height), specialBarFrame, ScaleMode.ScaleToFit);

        //GUI.DrawTexture(new Rect(58, 34, specialBarBackground.width - 48, specialBarBackground.height), specialBarBackground, ScaleMode.ScaleToFit);
        //GUI.DrawTexture(new Rect(58, 34, percentSpecial, specialBarForeground.height), specialBarForeground, ScaleMode.ScaleAndCrop);
    }

    // Use this for initialization
    protected void BaseStart(string dataFileName)
    {
        transform.GetChild(0).tag = tag;
        animator = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        LoadInfo(dataFileName);

        curHealth = cInfo.maxHealth;
        curSpecial = 0;

        transform.position = new Vector3(spawn.x, spawn.y + cInfo.footOffset, spawn.y);
        //transform.position = new Vector3(transform.position.x, transform.position.y - cInfo.footOffset, transform.position.z);
        //feetOffset.y = transform.position.y;
    }


    private void LoadInfo(string fileName)
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

    private void StoreCharacterInfo(string jsonData)
    {
        this.cInfo = JsonUtility.FromJson<CharacterInfo>(jsonData);
    }

    private void StoreAttackInfo(string jsonData)
    {
        DamageInfo info = new DamageInfo();

        info = JsonUtility.FromJson<DamageInfo>(jsonData);

        attackInfo.Add(info.combo, info);
        comboList.Add(info.combo);
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

    protected void CheckEXFlash()
    {
        if (exAttack && 1 < comboString.Length && 'H' == comboString[comboString.Length - 1])
        {
            exFlashTimer += Time.deltaTime;

            if (0.1f <= exFlashTimer)
            {
                exFlashTimer = 0;
                if (Color.yellow == GetComponent<SpriteRenderer>().color)
                    GetComponent<SpriteRenderer>().color = Color.white;
                else
                    GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
    }

    // Update is called once per frame
    protected void BaseUpdate()
    {
        CheckEXFlash();
        animator.SetBool("inAir", inAir);

        if (!isPaused)
        {
            //Sets the layers of drawing, the lower the Z the closer to the front
            if (!inAir)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y - cInfo.footOffset); //1.5 for offset
                //feetOffset.x = transform.position.x;
                //Debug.DrawRay(feetOffset, Vector2.down);
            }

            if (!inputDisabled)
                ActionInput();

            MovementInput();
        }
    }

    private void CheckMovementInput()
    {
        if (!is_dodging)
        {
            xMove = Input.GetAxisRaw("Horizontal");
            yMove = Input.GetAxisRaw("Vertical");
        }

        if (inputDisabled)
        {
            xMove = 0;
            yMove = 0;
        }

        if (!inAir && !is_guarding && !is_attacking && !is_dodging && !isHurt)
        {
            facing.y = yMove;

            velocity.x = xMove * cInfo.maxSpeed;
            velocity.y = yMove * (cInfo.maxSpeed * 0.75f);

            if (0 != xMove && (xMove / Mathf.Abs(xMove)) == -transform.localScale.x)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            facing.x = transform.localScale.x;

            if (1 == Mathf.Abs(xMove))
            {
                if (!isRunning && canRun)
                {
                    runDirection = facing.x;
                    canRun = false;
                    isRunning = true;
                }

                if (facing.x != runDirection)
                {
                    canRun = false;
                    isRunning = false;
                }
            }
            else
            {
                canRun = false;
                isRunning = false;
            }

            if (isRunning)
                velocity.x *= 2.5f;
        }

        if (!inAir && horizDampEnable)
        {
            if (0.5f < Mathf.Abs(velocity.x))
                velocity.x -= 5 * transform.localScale.x * Time.deltaTime;
            else
                velocity.x = 0;
        }
    }

    private void CheckDodgeInput()
    {
        float dodge = Input.GetAxisRaw("Dodge");

        if (inputDisabled)
            dodge = 0;

        if (0 >= dodge)
            dodgeHeld = false;

        // Will play dodge animation during Heavy Attack, might want to change the if from Start Dodge to here
        if (!isHurt && ("Sp" != comboString || Tags.Characters.RYU == name) && !comboString.Contains("Su"))
        {
            if (!is_dodging && !dodgeHeld && 0 < dodge && (0 != xMove || 0 != yMove))
            {
                animator.ResetTrigger("Normal");
                animator.ResetTrigger("Special");
                animator.ResetTrigger("Super");
                animator.ResetTrigger("AttackPressed");
                animator.SetTrigger("Dodge");
                is_dodging = true;
                dodgeHeld = true;
                facing.x = xMove;
                facing.y = yMove;
            }
        }
    }

    protected void CheckInAir()
    {
        if (inAir)
        {
            if (velocity.y > -10)
                velocity.y -= 25f * Time.deltaTime;

            if (velocity.y < 0 && transform.position.y > (startPosition.y - 0.5f) && transform.position.y < (startPosition.y + 0.1f)) // Changed from 0.1 to 0.5 might fix the fall through floor thing
            {
                inAir = false;
                transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
                velocity.y = 0;
                //velocity.x = 0;
                //animator.SetBool("inAir", inAir);
            }
        }
    }

    void CheckTransportInput()
    {
        float submit = Input.GetAxisRaw("Plane Shift");

        if (0 >= submit)
            submitHeld = false;

        if (canTransport && !submitHeld && 0 < submit)
        {
            submitHeld = true;

            float directionOffset = (endPoint.y < transform.position.z) ? 2 : 2.4f; // Directional offsets to match the direction transporting (Transport up = bottom of path, down = top)

            transform.position = new Vector3(endPoint.x, endPoint.y + cInfo.footOffset - directionOffset, endPoint.y);
            GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).SendMessage("SetStageLayer", endPoint.y / 10f);
        }
    }

    protected virtual void MovementInput()
    {
        CheckTransportInput();
        CheckMovementInput();
        CheckDodgeInput();

        CheckMovementCollision();

        CheckInAir();

        animator.SetBool("isRunning", isRunning);
        animator.SetFloat("XVelocity", Mathf.Abs(velocity.x));
        animator.SetFloat("YVelocity", Mathf.Abs(velocity.y));

        GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y);
    }

    void CheckMovementCollision()
    {
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.z);
        float yDir = 0;
        float xDir = 0;
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

    public void DestoryChildren()
    {
        if (null != spawnedItem)
            Destroy(spawnedItem);
    }

    public virtual void StartDOOODGE()
    {
        if (0 != xMove)
        {
            is_dodging = true;
            DestoryChildren();
            velocity.x = xMove * cInfo.dodgeSpeed;

            if (0 != xMove && (xMove / Mathf.Abs(xMove)) == -transform.localScale.x)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        if (0 != yMove)
        {
            is_dodging = true;
            DestoryChildren();
            velocity.y = yMove * (cInfo.dodgeSpeed / 3.0f);
        }
    }

    public virtual void StopDOOODGE()
    {
        is_dodging = false;

        if (dodgeHeld)
            canRun = true;
    }


    protected virtual void ActionInput()
    {
        //if (!inAir)
        //{
        float normal = Input.GetAxisRaw("Normal");
        float special = Input.GetAxisRaw("Special");
        float super = Input.GetAxisRaw("Super");

        if (0 >= normal)
            normalHeld = false;

        if (!normalHeld && 0 < normal)
        {
            animator.SetTrigger("Normal");
            animator.ResetTrigger("Special");
            animator.ResetTrigger("Super");
            animator.SetTrigger("AttackPressed");
            normalHeld = true;
        }

        if (0 >= special)
            specialHeld = false;

        if (!specialHeld && 0 < special)
        {
            /*if (33 <= curSpecial && 0 < normal)
            {
                animator.SetTrigger("EXAttack");
                exAttack = true;
                curSpecial -= 33;
            }*/

            animator.ResetTrigger("Normal");
            animator.SetTrigger("Special");
            animator.ResetTrigger("Super");
            animator.SetTrigger("AttackPressed");
            specialHeld = true;
        }


        if (0 >= super)
            superHeld = false;

        if (100 <= curSpecial && !superHeld && 0 < super)
        {
            comboString = "";
            superHeld = true;
            animator.ResetTrigger("Normal");
            animator.ResetTrigger("Special");
            animator.SetTrigger("Super");
            animator.SetTrigger("AttackPressed");
        }
        //}
    }

    public void Normal()
    {
        animator.ResetTrigger("OnHit");
        animator.ResetTrigger("AttackPressed");
        xMove = Input.GetAxisRaw("Horizontal");

        if (0 != xMove && (xMove / Mathf.Abs(xMove)) == -transform.localScale.x)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        is_dodging = false;
        if (!inAir) StopMovement();
        StartAttack();
        comboString += "N";
    }

    public void Special()
    {
        animator.ResetTrigger("OnHit");
        animator.ResetTrigger("AttackPressed");
        xMove = Input.GetAxisRaw("Horizontal");

        if (!inAir && 0 != xMove && (xMove / Mathf.Abs(xMove)) == -transform.localScale.x)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        is_dodging = false;
        if (!inAir) StopMovement();
        StartAttack();
        comboString += "Sp";
    }

    public void Super()
    {
        /*animator.ResetTrigger("OnHit");
        xMove = Input.GetAxisRaw("Horizontal");

        if (0 != xMove && (xMove / Mathf.Abs(xMove)) == -transform.localScale.x)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);*/

        curSpecial = 0;
        is_dodging = false;
        StopMovement();
        StartAttack();
        comboString += "Su";
    }

    public void ResetAttacks()
    {
        animator.ResetTrigger("Normal");
        animator.ResetTrigger("Special");
        animator.ResetTrigger("Super");
        animator.ResetTrigger("OnHit");
        animator.ResetTrigger("AttackPressed");
        comboString = "";
        is_attacking = false;
        horizDampEnable = true;
        isHurt = false;
        exAttack = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void StopMovement()
    {
        if (!is_dodging)
        {
            isRunning = false;
            velocity.x = 0;
            velocity.y = 0;
        }
    }

    public void StartAttack()
    {
        is_attacking = true;
    }

    protected virtual void BuildMeter()
    {
        curSpecial++;

        if (curSpecial >= maxSpecial)
            curSpecial = maxSpecial;
    }

    void SpawnHitSpark(Collider2D col)
    {
        BoxCollider2D pHitBox = GetComponent<BoxCollider2D>();

        Vector3 hitSparkSpawn = pHitBox.bounds.center + col.bounds.center;

        hitSparkSpawn.x *= 0.5f;
        hitSparkSpawn.y *= 0.5f;
        hitSparkSpawn.z *= 0.5f;

        Instantiate(hitSpark, hitSparkSpawn, Quaternion.identity);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (Tags.ENEMY == col.tag && "" != comboString)
        {
            DamageInfo info = new DamageInfo(attackInfo[comboString]);

            if (col.transform.position.z <= (transform.position.z + info.range) && col.transform.position.z >= (transform.position.z - info.range))
            {
                animator.SetTrigger("OnHit");
                info.knockback.x *= transform.localScale.x;

                col.gameObject.SendMessage("TakeDamage", info);

                BuildMeter();

                SpawnHitSpark(col);
            }
        }
    }

    void DisableInput(bool value)
    {
        inputDisabled = value;
    }

    protected virtual void TakeDamage(DamageInfo info)
    {
        curHealth -= info.damage;
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

        //animator.SetBool("inAir", inAir);

        if (0 >= curHealth)
        {
            isDead = true;
            animator.SetBool("isDead", isDead);

            if (0 == velocity.x)
                velocity.x = 2f;
            if (0 == velocity.y)
            {
                if (!inAir)
                    startPosition.y = transform.position.y;

                velocity.y = 5f;  // Generic Death Knockback need to send it which direction it came in

                inAir = true;
            }

            SceneManager.LoadScene(0); // Temporary
        }
    }

    public void CanTransport(Vector2 endPoint)
    {
        if (555 == endPoint.x && 555 == endPoint.y)
            canTransport = false;
        else
            canTransport = true;

        this.endPoint = endPoint;
    }

    public string GetName()
    {
        return name;
    }

    public List<string> GetComboList()
    {
        return comboList;
    }

    public void SetSpawnOnGround(Vector2 spawn)
    {
        this.spawn = spawn;
    }

    //DO NOT DELETE
    protected virtual void SetUpAttacks()
    { }
}