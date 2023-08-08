using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject attackArena;

    private bool isRight = true;

    private Character target;
    public Character Target => target;

    // Update is called once per frame
    private void Update()
    {
        if(currentState != null)
        {
            currentState.OnExcute(this);
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        ChangeState(new IdleState());
        DeActiveAttackArena();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        ChangeState(null);
        base.OnDeath();
    }

    private InterfaceState currentState;

    public void ChangeState(InterfaceState newState)
    {
        if (currentState != null && !isDeath)
        {
            currentState.OnExit(this);
        }

        currentState = newState;

        if(currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    internal void SetTarget(Character character)
    {
        this.target = character;

        if (IstargetInRange())
        {
            ChangeState(new AttackState());
        } else if(Target != null)
        {
            ChangeState(new PatrolState());
        } else
        {
            ChangeState(new IdleState());
        }
    }

    public void Moving()
    {
        ChangeAnim("is_run");

        rb.velocity = transform.right * moveSpeed;
    }

    public void StopMoving()
    {
        ChangeAnim("is_idle");
        rb.velocity = Vector2.zero;
    }

    public void Attack()
    {
        ChangeAnim("is_attack");
        ActiveAttackArena();
        Invoke(nameof(DeActiveAttackArena), 0.5f);
    }

    public bool IstargetInRange()
    {
        if(target != null && Vector2.Distance(target.transform.position, transform.position) <= attackRange)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyWall")
        {
            ChangeDirection(!isRight);
        }
    }

    public void ChangeDirection(bool isRight)
    {
        this.isRight = isRight;
        transform.rotation = isRight ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180);
    }

    private void ActiveAttackArena()
    {
        attackArena.SetActive(true);
    }

    private void DeActiveAttackArena()
    {
        attackArena.SetActive(false);
    }

}
