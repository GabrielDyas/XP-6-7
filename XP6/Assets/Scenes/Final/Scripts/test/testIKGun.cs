using UnityEngine;

public class testIKGun : MonoBehaviour
{
    [Header("Targets da arma")]
    [SerializeField] private Transform targetArmaDireita;
    [SerializeField] private Transform targetArmaEsquerda;

    [Header("Targets do Two Bone IK")]
    [SerializeField] private Transform targetIKDireita;
    [SerializeField] private Transform targetIKEsquerda;

    private void LateUpdate()
    {
        // Mão direita
        if (targetArmaDireita != null && targetIKDireita != null)
        {
            targetIKDireita.SetPositionAndRotation(
                targetArmaDireita.position,
                targetArmaDireita.rotation);
        }

        // Mão esquerda
        if (targetArmaEsquerda != null && targetIKEsquerda != null)
        {
            targetIKEsquerda.SetPositionAndRotation(
                targetArmaEsquerda.position,
                targetArmaEsquerda.rotation);
        }
    }
}