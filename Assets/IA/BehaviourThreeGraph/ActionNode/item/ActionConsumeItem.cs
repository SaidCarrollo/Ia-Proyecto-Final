using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("MyAI/Action")]
[TaskDescription("Consume el ítem si está dentro del rango especificado.")]
public class ActionConsumeItem : Action
{
    public SharedFloat consumeDistance = 1.5f;

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
        if (_IACharacterVehiculo == null || AIEye == null) return TaskStatus.Failure;

        if (_IACharacterVehiculo.health.IsDead || AIEye.ViewItem == null)
        {
            return TaskStatus.Failure;
        }

        float distanceToItem = Vector3.Distance(transform.position, AIEye.ViewItem.transform.position);

        if (distanceToItem > consumeDistance.Value)
        {
            return TaskStatus.Running;
        }

        AIEye.ViewItem.Consume(_IACharacterVehiculo.health);

        return TaskStatus.Success;
    }
}