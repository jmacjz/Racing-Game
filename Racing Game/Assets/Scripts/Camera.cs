using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        Vector3 rotatedOffset = player.rotation * offset;
        transform.position = player.position + rotatedOffset;
        transform.LookAt(player);
    }
}
