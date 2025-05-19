using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Check")]
public class IsLowheatlh : ActionNodeVehicle
{
    [SerializeField] float healthThreshold = 50f; // Umbral configurable en el Inspector

    public override TaskStatus OnUpdate()
    {
        // Verificar si hay enemigo visible
        if (_IACharacterVehiculo.AIEye.ViewEnemy == null)
            return TaskStatus.Failure;

        // Obtener componente Health del enemigo
        Health enemyHealth = _IACharacterVehiculo.AIEye.ViewEnemy;

        // Verificar si el enemigo está muerto o con vida baja
        if (enemyHealth.IsDead || enemyHealth.health < healthThreshold)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}