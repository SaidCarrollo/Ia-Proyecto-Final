using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime.Tasks; // Asegúrate de tener esta dependencia para TaskStatus

public class IACharacterVehiculo : IACharacterControl
{
    protected CalculateDiffuse _CalculateDiffuse;
    protected float speedRotation = 0;

    [Header("Configuración de Huida")]
    [SerializeField] protected float fleeMultiplier = 2f;

    // --- Lógica de huida integrada en este script ---
    private FleeFuzzySystem _fleeFuzzySystem;
    private FleeFuzzyConfig _fleeConfig; // Componente que debe estar en el mismo GameObject
    private const float CARNIVORE_FLEE_THRESHOLD = 0.5f;
    private const float CARNIVORE_STOP_FLEE_THRESHOLD = 0.45f;
    private const float HERBIVORE_FLEE_THRESHOLD = 0.4f;
    // -----------------------------------------------

    public float RangeWander;
    protected Vector3 positionWander;
    float FrameRate = 0;
    float Rate = 4;

    // Propiedades para gestionar el estado de huida
    public bool IsCurrentlyFleeing { get; private set; } = false;
    public Vector3 CurrentFleeDestination { get; private set; }

    public override void LoadComponent()
    {
        base.LoadComponent();
        positionWander = RandoWander(transform.position, RangeWander);
        _CalculateDiffuse = GetComponent<CalculateDiffuse>();
        if (AIEye == null) Debug.LogError("AIEye no está asignado en " + gameObject.name);

        // Inicializa el sistema de huida directamente en el vehículo.
        InitializeFleeSystem();
    }

    /// <summary>
    /// Carga la configuración y inicializa el sistema de lógica difusa para la huida.
    /// </summary>
    private void InitializeFleeSystem()
    {
        _fleeConfig = GetComponent<FleeFuzzyConfig>();
        if (_fleeConfig == null)
        {
            Debug.LogError("El componente FleeFuzzyConfig no se encontró en el Agente.", gameObject);
            return;
        }

        // Selecciona las curvas de animación correctas según el tipo de unidad
        // CORRECCIÓN 1: Se accede a _UnitGame a través de la variable 'health'.
        FleeCurves curvesToUse = (health._UnitGame == UnitGame.Carnivore) ?
            _fleeConfig.CarnivoreFleeCurves :
            _fleeConfig.HerbivoreFleeCurves;

        // Inicializa el sistema difuso
        _fleeFuzzySystem = new FleeFuzzySystem(
            curvesToUse.VeryLowHealthCurve,
            curvesToUse.LowHealthCurve,
            curvesToUse.ModerateHealthCurve,
            curvesToUse.HighHealthCurve
        );
    }

    /// <summary>
    /// Método principal llamado por el nodo ActionFlee.
    /// Evalúa y ejecuta la lógica de huida, retornando el estado de la tarea.
    /// </summary>
    /// <returns>El estado actual de la tarea (Running, Success, o Failure).</returns>
    public TaskStatus EvaluateAndExecuteFlee()
    {
        if (_fleeFuzzySystem == null || health.IsDead)
        {
            if (IsCurrentlyFleeing) ConcludeFleeState();
            return TaskStatus.Failure;
        }

        // 1. Calcular la "fuerza" de la decisión de huir
        _fleeFuzzySystem.CurrentHealth = health.health;
        _fleeFuzzySystem.MaxHealth = health.healthMax;
        // CORRECCIÓN 2: Se pasa el tipo de unidad desde el componente 'health'.
        _fleeFuzzySystem.CalculateFleeDecision(health._UnitGame);
        float currentFleeStrength = _fleeFuzzySystem.FleeDecisionStrength;

        // 2. Lógica para que un carnívoro deje de huir si se siente más seguro
        // CORRECCIÓN 3: Se comprueba el tipo de unidad desde el componente 'health'.
        if (IsCurrentlyFleeing && health._UnitGame == UnitGame.Carnivore)
        {
            if (currentFleeStrength < CARNIVORE_STOP_FLEE_THRESHOLD)
            {
                ConcludeFleeState();
                return TaskStatus.Failure; // La huida se interrumpe, la tarea falla.
            }
        }

        // 3. Lógica para decidir si se debe empezar a huir
        if (!IsCurrentlyFleeing)
        {
            bool shouldStartFleeing = false;
            // CORRECCIÓN 4: Se comprueba el tipo de unidad desde el componente 'health'.
            if (health._UnitGame == UnitGame.Carnivore && currentFleeStrength > CARNIVORE_FLEE_THRESHOLD)
            {
                shouldStartFleeing = true;
            }
            // CORRECCIÓN 5: Se comprueba el tipo de unidad desde el componente 'health'.
            else if (health._UnitGame == UnitGame.Herbivore && currentFleeStrength > HERBIVORE_FLEE_THRESHOLD)
            {
                shouldStartFleeing = true;
            }

            if (shouldStartFleeing)
            {
                InitiateFleeState();
            }
            else
            {
                // No se cumplen las condiciones para huir, la tarea falla.
                return TaskStatus.Failure;
            }
        }

        // 4. Si ya está huyendo, monitorear el progreso
        if (IsCurrentlyFleeing)
        {
            if (HasReachedFleeDestination())
            {
                ConcludeFleeState();
                return TaskStatus.Success; // Llegó al destino, tarea completada.
            }
            return TaskStatus.Running; // Todavía está en camino.
        }

        return TaskStatus.Failure; // Estado por defecto si ninguna condición se cumple.
    }

    public virtual void LookEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        Vector3 dir = (AIEye.ViewEnemy.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 50);
    }

    public virtual void LookPosition(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot.x = 0;
        rot.z = 0;
        float currentSpeedRotation = (speedRotation > 0) ? speedRotation : 10f;
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * currentSpeedRotation);
    }

    public virtual void LookRotationCollider()
    {
        if (_CalculateDiffuse != null && _CalculateDiffuse.Collider)
        {
            speedRotation = _CalculateDiffuse.speedRotation;
            Vector3 posNormal = _CalculateDiffuse.hit.point + _CalculateDiffuse.hit.normal * 2;
            LookPosition(posNormal);
        }
    }

    public virtual void MoveToPosition(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(pos);
        }
    }

    public virtual void MoveToEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        MoveToPosition(AIEye.ViewEnemy.transform.position);
    }

    public virtual void MoveToAllied()
    {
        if (AIEye.ViewAllie == null) return;
        MoveToPosition(AIEye.ViewAllie.transform.position);
    }

    public virtual void MoveToEvadEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        Vector3 dir = (transform.position - AIEye.ViewEnemy.transform.position).normalized;
        Vector3 newPosition = transform.position + dir * 5f;
        MoveToPosition(newPosition);
    }

    Vector3 RandoWander(Vector3 position, float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection.y = 0; // Mantener en el plano horizontal
        Vector3 targetPosition = position + randomDirection;
        NavMeshHit hit;
        for (int i = 0; i < 30; i++)
        {
            if (NavMesh.SamplePosition(targetPosition, out hit, range, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return position; // Si falla, regresa la posición original
    }

    public virtual void FleeRandomDirection()
    {
        Vector3 fleePositionBase;
        if (AIEye.ViewEnemy != null)
        {
            Vector3 fleePosition = RandoWander(transform.position, RangeWander * fleeMultiplier);
            Vector3 enemyDirection = (transform.position - AIEye.ViewEnemy.transform.position).normalized;
            Vector3 randomDirection = (fleePosition - transform.position).normalized;
            Vector3 finalDirection = (enemyDirection + randomDirection).normalized;
            fleePositionBase = transform.position + finalDirection * (RangeWander * fleeMultiplier);
        }
        else
        {
            fleePositionBase = RandoWander(transform.position, RangeWander * fleeMultiplier);
        }
        MoveToPosition(fleePositionBase);
    }

    public virtual void MoveToWander()
    {
        if (AIEye.ViewEnemy != null) return;

        float distance = (transform.position - positionWander).magnitude;
        if (distance < 2)
        {
            positionWander = RandoWander(transform.position, RangeWander);
        }

        if (FrameRate > Rate)
        {
            FrameRate = 0;
            positionWander = RandoWander(transform.position, RangeWander);
        }
        FrameRate += Time.deltaTime;

        MoveToPosition(positionWander);
    }

    public virtual void InitiateFleeState()
    {
        if (agent == null || !agent.isOnNavMesh) return;
        IsCurrentlyFleeing = true;
        FleeRandomDirection();
        CurrentFleeDestination = agent.destination;
        LookPosition(CurrentFleeDestination);
    }

    public virtual void ConcludeFleeState()
    {
        if (agent == null) return;
        IsCurrentlyFleeing = false;
        if (agent.isOnNavMesh && agent.hasPath)
        {
            agent.ResetPath();
        }
    }

    public virtual bool HasReachedFleeDestination()
    {
        if (!IsCurrentlyFleeing || agent == null || !agent.isOnNavMesh)
        {
            return false;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }
        return false;
    }
}