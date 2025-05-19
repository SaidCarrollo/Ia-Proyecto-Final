using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionFlee : ActionNodeVehicle
{
    private bool isFleeing = false;
    private Vector3 fleeDestination;

    public override void OnStart()
    {
        base.OnStart();
        isFleeing = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (_IACharacterVehiculo.health.IsDead)
            return TaskStatus.Failure;

        // Lógica de interrupción solo para Carnívoros
        if (isFleeing && _UnitGame == UnitGame.Carnivore && _IACharacterVehiculo.health.health >= 50)
        {
            ResetFlee();
            return TaskStatus.Failure;
        }

        if (!isFleeing)
        {
            switch (_UnitGame)
            {
                case UnitGame.Carnivore:
                    if (_IACharacterVehiculo.health.health < 50)
                        StartFlee();
                    break;

                case UnitGame.Herbivore:
                    StartFlee(); // Herbívoro huye sin condición de salud
                    break;
            }
        }

        return MonitorFleeProgress();
    }

    void StartFlee()
    {
        switch (_UnitGame)
        {
            case UnitGame.Herbivore:
                if (_IACharacterVehiculo is IACharacterVehiculoHerbivore herbivore)
                {
                    herbivore.FleeRandomDirection();
                    herbivore.LookPosition(herbivore.agent.destination);
                    fleeDestination = herbivore.agent.destination;
                    isFleeing = true;
                }
                break;

            case UnitGame.Carnivore:
                if (_IACharacterVehiculo is IACharacterVehiculoCarnivore carnivore)
                {
                    carnivore.FleeRandomDirection();
                    carnivore.LookPosition(carnivore.agent.destination);
                    fleeDestination = carnivore.agent.destination;
                    isFleeing = true;
                }
                break;
        }
    }

    TaskStatus MonitorFleeProgress()
    {
        if (!isFleeing)
            return TaskStatus.Failure;

        if (HasReachedDestination())
        {
            ResetFlee();
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    bool HasReachedDestination()
    {
        return !_IACharacterVehiculo.agent.pathPending &&
               _IACharacterVehiculo.agent.remainingDistance <= _IACharacterVehiculo.agent.stoppingDistance;
    }

    void ResetFlee()
    {
        isFleeing = false;
        _IACharacterVehiculo.agent.ResetPath();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ResetFlee();
    }
}