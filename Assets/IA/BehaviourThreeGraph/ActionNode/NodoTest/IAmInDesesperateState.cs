using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Check")]
[TaskDescription("Verifica si la unidad tiene una cantidad de vida críticamente baja, para un ataque desesperado.")]
public class IAmInDesperateState : ActionNodeVehicle
{
    [SerializeField] float desperateHealthThreshold = 25f; 

    public override TaskStatus OnUpdate()
    {
        if (_IACharacterVehiculo.health == null)
        {
            return TaskStatus.Failure;
        }

        if (_IACharacterVehiculo.health.health < desperateHealthThreshold && !_IACharacterVehiculo.health.IsDead)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}