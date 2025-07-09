using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class Spawner : MonoBehaviour
{
    public Camera cam;
    public GameObject[] prefabs;
    public float navMeshCheckRadius = 1.0f;

    private int currentPrefabIndex = 0;
    private bool deleteMode = false;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    [Header("UI")]
    public Image modeIcon;
    public Sprite spawnIcon;
    public Sprite deleteIcon;

    [Header("Escalas de los prefabs")]
    [SerializeField]
    private Vector3[] targetScales = new Vector3[]
    {
        new Vector3(3.1095f, 3.1095f, 3.1095f),     // Prefab 0
        new Vector3(6.04218f, 6.04218f, 6.04218f),  // Prefab 1
        new Vector3(1f, 1f, 1f)                    // Prefab 2
    };

    void Start()
    {
        deleteMode = false;
        UpdateModeIcon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            deleteMode = !deleteMode;
            Debug.Log(deleteMode ? "Modo BORRADO activado" : "Modo SPAWN activado");
            UpdateModeIcon();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (deleteMode)
                {
                    GameObject target = hit.collider.gameObject;
                    if (spawnedObjects.Contains(target))
                    {
                        spawnedObjects.Remove(target);
                        Destroy(target);
                    }
                }
                else
                {
                    if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, navMeshCheckRadius, NavMesh.AllAreas))
                    {
                        GameObject prefabToSpawn = prefabs[currentPrefabIndex];
                        Vector3 finalScale = prefabToSpawn.transform.localScale;

                        GameObject obj = Instantiate(prefabToSpawn, navHit.position, Quaternion.identity);
                        obj.transform.localScale = Vector3.zero;

                        obj.transform.DOScale(finalScale, 0.3f).SetEase(Ease.OutBack);

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


