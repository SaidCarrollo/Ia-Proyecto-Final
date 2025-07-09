using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionFlee : ActionNodeVehicle
{
    // Variable para guardar la referencia específica al componente del animal.
    private IACharacterVehiculoAnimal _animal;

    /// <summary>
    /// Se llama una vez cuando la tarea comienza a ejecutarse.
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();
        _animal = _IACharacterVehiculo as IACharacterVehiculoAnimal;
    }

    /// <summary>
    /// Se llama en cada frame mientras la tarea está activa.
    /// </summary>
    public override TaskStatus OnUpdate()
    {
        // Si no es un animal (_animal es null) o si está muerto, la tarea falla.
        // Esto previene errores y asegura que solo los animales puedan usar esta acción.
        if (_animal == null || _animal.health.IsDead)
        {
            return TaskStatus.Failure;
        }

        // Llamamos al método de huida desde la referencia correcta del animal.
        return _animal.EvaluateAndExecuteFlee();
    }

    /// <summary>
    /// Se llama cuando la tarea termina (ya sea por éxito, fallo o porque fue abortada).
    /// </summary>
    public override void OnEnd()
    {
        // Nos aseguramos de limpiar el estado de huida si la tarea se interrumpe.
        if (_animal != null && _animal.IsCurrentlyFleeing)
        {
            _animal.ConcludeFleeState();
        }
        base.OnEnd();
    }
}