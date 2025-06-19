using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/Move")]
public class ActionFollowEnemy : ActionNodeVehicle
{
    public override void OnStart()
    {
        base.OnStart();
    }
    public override TaskStatus OnUpdate()
    {
        if(_IACharacterVehiculo.health.IsDead)
            return TaskStatus.Failure;

        SwitchUnit();

        return TaskStatus.Success;

    }
    void SwitchUnit()
    {


        switch (_UnitGame)
        {
            case UnitGame.Zombie:
                if(_IACharacterVehiculo is IACharacterVehiculoZombie)
                {
                    ((IACharacterVehiculoZombie)_IACharacterVehiculo).MoveToEnemy();
                    ((IACharacterVehiculoZombie)_IACharacterVehiculo).LookEnemy();
                }

                break;
            case UnitGame.Soldier:
                if (_IACharacterVehiculo is IACharacterVehiculoSoldier)
                {
                    ((IACharacterVehiculoSoldier)_IACharacterVehiculo).MoveToEnemy();
                    ((IACharacterVehiculoSoldier)_IACharacterVehiculo).LookEnemy();
                }
                break;
            case UnitGame.Carnivore:
                if (_IACharacterVehiculo is IACharacterVehiculoCarnivore)
                {
                    ((IACharacterVehiculoCarnivore)_IACharacterVehiculo).MoveToEnemy();
                    ((IACharacterVehiculoCarnivore)_IACharacterVehiculo).LookEnemy();
                }
                break;
            case UnitGame.Herbivore:
                if (_IACharacterVehiculo is IACharacterVehiculoHerbivore)
                {
                    ((IACharacterVehiculoHerbivore)_IACharacterVehiculo).MoveToEnemy();
                    ((IACharacterVehiculoHerbivore)_IACharacterVehiculo).LookEnemy();
                   
                }
                break;
            case UnitGame.Hunter:
                if (_IACharacterVehiculo is IACharacterVehiculoHunter)
                {
                    ((IACharacterVehiculoHunter)_IACharacterVehiculo).MoveToEnemy();
                    ((IACharacterVehiculoHunter)_IACharacterVehiculo).LookEnemy();
                }
                break;
            case UnitGame.None:
                break;
            default:
                break;
        }



    }

}