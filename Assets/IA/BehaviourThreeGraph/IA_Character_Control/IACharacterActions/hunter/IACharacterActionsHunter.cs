using UnityEngine;

public class IACharacterActionsHunter : IACharacterActions
{
    [Header("Attack Logic")]
    // La referencia al arma se mueve aquí
    public WeaponBase weapon;

    // La máscara de capas para el ataque se mueve aquí
    public LayerMask enemyLayerMask;

    public override void LoadComponent()
    {
        base.LoadComponent();
        // Cargar el arma al iniciar
        if (weapon == null)
        {
            weapon = GetComponentInChildren<WeaponBase>();
        }
        if (weapon != null)
        {
            weapon.LoadComponent();
        }
    }

    public void AttackEnemy()
    {

        if (AIEye == null)
        {
            AIEye = GetComponent<IAEyeBase>();
            if (AIEye == null) return; 
        }
        if (agent == null)
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }


        if (AIEye.ViewEnemy == null || weapon == null) return;

        // La acción de atacar implica detenerse
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        // La acción de atacar implica mirar al objetivo
        LookEnemy();

        // Ejecuta el disparo
        weapon.Shoot(enemyLayerMask);
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