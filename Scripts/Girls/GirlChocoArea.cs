using UnityEngine;

namespace Chocolate.Girls
{
    public class GirlChocoArea : MonoBehaviour
    {
        [SerializeField] private GameObject chocoAreaL;
        [SerializeField] private GameObject chocoAreaR;

        private void Awake()
        {
            chocoAreaL.SetActive(false);
            chocoAreaR.SetActive(false);
        }

        public void SetActiveChocoAreaL(bool isActive)
        {
            chocoAreaL.SetActive(isActive);
        }

        public void SetActiveChocoAreaR(bool isActive)
        {
            chocoAreaR.SetActive(isActive);
        }
    }
}
