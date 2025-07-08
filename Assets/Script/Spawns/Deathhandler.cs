using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NavMeshAgent))]
public class DeathHandler : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private GameObject itemDropOnDeathPrefab;
    [SerializeField] private ParticleSystem continuousDeathParticles;
    [SerializeField] private float deathRotationDuration = 0.5f;
    [SerializeField] private float delayBeforeDestroy = 3f;

    // --- Referencias ---
    private Renderer objectRenderer;
    private Health healthScript;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        healthScript = GetComponent<Health>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void StartDeathSequence()
    {
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        // 1. Detener movimiento
        if (navMeshAgent != null)
            navMeshAgent.enabled = false;

        // 2. Soltar ítem
        if (itemDropOnDeathPrefab != null)
            Instantiate(itemDropOnDeathPrefab, transform.position, Quaternion.identity);

        // 3. Rotación de muerte
        float elapsedTime = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = transform.rotation * Quaternion.Euler(0, 0, 180f);

        while (elapsedTime < deathRotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsedTime / deathRotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRot;

        // 4. Cambiar color a rojo y ocultar mesh
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.red;
            objectRenderer.enabled = false;
        }

        foreach (var col in GetComponents<Collider>())
            col.enabled = false;

        // 5. Activar partículas
        if (continuousDeathParticles != null)
            continuousDeathParticles.Play();

        // 6. Esperar efectos
        yield return new WaitForSeconds(delayBeforeDestroy);

        // 7. Detener partículas (emisión), luego destruir GameObject
        if (continuousDeathParticles != null)
            continuousDeathParticles.Stop();

        Destroy(gameObject);
    }
}
