using UnityEngine;

public class HunterViewEnemyIK : MonoBehaviour
{
    IKMarine IK;
    IAEyeHunterShootAttack eye;
    healthHunter health;
    public Transform aim;
    Vector3 storeposition;
    public float lenght;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IK=GetComponent<IKMarine>();
         
        eye = GetComponent<IAEyeHunterShootAttack>();

        storeposition = aim.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (eye.ViewEnemy != null)
        {
            IK.target = eye.ViewEnemy.AimOffset;
        }
        else
        {
            IK.target = aim;
        }
    }
}
