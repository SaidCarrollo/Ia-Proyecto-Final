using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/Action")]
public class ActionNodeAttack : ActionNodeAction
{
     

    public override void OnStart()
    {
        base.OnStart();
    }
    public override TaskStatus OnUpdate()
    {
        if (_IACharacterVehiculo.health.IsDead)
            return TaskStatus.Failure;

        SwitchUnit();

        return TaskStatus.Success;

    }
    void SwitchUnit()
    {


        switch (_UnitGame)
        {
            case UnitGame.Zombie:
                if (_IACharacterActions is IACharacterActionsZombie)
                {
                    ((IACharacterActionsZombie)_IACharacterActions).Attack();
                }

                break;
            case UnitGame.Soldier:
                if (_IACharacterActions is IACharacterActionsSoldier)
                {
                    ((IACharacterActionsSoldier)_IACharacterActions).Attack();
                }

                break;
            case UnitGame.Hunter:
                if (_IACharacterActions is IACharacterActionsHunter)
                {
                    ((IACharacterActionsHunter)_IACharacterActions).Attack();
                }

                break;
            case UnitGame.Herbivore:
                if (_IACharacterActions is IACharacterActionsHerbivore)
                {
                    ((IACharacterActionsHerbivore)_IACharacterActions).Attack();
                }

                break;
            case UnitGame.Carnivore:
                if (_IACharacterActions is IACharacterActionsCarnivore)
                {
                    ((IACharacterActionsCarnivore)_IACharacterActions).Attack();
                }

                break;
            case UnitGame.None:
                break;
            default:
                break;
        }



    }
}