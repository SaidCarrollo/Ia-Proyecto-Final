using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRifle : WeaponBase
{
    public LayerMask enemyLayer;

    [Tooltip("Distancia desde el cañón para centrar la esfera de detección a quemarropa.")]
    public float pointBlankCheckDistance = 0.25f; // A 25cm delante del cañón

    [Tooltip("Radio de la esfera de detección a quemarropa.")]
    public float pointBlankCheckRadius = 0.3f; // Una esfera de 30cm de radio
    void Awake()
    {
        LoadComponent();
    }

    public override void LoadComponent()
    {
        base.LoadComponent();
    }

    // Update is called once per frame
    // --- SE HA CAMBIADO 'virtual' POR 'override' ---
    private void ProcessHit(Collider targetCollider, Vector3 hitPoint)
    {
        _MuzzleFlashWeamon.LookAtPosition(hitPoint);
        Health targetHealth = targetCollider.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.Damage(damage, ownerHealth);
        }
        else
        {
            Debug.LogWarning("<color=yellow>El objeto impactado '" + targetCollider.name + "' no tiene un componente Health.</color>", targetCollider.gameObject);
        }
    }

    public override void Shoot()
    {
        if (!canShoot) return;
        if (Time.time < FrameRate) return;
        FrameRate = Time.time + Rate;

        _MuzzleFlashWeamon.Play();
        Transform muzzleTransform = _MuzzleFlashWeamon.root != null ? _MuzzleFlashWeamon.root.transform : this.transform;

        // --- FASE 1: VERIFICACIÓN A QUEMARROPA ---
        Vector3 pointBlankCenter = muzzleTransform.position + muzzleTransform.forward * pointBlankCheckDistance;
        Collider[] pointBlankHits = Physics.OverlapSphere(pointBlankCenter, pointBlankCheckRadius, enemyLayer);

        if (pointBlankHits.Length > 0)
        {
            Collider targetCollider = pointBlankHits[0];
            Vector3 hitPoint = targetCollider.ClosestPoint(pointBlankCenter);
            Debug.Log("<color=magenta>¡Impacto a QUEMARROPA! Objeto: " + targetCollider.name + "</color>", targetCollider.gameObject);
            ProcessHit(targetCollider, hitPoint); // Llamamos a la función auxiliar
        }
        else
        {
            // --- FASE 2: DISPARO A DISTANCIA ---
            RaycastHit hit; // Declaramos 'hit' aquí, donde se va a usar.
            Vector3 rayOrigin = muzzleTransform.position;
            Vector3 rayDirection = muzzleTransform.forward;
            float longRangeSphereRadius = 0.1f;

            if (Physics.SphereCast(rayOrigin, longRangeSphereRadius, rayDirection, out hit, 100f, enemyLayer))
            {
                Debug.Log("<color=green>¡Impacto a DISTANCIA! Objeto: " + hit.collider.name + "</color>", hit.collider.gameObject);
                ProcessHit(hit.collider, hit.point); // Llamamos a la función auxiliar con los datos del SphereCast
            }
            else
            {
                // No se impactó nada
                Debug.Log("<color=red>Disparo al aire. Ninguna detección tuvo éxito.</color>");
                _MuzzleFlashWeamon.LookAtPosition(muzzleTransform.position + muzzleTransform.forward * 100f);
            }
        }

        // El conteo de balas se reduce sin importar si se impactó o no, porque el disparo se realizó.
        _countbullet--;
        if (_countbullet <= 0)
        {
            Debug.Log("Out of ammo!");
        }
    }

    // Gizmos para ver las áreas de detección en el editor.
    private void OnDrawGizmosSelected()
    {
        // Intenta encontrar el transform del muzzle flash, si no, usa el transform del arma.
        Transform muzzleTransform = null;
        if (_MuzzleFlashWeamon != null && _MuzzleFlashWeamon.root != null)
        {
            muzzleTransform = _MuzzleFlashWeamon.root.transform;
        }
        else
        {
            muzzleTransform = this.transform;
        }

        // Dibuja la esfera de detección a quemarropa
        Gizmos.color = new Color(1, 0, 1, 0.5f); // Magenta semitransparente
        Vector3 pointBlankCenter = muzzleTransform.position + muzzleTransform.forward * pointBlankCheckDistance;
        Gizmos.DrawSphere(pointBlankCenter, pointBlankCheckRadius);
    }
}
