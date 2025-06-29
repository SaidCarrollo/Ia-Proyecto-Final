using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NUEVO: Necesario para controlar el NavMeshAgent

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NavMeshAgent))] // NUEVO: Asegura que siempre haya un NavMeshAgent
public class DeathHandler : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private GameObject itemDropOnDeathPrefab;

    // CAMBIO: Ya no es un prefab, sino una referencia directa a las partículas del personaje.
    [Tooltip("Las partículas que se reproducirán EN BUCLE durante la muerte. Deben ser un componente hijo del personaje.")]
    [SerializeField] private ParticleSystem continuousDeathParticles;

    [SerializeField] private float deathRotationDuration = 0.5f;

    [Header("Respawn Settings")]
    [SerializeField] private List<Transform> respawnPoints;

    // --- Referencias a componentes ---
    private Renderer objectRenderer;
    private Health healthScript;
    private NavMeshAgent navMeshAgent; // NUEVO: Referencia al agente de navegación

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        healthScript = GetComponent<Health>();
        navMeshAgent = GetComponent<NavMeshAgent>(); // NUEVO: Obtenemos el componente NavMeshAgent
    }

    public void StartDeathSequence()
    {
        StartCoroutine(DeathAndRespawnCoroutine());
    }

    private IEnumerator DeathAndRespawnCoroutine()
    {
        // --- 1. DESACTIVAR COMPONENTES ---
        // EXPLICACIÓN: Lo primero es detener el movimiento del personaje.
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        // --- 2. ANIMACIÓN DE MUERTE ---
        if (itemDropOnDeathPrefab != null)
        {
            Instantiate(itemDropOnDeathPrefab, transform.position, Quaternion.identity);
        }

        float elapsedTime = 0f;
        Quaternion startingRotation = transform.rotation;
        Quaternion finalRotation = transform.rotation * Quaternion.Euler(0, 0, 180f);

        while (elapsedTime < deathRotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, finalRotation, elapsedTime / deathRotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = finalRotation;

        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.red;
        }

        // --- 3. ACTIVAR PARTÍCULAS EN BUCLE ---
        // CAMBIO: En lugar de instanciar, ahora reproducimos las partículas existentes.
        if (continuousDeathParticles != null)
        {
            continuousDeathParticles.Play();
        }

        // Ocultamos el mesh del objeto, pero las partículas seguirán visibles.
        if (objectRenderer != null)
        {
            objectRenderer.enabled = false;
        }
        foreach (var collider in GetComponents<Collider>())
        {
            collider.enabled = false;
        }

        // --- 4. ESPERAR ANTES DE REAPARECER ---
        yield return new WaitForSeconds(3f); // Aumenté el tiempo para que las partículas se vean

        // --- 5. DETENER PARTÍCULAS Y PREPARAR RESPAWN ---
        // CAMBIO: Detenemos la emisión de nuevas partículas. Las existentes desaparecerán suavemente.
        if (continuousDeathParticles != null)
        {
            continuousDeathParticles.Stop();
        }

        // --- 6. LÓGICA DE RESPAWN ---
        if (respawnPoints != null && respawnPoints.Count > 0)
        {
            Transform spawnPoint = respawnPoints[Random.Range(0, respawnPoints.Count)];

            // NUEVO: Para teletransportar un NavMeshAgent, es mejor usar Warp() mientras está activo.
            // Primero lo reactivamos.
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
                // Warp teletransporta el agente de forma segura sin calcular una ruta.
                navMeshAgent.Warp(spawnPoint.position);
            }
            else // Si no hay NavMeshAgent, movemos el transform directamente
            {
                transform.position = spawnPoint.position;
            }
            transform.rotation = spawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("No se han asignado puntos de respawn.", this);
        }

        // --- 7. RESTAURAR ESTADO ---
        if (objectRenderer != null)
        {
            objectRenderer.enabled = true;
            objectRenderer.material.color = Color.white;
        }
        foreach (var collider in GetComponents<Collider>())
        {
            collider.enabled = true;
        }

        healthScript.LoadComponent();
    }
}