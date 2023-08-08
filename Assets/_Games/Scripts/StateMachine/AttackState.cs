using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : InterfaceState
{
    float timerAttack;
    public void OnEnter(Enemy enemy)
    {
        if(enemy.Target != null)
        {
            enemy.ChangeDirection(enemy.Target.transform.position.x > enemy.transform.position.x);
            enemy.StopMoving();
            enemy.Attack();
        }
        timerAttack = 0;
    }

    public void OnExcute(Enemy enemy)
    {
        timerAttack += Time.deltaTime;
        if(timerAttack >= 1.5f)
        {
            enemy.ChangeState(new PatrolState());
        }
    }

    public void OnExit(Enemy enemy)
    {
    }
}
