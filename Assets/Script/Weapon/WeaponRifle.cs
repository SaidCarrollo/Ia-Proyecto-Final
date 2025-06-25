using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRifle : WeaponBase
{
    public LayerMask enemyLayer;
    // Start is called before the first frame update
    void Awake()
    {
        LoadComponent();
    }
    public override void LoadComponent()
    {
        base.LoadComponent();
    }
    // Update is called once per frame
    public virtual void Shoot()
    {
        if (!canShoot) return;

        if (Time.time < FrameRate) return;
        FrameRate = Time.time + Rate;

        _MuzzleFlashWeamon.Play(); //

        Transform muzzleTransform = _MuzzleFlashWeamon.root != null ? _MuzzleFlashWeamon.root.transform : this.transform;
        Vector3 rayOrigin = muzzleTransform.position;
        Vector3 rayDirection = muzzleTransform.forward;
        Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.cyan, 2.0f);
        RaycastHit hit;
        // Se lanza el Raycast desde la posición y dirección del cañón del arma.
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 100f, enemyLayer))
        {
            Debug.Log("<color=green>¡Impacto de Raycast! Objeto: " + hit.collider.name + "</color>", hit.collider.gameObject);
            _MuzzleFlashWeamon.LookAtPosition(hit.point);

            Health targetHealth = hit.collider.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.Damage(damage, ownerHealth);
            }
            else
            {
                // --- AÑADE ESTA LÍNEA ---
                Debug.LogWarning("<color=yellow>El objeto impactado '" + hit.collider.name + "' no tiene un componente Health.</color>", hit.collider.gameObject);
            }
        }
        else
        {
            Debug.Log("<color=red>Disparo al aire. El Raycast no impactó con nada.</color>");
            // Si no se impacta nada, los efectos visuales apuntan hacia adelante en la dirección del disparo.
            _MuzzleFlashWeamon.LookAtPosition(rayOrigin + rayDirection * 100f);
        }

        _countbullet--;
        if (_countbullet <= 0)
        {
            Debug.Log("Out of ammo!");
        }
    }
}
