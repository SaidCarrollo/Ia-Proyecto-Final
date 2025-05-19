using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Check")]
public class IsNotLowheatlh : ActionNodeVehicle
{
    [SerializeField] float healthThreshold = 50f; // Configurable en el Inspector

    public override TaskStatus OnUpdate()
    {
        // Verificar si no hay enemigo visible
        if (_IACharacterVehiculo.AIEye.ViewEnemy == null)
            return TaskStatus.Failure;

        Health enemyHealth = _IACharacterVehiculo.AIEye.ViewEnemy;

        // Verificar si el enemigo está vivo Y con vida alta
        if (!enemyHealth.IsDead && enemyHealth.health >= healthThreshold)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}