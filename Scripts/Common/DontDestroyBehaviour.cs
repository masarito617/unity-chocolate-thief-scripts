using UnityEngine;

namespace Chocolate.Common
{
    public class DontDestroyBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
