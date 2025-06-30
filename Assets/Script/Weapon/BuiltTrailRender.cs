using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public float speed = 75f; // Velocidad de la bala/rastro
    private Vector3 targetPosition;
    private TrailRenderer trailRenderer;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Usamos un método para inicializar el objetivo desde el arma
    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        // Empieza a moverse inmediatamente
        StartCoroutine(MoveTrail());
    }

    private System.Collections.IEnumerator MoveTrail()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float duration = distanceToTarget / speed;
        float elapsedTime = 0f;

        // Mientras no hayamos llegado al destino
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que termina exactamente en el punto final
        transform.position = targetPosition;

        // Desactivar la emisión y esperar a que el rastro desaparezca antes de destruir el objeto
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
            // Esperar el tiempo de vida del rastro + un pequeño margen
            Destroy(gameObject, trailRenderer.time + 0.1f);
        }
        else
        {
            Destroy(gameObject); // Si no hay trail, destruir inmediatamente
        }
    }
}