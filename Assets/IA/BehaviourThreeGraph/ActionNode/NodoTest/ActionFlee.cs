using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionFlee : ActionNodeVehicle // ActionNodeVehicle hereda de ActionNode y tiene _IACharacterVehiculo
{
    private FleeFuzzySystem _fleeFuzzySystem;
    private FleeFuzzyConfig _fleeConfig; // Referencia al componente de configuración

    // Umbrales (estos se mantienen)
    private const float CARNIVORE_FLEE_THRESHOLD = 0.5f;
    private const float CARNIVORE_STOP_FLEE_THRESHOLD = 0.45f;
    private const float HERBIVORE_FLEE_THRESHOLD = 0.4f;

    public override void OnStart()
    {
        base.OnStart();

        // Obtener el componente de configuración del GameObject
        _fleeConfig = GetComponent<FleeFuzzyConfig>();
        if (_fleeConfig == null)
        {
            Debug.LogError("El componente FleeFuzzyConfig no se encontró en el Agente.", gameObject);
            return;
        }

        // Seleccionar las curvas correctas según el tipo de unidad
        FleeCurves curvesToUse = (_UnitGame == UnitGame.Carnivore) ?
            _fleeConfig.CarnivoreFleeCurves :
            _fleeConfig.HerbivoreFleeCurves;

        // Inicializar el sistema difuso con las curvas seleccionadas
        _fleeFuzzySystem = new FleeFuzzySystem(
            curvesToUse.VeryLowHealthCurve,
            curvesToUse.LowHealthCurve,
            curvesToUse.ModerateHealthCurve,
            curvesToUse.HighHealthCurve
        );
    }

    public override TaskStatus OnUpdate()
    {
        if (_fleeFuzzySystem == null || _IACharacterVehiculo == null || _IACharacterVehiculo.health.IsDead)
        {
            if (_IACharacterVehiculo != null && _IACharacterVehiculo.IsCurrentlyFleeing)
            {
                _IACharacterVehiculo.ConcludeFleeState();
            }
            return TaskStatus.Failure;
        }

        // Asignar los valores de entrada al sistema difuso
        _fleeFuzzySystem.CurrentHealth = _IACharacterVehiculo.health.health;
        _fleeFuzzySystem.MaxHealth = _IACharacterVehiculo.health.healthMax; // ¡No olvides la vida máxima!
        _fleeFuzzySystem.CalculateFleeDecision(_UnitGame);

        float currentFleeStrength = _fleeFuzzySystem.FleeDecisionStrength;

        // Lógica de interrupción para Carnívoros
        if (_IACharacterVehiculo.IsCurrentlyFleeing && _UnitGame == UnitGame.Carnivore)
        {
            if (currentFleeStrength < CARNIVORE_STOP_FLEE_THRESHOLD)
            {
                _IACharacterVehiculo.ConcludeFleeState();
                return TaskStatus.Failure;
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

        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {

        if (_IACharacterVehiculo != null && _IACharacterVehiculo.IsCurrentlyFleeing)
        {
            _IACharacterVehiculo.ConcludeFleeState();
        }
        base.OnEnd();
    }
}