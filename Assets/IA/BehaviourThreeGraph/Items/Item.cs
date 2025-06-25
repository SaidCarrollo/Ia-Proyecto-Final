using UnityEngine;

public enum ItemType
{
    Carne,
    Planta,
    Agua
}

public abstract class Item : MonoBehaviour
{
    [Header("Configuración del Ítem")]
    public ItemType itemType;
    public int value; 

    public abstract void Consume(Health consumer);

    private void Start()
    {
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError($"El ítem '{gameObject.name}' no tiene un Collider. No podrá ser detectado.");
        }
    }
}