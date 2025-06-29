using UnityEngine;

public class ItemBalas : Item
{
    private void Awake()
    {
        itemType = ItemType.Balas;
        // 'value' representa la cantidad de cartuchos a recargar
        if (value == 0) value = 2; // Añade 2 cartuchos por defecto
    }

    public override void Consume(Health consumer)
    {
        if (consumer == null)
        {
            Destroy(gameObject);
            return;
        }

        WeaponsManager weaponsManager = consumer.GetComponent<WeaponsManager>();
        if (weaponsManager != null)
        {
            weaponsManager.AddAmmo(value);
            Debug.Log($"{consumer.name} ha recogido {this.name} y recargado {value} cartuchos.");
        }
        else
        {
            Debug.LogWarning($"El personaje {consumer.name} intentó consumir munición pero no tiene un WeaponsManager.");
        }

        Destroy(gameObject);
    }
}