using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Camera cam;
    public GameObject[] prefabs; // asignas los prefabs en el inspector
    int currentPrefabIndex = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click izquierdo
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Instantiate(prefabs[currentPrefabIndex], hit.point, Quaternion.identity);
            }
        }
    }

    public void SelectPrefab(int index)
    {
        currentPrefabIndex = index;
    }
}
