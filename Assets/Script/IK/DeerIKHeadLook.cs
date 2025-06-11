using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DeerNeckIKEditorWithLimits : MonoBehaviour
{
    [System.Serializable]
    public class BoneSettings
    {
        public Transform bone;

        public Vector3 positionOffset;

        public Vector3 rotationOffset;

        public Vector3 rotationOffsetMin = new Vector3(-30f, -30f, -30f);
        public Vector3 rotationOffsetMax = new Vector3(30f, 30f, 30f);

        [Range(0f, 1f)] public float weight = 1f;

        public Vector3 GetClampedRotationOffset()
        {
            return new Vector3(
                Mathf.Clamp(rotationOffset.x, rotationOffsetMin.x, rotationOffsetMax.x),
                Mathf.Clamp(rotationOffset.y, rotationOffsetMin.y, rotationOffsetMax.y),
                Mathf.Clamp(rotationOffset.z, rotationOffsetMin.z, rotationOffsetMax.z)
            );
        }
    }

    public bool enableLookIK = true; // <-- Variable para activar/desactivar

    public Transform lookTarget;
    public List<BoneSettings> bones = new List<BoneSettings>();
    Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!enableLookIK) return;  // Salimos si está desactivado

        if (lookTarget == null || bones.Count == 0) return;
        //if (animator != null) animator.enabled = false;
        foreach (var b in bones)
        {
            if (b.bone == null) continue;

            Vector3 targetPos = lookTarget.position + b.positionOffset;
            Vector3 dirToTarget = targetPos - b.bone.position;

            Quaternion lookRot = Quaternion.LookRotation(dirToTarget, Vector3.up);

            Vector3 clampedRotOffset = b.GetClampedRotationOffset();
            Quaternion offsetRot = Quaternion.Euler(clampedRotOffset);

            Quaternion finalRot = lookRot * offsetRot;

            b.bone.rotation = Quaternion.Slerp(b.bone.rotation, finalRot, b.weight);
        }
    }

    void OnDrawGizmos()
    {
        if (!enableLookIK) return; // No dibujar si está desactivado

        if (lookTarget == null) return;

        Gizmos.color = Color.green;
        foreach (var b in bones)
        {
            if (b.bone != null)
            {
                Gizmos.DrawLine(b.bone.position, lookTarget.position + b.positionOffset);
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lookTarget.position, 0.05f);
    }
}
