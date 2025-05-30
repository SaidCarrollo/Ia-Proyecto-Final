using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/Move")]
public class ActionWander : ActionNodeVehicle
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
                    ((IACharacterVehiculoZombie)_IACharacterVehiculo).MoveToWander();
                    
                }

                break;
            case UnitGame.Soldier:
                if (_IACharacterVehiculo is IACharacterVehiculoSoldier)
                {
                    ((IACharacterVehiculoSoldier)_IACharacterVehiculo).MoveToWander();

                }
                break;
            case UnitGame.Carnivore:
                if (_IACharacterVehiculo is IACharacterVehiculoCarnivore)
                {
                    ((IACharacterVehiculoCarnivore)_IACharacterVehiculo).MoveToWander();

                }
                break;
            case UnitGame.Herbivore:
                if (_IACharacterVehiculo is IACharacterVehiculoHerbivore)
                {
                    ((IACharacterVehiculoHerbivore)_IACharacterVehiculo).MoveToWander();

                }

                break;
            case UnitGame.Hunter:
                if (_IACharacterVehiculo is IACharacterVehiculoHunter)
                {
                    ((IACharacterVehiculoHunter)_IACharacterVehiculo).MoveToWander();

                }
                break;
                
            case UnitGame.None:
                break;
            default:
                break;
        }



    }

}