using UnityEngine;

public class ItemCarne : Item
{
    private void Awake()
    {

        itemType = ItemType.Carne;
        if (value == 0) value = 25;
    }

    public override void Consume(Health consumer)
    {
        if (consumer != null)
        {
            consumer.Damage(-value, null);
            Debug.Log($"{consumer.name} ha consumido {this.name} y recuperado {value} de vida.");
        }

        Destroy(gameObject);
    }
}