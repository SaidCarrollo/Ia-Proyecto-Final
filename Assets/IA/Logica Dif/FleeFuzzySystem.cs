using System;
using System.Collections.Generic;

// Asegúrate de que el enum 'UnitGame' esté definido en un lugar accesible.
// Si está dentro de un namespace específico, deberías añadir el 'using' correspondiente
// o cualificarlo completamente (ej. MyGame.Enums.UnitGame).
// Por ahora, asumimos que 'UnitGame' es directamente accesible.

public class FleeFuzzySystem
{
    // Entrada del sistema difuso
    public float CurrentHealth { get; set; } // Rango: 0-100

    // Salida del sistema difuso (después de la desfuzzificación)
    public float FleeDecisionStrength { get; private set; } // Rango: 0-1 (qué tan imperativo es huir)

    // Funciones de Pertenencia para CurrentHealth
    private float VeryLowHealthMembership()
    {
        if (CurrentHealth <= 10) return 1.0f;
        if (CurrentHealth >= 25) return 0.0f;
        return (25f - CurrentHealth) / 15f;
    }

    private float LowHealthMembership()
    {
        if (CurrentHealth <= 10 || CurrentHealth >= 40) return 0.0f;
        if (CurrentHealth > 10 && CurrentHealth <= 25) return (CurrentHealth - 10f) / 15f;
        return (40f - CurrentHealth) / 15f;
    }

    private float ModerateHealthMembership()
    {
        if (CurrentHealth <= 35 || CurrentHealth >= 65) return 0.0f;
        if (CurrentHealth > 35 && CurrentHealth <= 50) return (CurrentHealth - 35f) / 15f;
        return (65f - CurrentHealth) / 15f;
    }

    private float HighHealthMembership()
    {
        if (CurrentHealth <= 60) return 0.0f;
        if (CurrentHealth >= 75) return 1.0f;
        return (CurrentHealth - 60f) / 15f;
    }

    // Método para calcular la fortaleza de la decisión de huir
    // Se corrigió el tipo del parámetro 'unitType'
    public void CalculateFleeDecision(UnitGame unitType)
    {
        // 1. Fuzzificación
        float veryLowHealth = VeryLowHealthMembership();
        float lowHealth = LowHealthMembership();
        float moderateHealth = ModerateHealthMembership();
        float highHealth = HighHealthMembership();

        // 2. Evaluación de Reglas Difusas
        var ruleOutputs = new List<Tuple<float, float>>();

        const float NO_FLEE_STRENGTH = 0.1f;
        const float LOW_FLEE_STRENGTH = 0.4f;
        const float MEDIUM_FLEE_STRENGTH = 0.7f;
        const float HIGH_FLEE_STRENGTH = 0.95f;

        // Se corrigieron las referencias a los valores del enum 'UnitGame'
        if (unitType == UnitGame.Carnivore)
        {
            if (veryLowHealth > 0) ruleOutputs.Add(Tuple.Create(veryLowHealth, HIGH_FLEE_STRENGTH));
            if (lowHealth > 0) ruleOutputs.Add(Tuple.Create(lowHealth, MEDIUM_FLEE_STRENGTH));
            if (moderateHealth > 0) ruleOutputs.Add(Tuple.Create(moderateHealth, LOW_FLEE_STRENGTH));
            if (highHealth > 0) ruleOutputs.Add(Tuple.Create(highHealth, NO_FLEE_STRENGTH));
        }
        else // Herbivore (o cualquier otro tipo por defecto)
        {
            if (veryLowHealth > 0) ruleOutputs.Add(Tuple.Create(veryLowHealth, HIGH_FLEE_STRENGTH));
            if (lowHealth > 0) ruleOutputs.Add(Tuple.Create(lowHealth, 0.90f));
            if (moderateHealth > 0) ruleOutputs.Add(Tuple.Create(moderateHealth, MEDIUM_FLEE_STRENGTH));
            if (highHealth > 0) ruleOutputs.Add(Tuple.Create(highHealth, 0.5f));
        }

        // 3. Desfuzzificación
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