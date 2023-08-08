using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InterfaceState
{
    void OnEnter(Enemy enemy);

    void OnExcute(Enemy enemy);

    void OnExit(Enemy enemy);
}
