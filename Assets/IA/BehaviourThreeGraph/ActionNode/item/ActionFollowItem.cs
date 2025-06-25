using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime; // Necesario para SharedFloat

[TaskCategory("MyAI/Move")] // Puedes mantener la categoría o cambiarla
[TaskDescription("Mueve la IA hacia la posición del ítem visible y lo consume cuando está en rango.")]
public class ActionFollowItem : Action
{
    // Traemos la distancia de consumo aquí
    [Header("Distancia para consumir el ítem")]
    public SharedFloat consumeDistance = 3.5f;

    private IACharacterVehiculo _IACharacterVehiculo;
    private IAEyeBase AIEye;

    public override void OnStart()
    {
        _IACharacterVehiculo = GetComponent<IACharacterVehiculo>();
        if (_IACharacterVehiculo != null)
        {
            AIEye = _IACharacterVehiculo.AIEye as IAEyeBase;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_IACharacterVehiculo == null || AIEye == null || AIEye.ViewItem == null || _IACharacterVehiculo.health.IsDead)
        {
            // Si no hay item o la IA está muerta, la acción falla.
            return TaskStatus.Failure;
        }

        // Calculamos la distancia al ítem
        Vector3 itemPosition = AIEye.ViewItem.transform.position;
        float distanceToItem = Vector3.Distance(transform.position, itemPosition);

        // Si estamos lejos, seguimos moviéndonos
        if (distanceToItem > consumeDistance.Value)
        {
            _IACharacterVehiculo.MoveToPosition(itemPosition);
            _IACharacterVehiculo.LookPosition(itemPosition);

            // Devolvemos Running porque la acción aún no ha terminado.
            return TaskStatus.Running;
        }
        else // Si ya estamos lo suficientemente cerca
        {
            // Consumimos el ítem
            AIEye.ViewItem.Consume(_IACharacterVehiculo.health);

            // ¡Hemos terminado! Devolvemos Success para que el árbol pueda continuar.
            return TaskStatus.Success;
        }
    }
}