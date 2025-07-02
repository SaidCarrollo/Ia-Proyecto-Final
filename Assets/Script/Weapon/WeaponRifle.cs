using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRifle : WeaponBase
{
    public LayerMask enemyLayer;

    [Tooltip("Distancia desde el cañón para centrar la esfera de detección a quemarropa.")]
    public float pointBlankCheckDistance = 0.25f;

    [Tooltip("Radio de la esfera de detección a quemarropa.")]
    public float pointBlankCheckRadius = 0.3f;

    [Header("Configuración de bala")]
    public GameObject bulletPrefab; // Prefab de la bala física
    public float bulletSpeed = 50f; // Velocidad de la bala
    public float bulletLifetime = 2f; // Tiempo de vida de la bala

    void Awake()
    {
        LoadComponent();
    }

    public override void LoadComponent()
    {
        base.LoadComponent();
    }

    private void ProcessHit(Collider targetCollider, Vector3 hitPoint)
    {
        // NO es necesario modificar _MuzzleFlashWeamon.LookAtPosition aquí, 
        // ya que el rastro se encargará de la dirección visual.
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
    private void GenerateTrail(Vector3 endPoint)
    {
        if (bulletTrailPrefab == null) return;

        // Obtenemos el punto de origen del disparo desde el MuzzleFlash
        Transform muzzleTransform = _MuzzleFlashWeamon.root != null ? _MuzzleFlashWeamon.root.transform : this.transform;

        // Creamos una instancia del prefab del rastro
        GameObject trailInstance = Instantiate(bulletTrailPrefab, muzzleTransform.position, Quaternion.identity);

        // Hacemos que el rastro "mire" hacia el punto de impacto
        trailInstance.transform.LookAt(endPoint);
    }

    /// <summary>
    /// Dispara una bala física hacia la posición objetivo
    /// </summary>
    private void FireBullet(Vector3 startPosition, Vector3 targetPosition)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("WeaponRifle: No hay prefab de bala asignado.");
            return;
        }

        // Calcular dirección
        Vector3 direction = (targetPosition - startPosition).normalized;

        // Instanciar bala
        GameObject bullet = Instantiate(bulletPrefab, startPosition, Quaternion.identity);

        // Orientar bala hacia la dirección
        bullet.transform.forward = direction;

        // Configurar bala
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript == null)
        {
            // Si no tiene el script, añadirlo
            bulletScript = bullet.AddComponent<Bullet>();
        }

        bulletScript.Initialize(direction, bulletSpeed, bulletLifetime);
    }

    public override void Shoot()
    {
        if (!canShoot) return;
        if (Time.time < FrameRate) return;
        FrameRate = Time.time + Rate;

        _MuzzleFlashWeamon.Play();
        Transform muzzleTransform = _MuzzleFlashWeamon.root != null ? _MuzzleFlashWeamon.root.transform : this.transform;
        Vector3 muzzlePosition = muzzleTransform.position;
        Vector3 muzzleDirection = muzzleTransform.forward;

        // --- FASE 1: VERIFICACIÓN A QUEMARROPA ---
        Vector3 pointBlankCenter = muzzlePosition + muzzleDirection * pointBlankCheckDistance;
        Collider[] pointBlankHits = Physics.OverlapSphere(pointBlankCenter, pointBlankCheckRadius, enemyLayer);

        if (pointBlankHits.Length > 0)
        {
            Collider targetCollider = pointBlankHits[0];
            Vector3 hitPoint = targetCollider.ClosestPoint(pointBlankCenter);
            Debug.Log("<color=magenta>¡Impacto a QUEMARROPA! Objeto: " + targetCollider.name + "</color>", targetCollider.gameObject);
            ProcessHit(targetCollider, hitPoint);
            GenerateTrail(hitPoint);
            FireBullet(muzzlePosition, hitPoint); // Disparar bala física
        }
        else
        {
            // --- FASE 2: DISPARO A DISTANCIA ---
            RaycastHit hit;
            Vector3 rayOrigin = muzzlePosition;
            float longRangeSphereRadius = 0.1f;
            float maxDistance = 100f;

            if (Physics.SphereCast(rayOrigin, longRangeSphereRadius, muzzleDirection, out hit, maxDistance, enemyLayer))
            {
                Debug.Log("<color=green>¡Impacto a DISTANCIA! Objeto: " + hit.collider.name + "</color>", hit.collider.gameObject);
                ProcessHit(hit.collider, hit.point);
                GenerateTrail(hit.point);
                FireBullet(muzzlePosition, hit.point); // Disparar bala física
            }
            else
            {
                // No se impactó nada
                Debug.Log("<color=red>Disparo al aire. Ninguna detección tuvo éxito.</color>");
                Vector3 endPoint = rayOrigin + muzzleDirection * maxDistance;
                GenerateTrail(endPoint);
                FireBullet(muzzlePosition, endPoint); // Disparar bala física al aire
            }
        }

        // El conteo de balas se reduce sin importar si se impactó o no.
        _countbullet--;
        if (_countbullet <= 0)
        {
            Debug.Log("Out of ammo!");
        }
    }

    // Gizmos (sin cambios)
    private void OnDrawGizmosSelected()
    {
        Transform muzzleTransform = null;
        if (_MuzzleFlashWeamon != null && _MuzzleFlashWeamon.root != null)
        {
            muzzleTransform = _MuzzleFlashWeamon.root.transform;
        }
        else
        {
            muzzleTransform = this.transform;
        }

        Gizmos.color = new Color(1, 0, 1, 0.5f);
        Vector3 pointBlankCenter = muzzleTransform.position + muzzleTransform.forward * pointBlankCheckDistance;
        Gizmos.DrawSphere(pointBlankCenter, pointBlankCheckRadius);
    }
}
