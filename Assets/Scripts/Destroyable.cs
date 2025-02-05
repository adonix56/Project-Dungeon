using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public void PublicDestroy()
    {
        Destroy(gameObject);
    }
}
