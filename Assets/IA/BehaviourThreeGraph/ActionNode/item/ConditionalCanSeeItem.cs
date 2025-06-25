using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Conditional")]
[TaskDescription("Revisa si la IA puede ver un ítem (Carne) y si necesita curarse.")]
public class ConditionalCanSeeItem : Conditional
{
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
        if (AIEye == null || _IACharacterVehiculo == null)
        {
            return TaskStatus.Failure;
        }

        if (AIEye.ViewItem != null && _IACharacterVehiculo.health.health < _IACharacterVehiculo.health.healthMax)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}