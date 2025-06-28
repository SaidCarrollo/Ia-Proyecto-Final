using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypeAgent { A, B, C, D, E }
public enum UnitGame
{
    Zombie,
    Soldier,
    Herbivore,
    Carnivore,
    Hunter,
    None
}
public class Health : MonoBehaviour
{
    [Header("imageUI")]
    public Image HealthBarLocal;

    [Header("CountHealth")]
    public int health;
    public int healthMax;

    public bool IsDead { get => (health <= 0); }

    [Header("AimOffSet")]
    public Transform AimOffset;
    public Health HurtingMe;

    [Header("Type Agent")]
    public TypeAgent typeAgent;
    [Header("Type List Agent Allies")]
    public List<TypeAgent> typeAgentAllies = new List<TypeAgent>();
    Coroutine HurtingMeroutine;

    public bool Importal = false;
    public UnitGame _UnitGame;
    public bool IsCantView=true;
    //[SerializeField] private GameObject itemDropOnDeathPrefab;

    private bool deathSequenceStarted = false;

    // --- Referencia al nuevo script ---
    private DeathHandler deathHandler;
    private void Awake()
    {
        // Obtenemos la referencia al DeathHandler. Es obligatorio que esté en el mismo objeto.
        deathHandler = GetComponent<DeathHandler>();
        if (deathHandler == null)
        {
            Debug.LogError("El componente DeathHandler no se encuentra en este GameObject. ¡Es necesario!", this);
        }
        LoadComponent();
    }
    
    IEnumerator HurtingMeActive(Health enemy)
    {
        HurtingMe = enemy;
        yield return new WaitForSeconds(3);
        HurtingMe = null;
        StopCoroutine(HurtingMeroutine);
    }

    public virtual void Damage(int damage, Health enemy)
    {
        if (Importal || deathSequenceStarted) return;

        if (!IsDead)
        {
            health -= damage;
            if (health < 0) health = 0;

            UpdateHealthBar();

            if (enemy != null)
            {
                if (HurtingMeroutine != null)
                {
                    StopCoroutine(HurtingMeroutine);
                }
                HurtingMeroutine = StartCoroutine(HurtingMeActive(enemy));
            }
        }

        // Si muere y la secuencia no ha comenzado...
        if (IsDead && !deathSequenceStarted)
        {
            deathSequenceStarted = true;
            // ...le decimos al DeathHandler que inicie la secuencia.
            deathHandler.StartDeathSequence();

            // Opcional: Desactivar este script para que no pueda recibir más daño.
            this.enabled = false;
        }
    }


    public void UpdateHealthBar()
    {
        if (HealthBarLocal != null)
        {
            float h = ((float)((float)health / (float)healthMax));
            HealthBarLocal.fillAmount = h;
        }
    }

    public virtual void LoadComponent()
    {
        health = healthMax;
        deathSequenceStarted = false;
        UpdateHealthBar();
        this.enabled = true;
    }


}
