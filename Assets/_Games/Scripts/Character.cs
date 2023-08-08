using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private Animator anim;
    [SerializeField] protected Healthbar healthBar;
    [SerializeField] protected CombatText combatText;

    private int coinStarts;

    protected float hp = 100;

    private string currentAnimName;

    public bool isDeath => hp <= 0;

    private void Start()
    {
        OnInit();
    }

    public virtual void OnInit()
    {
        hp = 100;
        healthBar.OnInit(100, transform);
        coinStarts = PlayerPrefs.GetInt("coin");
        UIManager.instance.SetPoint(coinStarts);
    }

    public virtual void OnDespawn()
    {

    }

    protected virtual void OnDeath()
    {
        ChangeAnim("is_die");
        Invoke(nameof(OnDespawn), 2f);
    }

    protected void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            anim.ResetTrigger(animName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }


    public void OnHit(float damage)
    {
        if(!isDeath)
        {
            hp -= damage;
            if(isDeath)
            {
                hp = 0;
                OnDeath();
            }
            if(hp> 0)
            {
                Instantiate(combatText, transform.position + Vector3.up, Quaternion.identity).OnInit(damage);
            }
        }
        healthBar.SetNewHp(hp);
    }
}
