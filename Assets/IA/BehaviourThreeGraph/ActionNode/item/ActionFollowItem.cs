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

        if (distanceToItem > consumeDistance.Value)
        {
            // Sigue moviéndose si está lejos.
            _IACharacterVehiculo.MoveToPosition(itemPosition);
            return TaskStatus.Running;
        }
        else
        {
            // Ya está en rango. Su trabajo aquí ha terminado.
            // Detiene el movimiento para evitar pasarse de largo.
            _IACharacterVehiculo.MoveToPosition(transform.position);
            return TaskStatus.Success;
        }
    }
}