using UnityEngine;
using BehaviorDesigner.Runtime.Tasks; // Necesario para TaskStatus

/// <summary>
/// Clase especializada para NPCs de tipo Animal.
/// Hereda todo el comportamiento base de IACharacterVehiculo y añade la lógica
/// de huida (flee) basada en un sistema de lógica difusa.
/// </summary>
public class IACharacterVehiculoAnimal : IACharacterVehiculo
{
    // --- Lógica de Huida (Flee) con Sistema Difuso ---
    [Header("Configuración de Huida")]
    //("Multiplicador para la distancia de huida.")]
    [SerializeField] protected float fleeMultiplier = 2f;

    // El sistema de lógica difusa que calcula qué tan fuerte debe ser el deseo de huir.
    private FleeFuzzySystem _fleeFuzzySystem;
    // Contiene las curvas (AnimationCurve) que definen los conjuntos difusos (vida baja, media, alta).
    private FleeFuzzyConfig _fleeConfig;

    // Umbrales para tomar la decisión de huir basados en la salida del sistema difuso.
    private const float CARNIVORE_FLEE_THRESHOLD = 0.5f;
    private const float CARNIVORE_STOP_FLEE_THRESHOLD = 0.45f;
    private const float HERBIVORE_FLEE_THRESHOLD = 0.4f;

    // Propiedades para gestionar el estado de huida actual.
    public bool IsCurrentlyFleeing { get; private set; } = false;
    public Vector3 CurrentFleeDestination { get; private set; }

    void Start()
    {
        // Llama al método de carga de componentes.
        this.LoadComponent();
    }

    /// <summary>
    /// Sobrescribe el LoadComponent base para añadir la inicialización del sistema de huida.
    /// </summary>
    public override void LoadComponent()
    {
        // Llama al método de la clase base para inicializar componentes comunes (NavMeshAgent, AIEye, etc.).
        base.LoadComponent();
        // Inicializa el sistema de lógica difusa para la huida, que es específico de los animales.
        InitializeFleeSystem();
    }

    /// <summary>
    /// Carga la configuración del FleeFuzzyConfig y crea el sistema de lógica difusa.
    /// </summary>
    private void InitializeFleeSystem()
    {
        // Obtiene el componente con las curvas de configuración.
        _fleeConfig = GetComponent<FleeFuzzyConfig>();
        if (_fleeConfig == null)
        {
            Debug.LogError("El componente FleeFuzzyConfig no se encontró en el Agente.", gameObject);
            return;
        }

        // Selecciona las curvas correctas según si el animal es carnívoro o herbívoro.
        FleeCurves curvesToUse = (health._UnitGame == UnitGame.Carnivore) ?
            _fleeConfig.CarnivoreFleeCurves :
            _fleeConfig.HerbivoreFleeCurves;

        // Crea una nueva instancia del sistema difuso con las curvas seleccionadas.
        _fleeFuzzySystem = new FleeFuzzySystem(
            curvesToUse.VeryLowHealthCurve,
            curvesToUse.LowHealthCurve,
            curvesToUse.ModerateHealthCurve,
            curvesToUse.HighHealthCurve
        );
    }

    /// <summary>
    /// Método principal que evalúa si el personaje debe huir.
    /// Es llamado generalmente desde un árbol de comportamiento (Behavior Tree).
    /// </summary>
    /// <returns>El estado de la tarea (Running, Success, Failure).</returns>
    public TaskStatus EvaluateAndExecuteFlee()
    {
        if (_fleeFuzzySystem == null || health.IsDead)
        {
            if (IsCurrentlyFleeing) ConcludeFleeState();
            return TaskStatus.Failure; // Falla si no hay sistema o está muerto.
        }

        // 1. Pasa los datos de entrada (vida actual) al sistema difuso.
        _fleeFuzzySystem.CurrentHealth = health.health;
        _fleeFuzzySystem.MaxHealth = health.healthMax;
        // 2. El sistema difuso calcula la "fuerza de huida".
        _fleeFuzzySystem.CalculateFleeDecision(health._UnitGame);
        float currentFleeStrength = _fleeFuzzySystem.FleeDecisionStrength;

        // 3. Lógica para detener la huida si un carnívoro se siente más seguro.
        if (IsCurrentlyFleeing && health._UnitGame == UnitGame.Carnivore)
        {
            if (currentFleeStrength < CARNIVORE_STOP_FLEE_THRESHOLD)
            {
                ConcludeFleeState();
                return TaskStatus.Failure; // La huida se interrumpe.
            }
        }

        // 4. Lógica para decidir si se debe empezar a huir.
        if (!IsCurrentlyFleeing)
        {
            bool shouldStartFleeing = false;
            if (health._UnitGame == UnitGame.Carnivore && currentFleeStrength > CARNIVORE_FLEE_THRESHOLD)
            {
                shouldStartFleeing = true;
            }
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
                return TaskStatus.Failure; // No se cumplen las condiciones para huir.
            }
        }

        // 5. Si ya está huyendo, se comprueba si ha llegado al destino.
        if (IsCurrentlyFleeing)
        {
            if (HasReachedFleeDestination())
            {
                ConcludeFleeState();
                return TaskStatus.Success; // Tarea completada con éxito.
            }
            return TaskStatus.Running; // Todavía está en camino.
        }

        return TaskStatus.Failure; // Estado por defecto.
    }

    /// <summary>
    /// Calcula una dirección de escape segura, idealmente opuesta al enemigo
    /// y combinada con una dirección aleatoria para evitar ser predecible.
    /// </summary>
    public void FleeRandomDirection()
    {
        Vector3 fleePositionBase;
        if (AIEye.ViewEnemy != null)
        {
            // Busca un punto aleatorio y lo combina con la dirección opuesta al enemigo.
            Vector3 fleePosition = RandoWander(transform.position, RangeWander * fleeMultiplier);
            Vector3 enemyDirection = (transform.position - AIEye.ViewEnemy.transform.position).normalized;
            Vector3 randomDirection = (fleePosition - transform.position).normalized;
            Vector3 finalDirection = (enemyDirection + randomDirection).normalized;
            fleePositionBase = transform.position + finalDirection * (RangeWander * fleeMultiplier);
        }
        else
        {
            // Si no hay enemigo, simplemente huye a un punto aleatorio.
            fleePositionBase = RandoWander(transform.position, RangeWander * fleeMultiplier);
        }
        MoveToPosition(fleePositionBase);
    }

    /// <summary>
    /// Inicia el estado de huida: establece el destino y empieza a moverse.
    /// </summary>
    public void InitiateFleeState()
    {
        if (agent == null || !agent.isOnNavMesh) return;
        IsCurrentlyFleeing = true;
        FleeRandomDirection(); // Calcula a dónde huir.
        CurrentFleeDestination = agent.destination;
        LookPosition(CurrentFleeDestination); // Mira hacia el destino.
    }

    /// <summary>
    /// Termina el estado de huida: detiene el movimiento del agente.
    /// </summary>
    public void ConcludeFleeState()
    {
        if (agent == null) return;
        IsCurrentlyFleeing = false;
        if (agent.isOnNavMesh && agent.hasPath)
        {
            agent.ResetPath(); // Limpia la ruta actual del NavMeshAgent.
        }
    }

    /// <summary>
    /// Comprueba si el agente ha llegado a su destino de huida.
    /// </summary>
    public bool HasReachedFleeDestination()
    {
        if (!IsCurrentlyFleeing || agent == null || !agent.isOnNavMesh)
        {
            return false;
        }

        // Comprueba si el agente está cerca del destino y ha dejado de moverse.
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