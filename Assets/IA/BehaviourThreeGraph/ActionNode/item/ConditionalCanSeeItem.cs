using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Conditional")]
[TaskDescription("Revisa si la IA puede ver un ítem (Carne) y si necesita curarse.")]
public class ConditionalCanSeeItem : Conditional
{
    private IACharacterVehiculo _IACharacterVehiculo;
    private IAEyeBase AIEye;
    private Health _health;

    public override void OnStart()
    {
        _IACharacterVehiculo = GetComponent<IACharacterVehiculo>();
        if (_IACharacterVehiculo != null)
        {
            AIEye = _IACharacterVehiculo.AIEye as IAEyeBase;
            _health = _IACharacterVehiculo.health;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (AIEye == null || _health == null || AIEye.ViewItem == null || _health.IsDead)
        {
            return TaskStatus.Failure;
        }

        // Condición 1: ¿Necesita la IA curarse?
        bool needsHealth = _health.health < _health.healthMax;

        if (needsHealth)
        {
            Item visibleItem = AIEye.ViewItem;
            UnitGame unitType = _health._UnitGame;

            // Condición 2: ¿El ítem es apropiado para la unidad?
            if ((unitType == UnitGame.Carnivore || unitType == UnitGame.Hunter) && visibleItem.itemType == ItemType.Carne)
            {
                return TaskStatus.Success; // Carnívoros y Cazadores comen Carne.
            }

            if (unitType == UnitGame.Herbivore && visibleItem.itemType == ItemType.Planta)
            {
                return TaskStatus.Success; // Herbívoros comen Plantas.
            }
        }

        return TaskStatus.Failure;
    }
}