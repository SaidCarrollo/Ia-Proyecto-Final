using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AimIK.Properties;
using AimIK.Functions;
[ExecuteInEditMode]
public class IKFireBat : IKBase
{
    public Transform LeftHand;
    public Transform RightHand;
    
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
        if (IKActive)
        {
            foreach (Part item in IKPart)
            {
                item.part.LookAt3D(target.position - item.positionOffset, item.rotationOffset);
                item.part.CheckClamp3D(item.limitRotation, item.GetRotation());
            }
        }
        

    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (IKActive)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHand.rotation);

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);



            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKRotation(AvatarIKGoal.RightHand, RightHand.rotation);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, RightHand.position);


        }
        //else
        //{
        //    LeftHand.position = anim.GetIKPosition(AvatarIKGoal.LeftHand);
        //    LeftHand.rotation = anim.GetIKRotation(AvatarIKGoal.LeftHand);


        //}
    }
    private void OnDrawGizmos()
    {


        if (LeftHand)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(LeftHand.position, 0.0251f);
        }
        if (RightHand)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(RightHand.position, 0.0251f); 
            Gizmos.DrawSphere(RightHand.parent.position, 0.0351f);
        }
        if (target)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(target.position, 0.051f);
        }

    }

}
