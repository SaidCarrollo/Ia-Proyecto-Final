using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionFlee : ActionNodeVehicle
{
    // Ya no se necesita ninguna variable de lógica difusa o umbrales aquí.

    public override void OnStart()
    {
        base.OnStart();
        // La inicialización del sistema de huida se maneja en IACharacterVehiculo.
    }

    public override TaskStatus OnUpdate()
    {
        if (_IACharacterVehiculo == null || _IACharacterVehiculo.health.IsDead)
        {
            return TaskStatus.Failure;
        }

        // La lógica compleja ha sido movida. Ahora solo llamamos a un método,
        // igual que en el nodo de ataque.
        return _IACharacterVehiculo.EvaluateAndExecuteFlee();
    }

    public override void OnEnd()
    {
        // Es una buena práctica asegurarse de que el estado de huida se limpie
        // si el árbol de comportamiento aborta esta tarea.
        if (_IACharacterVehiculo != null && _IACharacterVehiculo.IsCurrentlyFleeing)
        {
            _IACharacterVehiculo.ConcludeFleeState();
        }
        base.OnEnd();
    }
}