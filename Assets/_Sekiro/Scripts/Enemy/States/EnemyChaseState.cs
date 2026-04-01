using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using R3;
using UnityEngine.AI;


public class EnemyChaseState : IEnemyState
{
    private float _delay;
    private float _duration;
    NavMeshAgent agent;
    public EnemyChaseState(EnemyAI enemy) : base(enemy)
    {
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void Enter()
    {
        agent.SetDestination(Vector3.one);
    }
    protected override void Update()
    {
        Debug.Log($"isOnNavMesh:{agent.isOnNavMesh}");
    }

    public override void Exit()
    {
        base.Exit();
    }

}