using UnityEngine;

public class IACharacterActionsAnimal: IACharacterActions
{
    public float FrameRate = 0;
    public float Rate = 1;
    public int damageAnimal;
    private void Start()
    {
        LoadComponent();
    }
    public override void LoadComponent()
    {
        base.LoadComponent();

    }
    public void Attack()
    {

        if (FrameRate > Rate)
        {
            FrameRate = 0;
            IAEyeAnimalAttack _IAEyeAnimalAttack = ((IAEyeAnimalAttack)AIEye);

            if (_IAEyeAnimalAttack != null &&
                _IAEyeAnimalAttack.ViewEnemy != null)
            {

                _IAEyeAnimalAttack.ViewEnemy.Damage(damageAnimal, health);
            }

        }
        FrameRate += Time.deltaTime;
    }
    public void ConsumeVisibleItem()
    {
        if (AIEye != null && AIEye.ViewItem != null)
        {
            // La lógica de consumir el ítem ahora vive aquí.
            AIEye.ViewItem.Consume(health);
        }
    }
}
