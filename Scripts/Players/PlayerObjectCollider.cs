using System.Collections.Generic;
using Chocolate.Const;
using Chocolate.Girls;
using UnityEngine;

namespace Chocolate.Players
{
    public class PlayerObjectCollider : MonoBehaviour
    {
        /// <summary>
        /// 衝突中の女の子
        /// </summary>
        private readonly List<GirlBehaviour> _triggerGirlList = new List<GirlBehaviour>();

        private void LateUpdate()
        {
            // Update終わりにリセットする
            _triggerGirlList.Clear();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(GameConst.TagNameGirl))
            {
                var girlBehaviour = other.GetComponentInParent<GirlBehaviour>();
                if (girlBehaviour == null) Debug.Log("GirlBehaviour is null.");
                _triggerGirlList.Add(girlBehaviour);
            }
        }

        /// <summary>
        /// 怒っている女の子と衝突しているか？
        /// </summary>
        public bool IsTriggerAngryMoveGirl()
        {
            foreach (var triggerGirl in _triggerGirlList)
            {
                if (triggerGirl.IsAngryMove()) return true;
            }
            return false;
        }
    }
}
