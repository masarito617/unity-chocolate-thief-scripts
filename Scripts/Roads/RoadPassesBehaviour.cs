using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chocolate.Roads
{
    public class RoadPassesBehaviour : MonoBehaviour
    {
        [Serializable] public class RoadPass
        {
            public Transform startPosition;
            public Transform endPosition;
        }
        [SerializeField] private List<RoadPass> roadPassList;

        public RoadPass GetRandomRoadPass()
        {
            return roadPassList[Random.Range(0, roadPassList.Count-1)];
        }
    }
}
