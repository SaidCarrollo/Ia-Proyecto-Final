using UnityEngine;

public class ItemPlanta : Item
{
    // --- INICIO DE LAS MODIFICACIONES ---

    /// <summary>
    /// Referencia al Spawner que creó esta planta.
    /// Se usa para notificarle cuando la planta es consumida.
    /// </summary>
    public PlantSpawner Spawner { get; set; }

    /// <summary>
    /// Referencia al punto de aparición original de esta planta.
    /// </summary>
    public Transform OriginPoint { get; set; }

    // --- FIN DE LAS MODIFICACIONES ---

    private void Awake()
    {
        itemType = ItemType.Planta;
        if (value == 0) value = 20;
    }

    public override void Consume(Health consumer)
    {
        if (consumer != null)
        {
            consumer.Damage(-value, null);
            Debug.Log($"{consumer.name} ha consumido {this.name} y recuperado {value} de vida.");
        }


        // Antes de destruirse, comprueba si fue creado por un spawner.
        if (Spawner != null)
        {
            // Si es así, le pide al spawner que inicie el contador para reaparecer.
            Spawner.ScheduleRespawn(OriginPoint);
        }

        // --- FIN DE LAS MODIFICACIONES ---

        Destroy(gameObject);
    }
}