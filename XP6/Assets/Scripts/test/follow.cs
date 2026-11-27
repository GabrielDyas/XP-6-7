using UnityEngine;

public class follow : MonoBehaviour
{
    [SerializeField] private Transform objToFollow;
    [SerializeField] private Vector3 offset = new Vector3(0f,2.25f,0f);
    void LateUpdate()
    {
        transform.position = objToFollow.position + offset;
    }
}
