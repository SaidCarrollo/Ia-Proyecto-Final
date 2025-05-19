using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Check")]
public class IAmNotLowheatlh : ActionNodeVehicle
{
    [SerializeField]
    float healthThreshold = 50f;

    public override TaskStatus OnUpdate()
    {
        // Validar componente Health
        if (_IACharacterVehiculo.health == null)
            return TaskStatus.Failure;

        // Verificar condiciones de salud alta
        bool isHealthy = _IACharacterVehiculo.health.health >= healthThreshold;
        bool isAlive = !_IACharacterVehiculo.health.IsDead;

        return (isHealthy && isAlive) ? TaskStatus.Success : TaskStatus.Failure;
    }
}