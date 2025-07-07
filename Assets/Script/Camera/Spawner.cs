using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public Camera cam;
    public GameObject[] prefabs;
    public float navMeshCheckRadius = 1.0f;

    private int currentPrefabIndex = 0;
    private bool deleteMode = false;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    [Header("UI")]
    public Image modeIcon; // arrastras aquí el objeto de imagen del Canvas
    public Sprite spawnIcon;  // icono modo instanciar
    public Sprite deleteIcon; // icono modo borrar

    void Start()
    {
        // al inicio, forzamos el modo spawn
        deleteMode = false;
        UpdateModeIcon();
    }

    void Update()
    {
        // alternar modo con B
        if (Input.GetKeyDown(KeyCode.B))
        {
            deleteMode = !deleteMode;
            Debug.Log(deleteMode ? "Modo BORRADO activado" : "Modo SPAWN activado");
            UpdateModeIcon();
        }

        if (Input.GetMouseButtonDown(0)) // click izquierdo
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (deleteMode)
                {
                    // borrar objeto
                    GameObject target = hit.collider.gameObject;
                    if (spawnedObjects.Contains(target))
                    {
                        spawnedObjects.Remove(target);
                        Destroy(target);
                    }
                }
                else
                {
                    // instanciar
                    if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, navMeshCheckRadius, NavMesh.AllAreas))
                    {
                        GameObject obj = Instantiate(prefabs[currentPrefabIndex], navHit.position, Quaternion.identity);
                        spawnedObjects.Add(obj);
                    }
                    else
                    {
                        Debug.Log("No hay NavMesh cerca del punto de clic.");
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ClearSpawnedObjects();
        }
    }

    private void UpdateModeIcon()
    {
        if (modeIcon != null)
        {
            modeIcon.sprite = deleteMode ? deleteIcon : spawnIcon;
        }
    }

    public void SelectPrefab(int index)
    {
        currentPrefabIndex = index;
    }

    public void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedObjects.Clear();
    }
}
