using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Clase base para personajes controlados por IA que se comportan como "vehículos".
/// Gestiona la navegación, la detección de obstáculos y el movimiento general como deambular (wander).
/// NO contiene lógica de huida (flee), que es un comportamiento especializado.
/// </summary>
public class IACharacterVehiculo : IACharacterControl
{
    // --- Sistema de Evasión de Obstáculos ---
    // Componente que usa Raycasts y lógica difusa para detectar obstáculos y calcular una rotación para esquivarlos.
    protected CalculateDiffuse _CalculateDiffuse;
    // Velocidad de rotación calculada por el sistema difuso de evasión.
    protected float speedRotation = 0;

    // --- Lógica de Deambular (Wander) ---
    [Header("Configuración de Wander")]
    public float RangeWander; // Rango máximo para buscar un nuevo punto al que deambular.
    protected Vector3 positionWander; // El destino actual al que el personaje está deambulando.
    float FrameRate = 0; // Temporizador para controlar la frecuencia de cambio de destino.
    float Rate = 4; // Cada cuántos segundos se buscará un nuevo destino de wander.

    /// <summary>
    /// Carga los componentes necesarios y establece el estado inicial.
    /// Se llama al inicio de la vida del objeto.
    /// </summary>
    public override void LoadComponent()
    {
        base.LoadComponent();
        // Establece una posición inicial para deambular.
        positionWander = RandoWander(transform.position, RangeWander);
        // Obtiene la referencia al componente de evasión de obstáculos.
        _CalculateDiffuse = GetComponent<CalculateDiffuse>();
        if (AIEye == null) Debug.LogError("AIEye no está asignado en " + gameObject.name);
    }

    // --- Métodos de Rotación (Look) ---

    /// <summary>
    /// Gira suavemente para mirar hacia el enemigo detectado.
    /// </summary>
    public virtual void LookEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        Vector3 dir = (AIEye.ViewEnemy.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot.x = 0; // Bloquea la rotación en los ejes X y Z para mantener al personaje derecho.
        rot.z = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 50);
    }

    /// <summary>
    /// Gira suavemente para mirar hacia una posición específica en el mundo.
    /// </summary>
    public virtual void LookPosition(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot.x = 0;
        rot.z = 0;
        // Usa la velocidad de rotación del sistema de evasión si está activo, si no, una velocidad por defecto.
        float currentSpeedRotation = (speedRotation > 0) ? speedRotation : 10f;
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * currentSpeedRotation);
    }

    /// <summary>
    /// Activa la rotación para evadir un obstáculo si el componente CalculateDiffuse ha detectado una colisión.
    /// </summary>
    public virtual void LookRotationCollider()
    {
        if (_CalculateDiffuse != null && _CalculateDiffuse.Collider)
        {
            speedRotation = _CalculateDiffuse.speedRotation;
            // Calcula un punto de destino basándose en la normal del impacto para esquivar el obstáculo.
            Vector3 posNormal = _CalculateDiffuse.hit.point + _CalculateDiffuse.hit.normal * 2;
            LookPosition(posNormal);
        }
    }

    // --- Métodos de Movimiento (Move) ---

    /// <summary>
    /// Establece el destino del NavMeshAgent para moverse a una posición.
    /// </summary>
    public virtual void MoveToPosition(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(pos);
        }
    }

    /// <summary>
    /// Mueve al personaje hacia el enemigo detectado.
    /// </summary>
    public virtual void MoveToEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        MoveToPosition(AIEye.ViewEnemy.transform.position);
    }

    /// <summary>
    /// Mueve al personaje hacia el aliado detectado.
    /// </summary>
    public virtual void MoveToAllied()
    {
        if (AIEye.ViewAllie == null) return;
        MoveToPosition(AIEye.ViewAllie.transform.position);
    }

    /// <summary>
    /// Se mueve en la dirección opuesta al enemigo para evadirlo.
    /// </summary>
    public virtual void MoveToEvadEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        Vector3 dir = (transform.position - AIEye.ViewEnemy.transform.position).normalized;
        Vector3 newPosition = transform.position + dir * 5f;
        MoveToPosition(newPosition);
    }

    /// <summary>
    /// Encuentra una posición aleatoria válida en el NavMesh dentro de un rango determinado.
    /// </summary>
    protected Vector3 RandoWander(Vector3 position, float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection.y = 0; // Mantener en el plano horizontal
        Vector3 targetPosition = position + randomDirection;
        NavMeshHit hit;
        // Intenta encontrar una posición válida en el NavMesh varias veces.
        for (int i = 0; i < 30; i++)
        {
            if (NavMesh.SamplePosition(targetPosition, out hit, range, NavMesh.AllAreas))
            {
                return hit.position; // Devuelve la posición válida encontrada.
            }
        }
        return position; // Si falla, regresa la posición original.
    }

    /// <summary>
    /// Gestiona el comportamiento de deambular (wander).
    /// </summary>
    public virtual void MoveToWander()
    {
        // No deambula si hay un enemigo a la vista.
        if (AIEye.ViewEnemy != null) return;

        float distance = (transform.position - positionWander).magnitude;
        // Si llega cerca del destino, o si ha pasado suficiente tiempo, busca un nuevo destino.
        if (distance < 2 || FrameRate > Rate)
        {
            FrameRate = 0;
            positionWander = RandoWander(transform.position, RangeWander);
        }
        FrameRate += Time.deltaTime;

        MoveToPosition(positionWander);
    }
}