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

    /// <summary>
    /// Genera el prefab del rastro de la bala desde el cañón hasta un punto final.
    /// </summary>
    /// <param name="endPoint">El punto del mundo donde el rastro debe terminar.</param>
    private void SpawnBulletTrail(Vector3 endPoint)
    {
        if (bulletTrailPrefab == null) return; // Si no hay prefab asignado, no hacer nada

        // Determina el punto de origen del efecto visual
        Transform muzzleTransform = _MuzzleFlashWeamon.root != null ? _MuzzleFlashWeamon.root.transform : this.transform;

        // Instanciar el prefab del rastro en la posición del cañón
        GameObject trailObject = Instantiate(bulletTrailPrefab, muzzleTransform.position, Quaternion.identity);

        // Obtener el script del rastro y decirle a dónde ir
        BulletTrail trail = trailObject.GetComponent<BulletTrail>();
        if (trail != null)
        {
            trail.SetTarget(endPoint);
        }
        else
        {
            // Si el prefab no tiene el script, destruir el objeto para no dejar basura
            Debug.LogWarning("El prefab del rastro de bala no tiene el script 'BulletTrail'.");
            Destroy(trailObject);
        }
    }

    public override void Shoot()
    {
        if (!canShoot) return; //
        if (Time.time < FrameRate) return; //
        FrameRate = Time.time + Rate; //

        _MuzzleFlashWeamon.Play(); //
        Transform muzzleTransform = _MuzzleFlashWeamon.root != null ? _MuzzleFlashWeamon.root.transform : this.transform; //

        // --- FASE 1: VERIFICACIÓN A QUEMARROPA ---
        Vector3 pointBlankCenter = muzzleTransform.position + muzzleTransform.forward * pointBlankCheckDistance; //
        Collider[] pointBlankHits = Physics.OverlapSphere(pointBlankCenter, pointBlankCheckRadius, enemyLayer); //

        if (pointBlankHits.Length > 0)
        {
            Collider targetCollider = pointBlankHits[0]; //
            Vector3 hitPoint = targetCollider.ClosestPoint(pointBlankCenter); //
            Debug.Log("<color=magenta>¡Impacto a QUEMARROPA! Objeto: " + targetCollider.name + "</color>", targetCollider.gameObject); //
            ProcessHit(targetCollider, hitPoint); //
            SpawnBulletTrail(hitPoint); // Llamar a la función del rastro con el punto de impacto
        }
        else
        {
            // --- FASE 2: DISPARO A DISTANCIA ---
            RaycastHit hit; //
            Vector3 rayOrigin = muzzleTransform.position; //
            Vector3 rayDirection = muzzleTransform.forward; //
            float longRangeSphereRadius = 0.1f; //
            float maxDistance = 100f;

            if (Physics.SphereCast(rayOrigin, longRangeSphereRadius, rayDirection, out hit, maxDistance, enemyLayer)) //
            {
                Debug.Log("<color=green>¡Impacto a DISTANCIA! Objeto: " + hit.collider.name + "</color>", hit.collider.gameObject); //
                ProcessHit(hit.collider, hit.point); //
                SpawnBulletTrail(hit.point); // Llamar a la función del rastro con el punto de impacto
            }
            else
            {
                // No se impactó nada
                Debug.Log("<color=red>Disparo al aire. Ninguna detección tuvo éxito.</color>"); //
                Vector3 endPoint = rayOrigin + rayDirection * maxDistance; // Calcula el punto final en el aire
                _MuzzleFlashWeamon.LookAtPosition(endPoint); //
                SpawnBulletTrail(endPoint); // Llamar a la función del rastro con el punto final en el aire
            }
        }

        // El conteo de balas se reduce sin importar si se impactó o no, porque el disparo se realizó.
        _countbullet--; //
        if (_countbullet <= 0)
        {
            Debug.Log("Out of ammo!"); //
        }
    }

    // Gizmos para ver las áreas de detección en el editor.
    private void OnDrawGizmosSelected()
    {
        // Intenta encontrar el transform del muzzle flash, si no, usa el transform del arma.
        Transform muzzleTransform = null; //
        if (_MuzzleFlashWeamon != null && _MuzzleFlashWeamon.root != null)
        {
            muzzleTransform = _MuzzleFlashWeamon.root.transform; //
        }
        else
        {
            muzzleTransform = this.transform; //
        }

        // Dibuja la esfera de detección a quemarropa
        Gizmos.color = new Color(1, 0, 1, 0.5f); //
        Vector3 pointBlankCenter = muzzleTransform.position + muzzleTransform.forward * pointBlankCheckDistance; //
        Gizmos.DrawSphere(pointBlankCenter, pointBlankCheckRadius); //
    }
}
