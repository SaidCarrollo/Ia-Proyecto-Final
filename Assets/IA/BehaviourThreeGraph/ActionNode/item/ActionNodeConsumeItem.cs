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
            case UnitGame.Herbivore:
            case UnitGame.Carnivore:
                if (_IACharacterActions is IACharacterActionsAnimal animalActions)
                {
                    animalActions.ConsumeVisibleItem();
                }
                break;

            case UnitGame.Hunter:
                if (_IACharacterActions is IACharacterActionsHunter hunterActions)
                {
                    hunterActions.ConsumeVisibleItem(); //  Ahora sí se llama correctamente
                }
                break;
        }
    }
}