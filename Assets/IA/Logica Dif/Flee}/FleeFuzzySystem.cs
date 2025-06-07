// FleeFuzzySystem.cs (Modificado)
using System;
using System.Collections.Generic;
using UnityEngine; // Necesitamos esto para AnimationCurve

public class FleeFuzzySystem
{
    // ENTRADAS
    public float CurrentHealth { get; set; } // Rango 0-100
    public float MaxHealth { get; set; }     // Rango > 0

    // SALIDA
    public float FleeDecisionStrength { get; private set; }

    // CURVAS (se asignarán desde fuera)
    private AnimationCurve _veryLowHealthCurve;
    private AnimationCurve _lowHealthCurve;
    private AnimationCurve _moderateHealthCurve;
    private AnimationCurve _highHealthCurve;

    // Constructor que recibe las curvas
    public FleeFuzzySystem(AnimationCurve veryLow, AnimationCurve low, AnimationCurve moderate, AnimationCurve high)
    {
        _veryLowHealthCurve = veryLow;
        _lowHealthCurve = low;
        _moderateHealthCurve = moderate;
        _highHealthCurve = high;
    }

    // Método principal
    public void CalculateFleeDecision(UnitGame unitType)
    {
        // 1. Fuzzificación (usando las curvas)
        // Normalizamos la vida a un rango de 0 a 1 para usarla en la curva
        float normalizedHealth = (MaxHealth > 0) ? CurrentHealth / MaxHealth : 0;

        float veryLowHealth = _veryLowHealthCurve.Evaluate(normalizedHealth);
        float lowHealth = _lowHealthCurve.Evaluate(normalizedHealth);
        float moderateHealth = _moderateHealthCurve.Evaluate(normalizedHealth);
        float highHealth = _highHealthCurve.Evaluate(normalizedHealth);

        // 2. Evaluación de Reglas Difusas (esta lógica no cambia)
        var ruleOutputs = new List<Tuple<float, float>>();

        const float NO_FLEE_STRENGTH = 0.1f;
        const float LOW_FLEE_STRENGTH = 0.4f;
        const float MEDIUM_FLEE_STRENGTH = 0.7f;
        const float HIGH_FLEE_STRENGTH = 0.95f;

        if (unitType == UnitGame.Carnivore)
        {
            if (veryLowHealth > 0) ruleOutputs.Add(Tuple.Create(veryLowHealth, HIGH_FLEE_STRENGTH));
            if (lowHealth > 0) ruleOutputs.Add(Tuple.Create(lowHealth, MEDIUM_FLEE_STRENGTH));
            if (moderateHealth > 0) ruleOutputs.Add(Tuple.Create(moderateHealth, LOW_FLEE_STRENGTH));
            if (highHealth > 0) ruleOutputs.Add(Tuple.Create(highHealth, NO_FLEE_STRENGTH));
        }
        else // Herbivore
        {
            if (veryLowHealth > 0) ruleOutputs.Add(Tuple.Create(veryLowHealth, HIGH_FLEE_STRENGTH));
            if (lowHealth > 0) ruleOutputs.Add(Tuple.Create(lowHealth, 0.90f));
            if (moderateHealth > 0) ruleOutputs.Add(Tuple.Create(moderateHealth, MEDIUM_FLEE_STRENGTH));
            if (highHealth > 0) ruleOutputs.Add(Tuple.Create(highHealth, 0.5f));
        }

        // 3. Desfuzzificación (esta lógica no cambia)
        float numerator = 0f;
        float denominator = 0f;

        foreach (var output in ruleOutputs)
        {
            numerator += output.Item1 * output.Item2;
            denominator += output.Item1;
        }

        if (denominator > 0)
        {
            FleeDecisionStrength = numerator / denominator;
        }
        else
        {
            FleeDecisionStrength = (unitType == UnitGame.Carnivore) ? 0.0f : 0.5f;
        }
    }
}