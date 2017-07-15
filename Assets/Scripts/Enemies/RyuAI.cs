using UnityEngine;
using System.Collections.Generic;

public class RyuAI : AI_Base_Script
{
    public GameObject hadouken;
    public GameObject shakunetsu;

    public Texture2D portrait;
    public Texture2D healthBarFrame;
    public Texture2D healthBarForeground;
    public Texture2D healthBarBackground;

    Vector2 hadoukenOffset = new Vector2(1.3f, 1.05f);
    private Vector2 spawn;

    public enum AI_STYLE { Turtle, Assault };

    public AI_STYLE aiStyle;

    void OnGUI()
    {
        if (null != player)
        {
            float percentHealthLeft = healthBarForeground.width * (float)(cInfo.maxHealth - health) / (float)cInfo.maxHealth;

            GUI.DrawTexture(new Rect(Screen.width - portrait.width - 10, 14, portrait.width, portrait.height), portrait, ScaleMode.ScaleToFit);

            GUI.DrawTexture(new Rect(Screen.width - portrait.width - 10 - healthBarBackground.width, 38, healthBarBackground.width, healthBarBackground.height), healthBarBackground, ScaleMode.ScaleToFit);
            GUI.DrawTexture(new Rect(Screen.width - portrait.width - 10 - (healthBarForeground.width - percentHealthLeft), 38, healthBarForeground.width - percentHealthLeft, healthBarForeground.height), healthBarForeground, ScaleMode.ScaleAndCrop);
            GUI.DrawTexture(new Rect(Screen.width - portrait.width - 10 - healthBarFrame.width, 38, healthBarFrame.width, healthBarFrame.height), healthBarFrame, ScaleMode.ScaleToFit);
        }
    }

    void Start()
    {
        base.BaseStart();

        cInfo.maxHealth = 10; // Temporary until a RYU AI Data is made
        ATTACK_INTERVAL = 5.0f;
        transform.position = new Vector3(spawn.x, spawn.y + cInfo.footOffset, spawn.y);

        STYLE_INTERVAL = 30.0f;
        styleTimer = 999.0f;
        aiStyle = (AI_STYLE)(Random.Range(0, 2));
        StyleChangeCheck();
    }

    protected override void IsItTimeToAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer > ATTACK_INTERVAL)
        {
            timeToAttack = true;
            attackTimer = 0;
        }
    }

    public override void FoundPlayer(bool found)
    {
        transform.FindChild("EnemyVision").gameObject.SetActive(false);
        Transform roomLocation = GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().WhatRoomAmIIn(transform);
        GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<CameraScript>().SetFocus(roomLocation);
        player = GameObject.FindGameObjectWithTag(Tags.PLAYER);

        if (!isHurt && !attacking && !isDead && null == player)
            velocity = new Vector2(0, 0);
    }

    public override void TakeDamage(DamageInfo info)
    {
        base.TakeDamage(info);

        if (0 >= health)
        {
            health = 0;
            GameObject.Find(Tags.MAIN_CAMERA).GetComponent<CameraScript>().SetFocus(null);
        }
    }

    protected override void AttackPlayer(float deltaX, float deltaZ)
    {
        switch (aiStyle)
        {
            case AI_STYLE.Turtle:
                attacking = true;
                timeToAttack = false;
                if (Random.Range(0, 4) >= 3)
                    animator.SetTrigger("HeavyAttack2");
                break;
            case AI_STYLE.Assault:
                if (deltaX > 0.8f)
                {
                    if (Random.Range(0, 9) >= 9)
                    {
                        attacking = true;
                        timeToAttack = false;
                        animator.SetTrigger("HeavyAttack4");
                    }
                    else
                        velocity.x = cInfo.maxSpeed * facingPlayerX;
                }
                else if (deltaZ < 0.3f)
                {
                    attacking = true;
                    timeToAttack = false;
                    if (Random.Range(0, 4) >= 3)
                        animator.SetTrigger("HeavyAttack3");
                    else
                        animator.SetTrigger("LightCombo");
                }
                break;
            default:
                break;
        }
    }

    protected override bool MoveToPlayer(float deltaX, float deltaZ)
    {
        if (deltaZ > 0.2f)
            velocity.y = (cInfo.maxSpeed * 0.5f) * facingPlayerY;
        else
            velocity.y = 0;

        if (timeToAttack) return false; // Still want the Y to be checked, just not the X

        switch (aiStyle)
        {
            case AI_STYLE.Turtle:
                if (deltaX > 2.0f)
                    velocity.x = cInfo.maxSpeed * facingPlayerX;
                else if (deltaX < 1.6f)
                    velocity.x = cInfo.maxSpeed * -facingPlayerX;
                else
                    velocity.x = 0;
                break;
            case AI_STYLE.Assault:
                if (deltaX > 0.8f)
                    velocity.x = cInfo.maxSpeed * facingPlayerX;
                else if (deltaX < 0.4f)
                    velocity.x = cInfo.maxSpeed * -facingPlayerX;
                else
                    velocity.x = 0;
                break;
            default:
                break;
        }

        // NEED TO SET WHEN A BOUND IS HIT SO THE RANDOM MOVEMENT DOESN'T BLOW EVERYTHING UP
        return (0 != velocity.x || 0 != velocity.y);
    }

    // No Random AI Movement
    protected override void AIMovement()
    {
        return;
    }

    void Update()
    {
        base.BaseUpdate();
        styleTimer += Time.deltaTime;

        StyleChangeCheck();
    }

    void StyleChangeCheck()
    {
        if (styleTimer > STYLE_INTERVAL)
        {
            switch (aiStyle)
            {
                case AI_STYLE.Turtle:
                    aiStyle = AI_STYLE.Assault;
                    ATTACK_INTERVAL = 8.0f;
                    break;
                case AI_STYLE.Assault:
                    aiStyle = AI_STYLE.Turtle;
                    ATTACK_INTERVAL = 5.0f;
                    break;
                default:
                    break;
            }

            styleTimer = 0;
        }
    }

    public void Normal()
    {
        attacking = true;
        animator.ResetTrigger("OnHit");

        if (!inAir) StopMovement();
        comboString += "N";
    }

    public void Special()
    {
        attacking = true;
        animator.ResetTrigger("OnHit");

        if (!inAir) StopMovement();
        comboString += "Sp";
    }

    public void Special2()
    {
        Vector3 projectileOffset = new Vector3(transform.position.x + hadoukenOffset.x * transform.localScale.x, transform.position.y + hadoukenOffset.y, transform.position.z);
        GameObject instance = Instantiate(hadouken, projectileOffset, Quaternion.identity) as GameObject;
        instance.GetComponent<ProjectileBaseScript>().SetProjectileInfo(projectiles["Hadouken"], this.tag, transform.localScale.x);
    }

    public void Special3()
    {
        inAir = true;
        velocity.y = 8;
        startPosition.y = transform.position.y;
    }

    public void Special4()
    {
        velocity.x = 2 * transform.localScale.x;
        horizDampEnable = false;
    }

    public void SetSpawnOnGround(Vector2 spawn)
    {
        this.spawn = spawn;
    }
}
