using UnityEngine;

public class IACharacterActionsHunter : IACharacterActions
{
    [Header("Attack Logic")]
    // La referencia al arma se mueve aquí
    public WeaponsManager _WeaponsManager;

    // La máscara de capas para el ataque se mueve aquí
    public LayerMask enemyLayerMask;
    private void Awake()
    {
        LoadComponent();
    }
    public override void LoadComponent()
    {
        base.LoadComponent();
         
        _WeaponsManager = GetComponentInChildren<WeaponsManager>();
         
    }

    public void AttackEnemy()
    {
         
        if (AIEye.ViewEnemy == null) return;

        // La acción de atacar implica detenerse
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
         
        // Ejecuta el disparo
        _WeaponsManager.Fire( );
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
}