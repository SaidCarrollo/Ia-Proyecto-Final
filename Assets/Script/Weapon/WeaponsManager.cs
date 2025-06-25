using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    [Header("Armas disponibles")]
    public List<WeaponBase> weapons = new List<WeaponBase>();

    [Header("Arma actual")]
    public WeaponBase currentWeaponBase;

    private int currentWeaponIndex = 0;

    void Start()
    {
        if (weapons.Count > 0)
        {
            SetCurrentWeapon(0);
        }
        else
        {
            Debug.LogWarning("WeaponsManager: No hay armas asignadas.");
        }
    }

    /// <summary>
    /// Cambia el arma actual a la que está en el índice dado.
    /// </summary>
    public void SetCurrentWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
        {
            Debug.LogWarning("WeaponsManager: Índice fuera de rango.");
            return;
        }

        currentWeaponIndex = index;
        currentWeaponBase = weapons[currentWeaponIndex];

        // Si estás activando/desactivando modelos, hazlo aquí
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(i == index);
        }

        Debug.Log($"Arma actual: {currentWeaponBase.weaponName}");
    }

    /// <summary>
    /// Dispara el arma actual.
    /// </summary>
    public void Fire()
    {
        if (currentWeaponBase == null)
        {
            Debug.LogWarning("WeaponsManager: No hay un arma seleccionada.");
            return;
        }

        currentWeaponBase.Shoot();
    }

    /// <summary>
    /// Cambia al siguiente arma en la lista (loop).
    /// </summary>
    public void NextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.Count;
        SetCurrentWeapon(nextIndex);
    }

    /// <summary>
    /// Cambia al arma anterior en la lista (loop).
    /// </summary>
    public void PreviousWeapon()
    {
        int prevIndex = (currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
        SetCurrentWeapon(prevIndex);
    }
}
