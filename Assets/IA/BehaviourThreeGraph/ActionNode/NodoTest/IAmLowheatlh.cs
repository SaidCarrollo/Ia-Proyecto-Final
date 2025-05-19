using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Check")]
public class IAmLowheatlh : ActionNodeVehicle
{
    [SerializeField] float healthThreshold = 50f; // Umbral configurable

    public override TaskStatus OnUpdate()
    {
        // Verificar si el componente Health existe
        if (_IACharacterVehiculo.health == null)
            return TaskStatus.Failure;

        // Comprobar salud actual y umbral
        if (_IACharacterVehiculo.health.health < healthThreshold && !_IACharacterVehiculo.health.IsDead)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}