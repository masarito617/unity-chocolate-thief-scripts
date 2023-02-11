using UnityEngine;

namespace Chocolate.Girls
{
    public class GirlMarkerBehaviour : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material standardMaterial;
        [SerializeField] private Material triggerMaterial;

        public void SetTriggerMaterial(bool isTrigger)
        {
            meshRenderer.material = isTrigger ? triggerMaterial : standardMaterial;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
