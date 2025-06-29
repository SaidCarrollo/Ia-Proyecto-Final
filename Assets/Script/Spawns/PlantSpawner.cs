using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    [Header("Configuración del Prefab")]
    [Tooltip("El Prefab del ítem 'Planta' que se va a instanciar.")]
    public GameObject plantPrefab;

    [Header("Puntos de Aparición")]
    [Tooltip("Una lista de todos los lugares donde pueden aparecer las plantas.")]
    public List<Transform> spawnPoints;

    [Header("Tiempo de Reaparición")]
    [Tooltip("El tiempo en segundos que tarda una planta en reaparecer después de ser consumida.")]
    public float respawnTime = 15.0f;

    void Start()
    {
        // Al empezar, nos aseguramos de que el prefab y la lista no estén vacíos.
        if (plantPrefab == null)
        {
            Debug.LogError("¡Falta el Prefab de la planta en el PlantSpawner!", this);
            return;
        }
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No se han asignado puntos de aparición en el PlantSpawner.", this);
            return;
        }

        // Hacemos que aparezca una planta en cada punto de la lista al iniciar.
        foreach (Transform point in spawnPoints)
        {
            SpawnPlantAt(point);
        }
    }

    private void SpawnPlantAt(Transform spawnPoint)
    {
        // Creamos la planta en la posición y rotación del punto de aparición.
        GameObject plantInstance = Instantiate(plantPrefab, spawnPoint.position, spawnPoint.rotation);

        // Obtenemos el script del ítem para 'registrarlo'.
        ItemPlanta plantScript = plantInstance.GetComponent<ItemPlanta>();
        if (plantScript != null)
        {
            // Le damos al script de la planta una referencia a este spawner y a su punto de origen.
            // Así sabrá a quién notificar cuando sea consumida.
            plantScript.Spawner = this;
            plantScript.OriginPoint = spawnPoint;
        }
    }


    public void ScheduleRespawn(Transform spawnPoint)
    {
        // Usamos una Coroutine para esperar el tiempo de reaparición sin bloquear el juego.
        StartCoroutine(RespawnCoroutine(spawnPoint));
    }

    private IEnumerator RespawnCoroutine(Transform spawnPoint)
    {
        // Espera la cantidad de segundos definida en respawnTime.
        yield return new WaitForSeconds(respawnTime);

        // Una vez transcurrido el tiempo, crea una nueva planta en el mismo lugar.
        SpawnPlantAt(spawnPoint);
    }
}