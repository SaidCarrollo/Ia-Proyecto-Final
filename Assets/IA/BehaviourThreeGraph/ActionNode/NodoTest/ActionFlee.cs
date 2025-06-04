using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionFlee : ActionNodeVehicle // ActionNodeVehicle hereda de ActionNode y tiene _IACharacterVehiculo
{
    // Ya no necesitamos 'isFleeing' ni 'fleeDestination' aquí.
    // Esas se gestionan en IACharacterVehiculo.

    private FleeFuzzySystem _fleeFuzzySystem;

    private const float CARNIVORE_FLEE_THRESHOLD = 0.5f;
    private const float CARNIVORE_STOP_FLEE_THRESHOLD = 0.45f;
    private const float HERBIVORE_FLEE_THRESHOLD = 0.4f;

    public override void OnStart()
    {
        base.OnStart(); // Esto inicializa _IACharacterVehiculo y _UnitGame
        if (_fleeFuzzySystem == null)
        {
            _fleeFuzzySystem = new FleeFuzzySystem();
        }
        // No es necesario llamar a _IACharacterVehiculo.ConcludeFleeState() aquí,
        // OnEnd() se encargará de limpiar si la tarea es abortada.
    }

    public override TaskStatus OnUpdate()
    {
        if (_IACharacterVehiculo == null || _IACharacterVehiculo.health.IsDead)
        {
            // Si estaba huyendo y muere, asegurarse de limpiar el estado
            if (_IACharacterVehiculo != null && _IACharacterVehiculo.IsCurrentlyFleeing)
            {
                _IACharacterVehiculo.ConcludeFleeState();
            }
            return TaskStatus.Failure;
        }

        _fleeFuzzySystem.CurrentHealth = _IACharacterVehiculo.health.health;
        _fleeFuzzySystem.CalculateFleeDecision(_UnitGame); // _UnitGame se obtiene de ActionNode
        float currentFleeStrength = _fleeFuzzySystem.FleeDecisionStrength;

        // Lógica de interrupción para Carnívoros
        if (_IACharacterVehiculo.IsCurrentlyFleeing && _UnitGame == UnitGame.Carnivore)
        {
            if (currentFleeStrength < CARNIVORE_STOP_FLEE_THRESHOLD)
            {
                _IACharacterVehiculo.ConcludeFleeState();
                return TaskStatus.Failure; // La condición para huir (estar bajo de salud) ya no se cumple
            }
        }

        // Lógica para empezar a huir
        if (!_IACharacterVehiculo.IsCurrentlyFleeing)
        {
            bool shouldStartFleeing = false;
            switch (_UnitGame)
            {
                case UnitGame.Carnivore:
                    if (currentFleeStrength > CARNIVORE_FLEE_THRESHOLD)
                        shouldStartFleeing = true;
                    break;

                case UnitGame.Herbivore:
                    if (currentFleeStrength > HERBIVORE_FLEE_THRESHOLD)
                        shouldStartFleeing = true;
                    break;
            }

            if (shouldStartFleeing)
            {
                _IACharacterVehiculo.InitiateFleeState();
                // Inmediatamente después de iniciar, es improbable que haya llegado,
                // por lo que se devolverá Running más abajo.
            }
            else
            {
                // Si no debe empezar a huir y no estaba huyendo, esta tarea falla.
                return TaskStatus.Failure;
            }
        }

        // Monitorear el progreso de la huida si está huyendo
        if (_IACharacterVehiculo.IsCurrentlyFleeing)
        {
            if (_IACharacterVehiculo.HasReachedFleeDestination())
            {
                _IACharacterVehiculo.ConcludeFleeState();
                return TaskStatus.Success; // Llegó a destino
            }
            return TaskStatus.Running; // Sigue huyendo
        }

        // Si llega aquí, significa que no está huyendo y no se decidió empezar a huir en este frame.
        // Esto podría pasar si CARNIVORE_STOP_FLEE_THRESHOLD se cumple para un herbívoro,
        // o si la lógica inicial de `if (!_IACharacterVehiculo.IsCurrentlyFleeing)` no resultó en `shouldStartFleeing`.
        // Por seguridad, si no está huyendo y no se cumplieron otras condiciones, se considera Failure.
        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        // Asegurarse de que el personaje deje de huir si la tarea termina
        // por cualquier razón (éxito, fallo, o abortada por el Behavior Tree).
        if (_IACharacterVehiculo != null && _IACharacterVehiculo.IsCurrentlyFleeing)
        {
            _IACharacterVehiculo.ConcludeFleeState();
        }
        base.OnEnd();
    }
}