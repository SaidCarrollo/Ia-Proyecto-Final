using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AimIK.Properties;
using AimIK.Functions;
[ExecuteInEditMode]
public class IKManager : MonoBehaviour
{
    public Animator anim;
    public Transform target;
    public bool IKActive = false;
    public Transform LeftHand;
    public Transform baseArmWeapon;
    public Part[] IKPart;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.Update(0);
    }
    private void LateUpdate()
    {
        anim.Update(0);
        foreach (Part item in IKPart)
        {
            item.part.LookAt3D(target.position - item.positionOffset, item.rotationOffset);
            item.part.CheckClamp3D(item.limitRotation, item.GetRotation());
        }
        
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (IKActive)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            Quaternion rotLeftHand = Quaternion.Lerp(anim.GetIKRotation(AvatarIKGoal.LeftHand), LeftHand.rotation,Time.deltaTime*10f);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHand.rotation);



            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);



            Quaternion rot = Quaternion.Lerp(anim.GetIKRotation(AvatarIKGoal.RightHand), baseArmWeapon.rotation, Time.deltaTime * 10f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKRotation(AvatarIKGoal.RightHand, baseArmWeapon.rotation);

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, baseArmWeapon.position);


        }
        else
        {
            LeftHand.position = anim.GetIKPosition(AvatarIKGoal.LeftHand);
            LeftHand.rotation = anim.GetIKRotation(AvatarIKGoal.LeftHand);


        }
    }
    
    private void OnDrawGizmos()
    {

       
        if (LeftHand)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(LeftHand.position, 0.0251f);
        }

        if (target)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(target.position, 0.051f);
        }

    }

}
