using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Configuraci�n")]
    public GameObject impactEffect;

    private Rigidbody rb;
    private float lifetime;
    private float timer = 0f;

    /// <summary>
    /// Inicializa la bala con direcci�n y velocidad
    /// </summary>
    public void Initialize(Vector3 direction, float speed, float lifeTime)
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = direction * speed;
        lifetime = lifeTime;

        // Configurar para destrucci�n autom�tica
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Evitar colisiones con el jugador y otras balas
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bullet"))
            return;

        // Crear efecto de impacto si est� asignado
        if (impactEffect != null)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.LookRotation(contact.normal);
            Instantiate(impactEffect, contact.point, rotation);
        }

        // Destruir la bala
        Destroy(gameObject);
    }
}