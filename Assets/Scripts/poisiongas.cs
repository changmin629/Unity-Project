using UnityEngine;

public class poisiongas : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);
    }
}
