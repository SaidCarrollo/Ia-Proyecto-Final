
using UnityEngine;

[System.Serializable]
public class FleeCurves
{
    [Tooltip("Curva para Salud MUY BAJA (e.g., 0% a 25% de vida)")]
    public AnimationCurve VeryLowHealthCurve;

    [Tooltip("Curva para Salud BAJA (e.g., 10% a 40% de vida)")]
    public AnimationCurve LowHealthCurve;

    [Tooltip("Curva para Salud MODERADA (e.g., 35% a 65% de vida)")]
    public AnimationCurve ModerateHealthCurve;

    [Tooltip("Curva para Salud ALTA (e.g., 60% a 100% de vida)")]
    public AnimationCurve HighHealthCurve;
}

public class FleeFuzzyConfig : MonoBehaviour
{
    [Header("Configuración para Carnívoros")]
    public FleeCurves CarnivoreFleeCurves;

    [Header("Configuración para Herbívoros")]
    public FleeCurves HerbivoreFleeCurves;
}