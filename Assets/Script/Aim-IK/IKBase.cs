using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AimIK.Properties;
using AimIK.Functions;
public class IKBase : MonoBehaviour
{
    protected Animator anim;
    public Transform target;
    public bool IKActive = false;
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
    

    

}
