using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Action")]
public class ActionNodeConsumeItem : ActionNodeAction
{
    public override TaskStatus OnUpdate()
    {
        if (_IACharacterVehiculo.health.IsDead)
            return TaskStatus.Failure;

        SwitchUnitAndConsume();

        return TaskStatus.Success;
    }

    void SwitchUnitAndConsume()
    {
        switch (_UnitGame)
        {
            // Solo los animales consumen ítems de esta manera.
            case UnitGame.Herbivore:
            case UnitGame.Carnivore:
            case UnitGame.Hunter: // Si el Hunter también come carne para curarse
                if (_IACharacterActions is IACharacterActionsAnimal)
                {
                    ((IACharacterActionsAnimal)_IACharacterActions).ConsumeVisibleItem();
                }
                else if (_IACharacterActions is IACharacterActionsHunter) // Ejemplo si el Hunter no hereda de Animal
                {
                    // ((IACharacterActionsHunter)_IACharacterActions).ConsumeVisibleItem();
                }
                break;
        }
    }
}