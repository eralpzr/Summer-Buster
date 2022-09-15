using Sirenix.OdinInspector;
using SummerBuster.Gameplay;
using UnityEngine;

namespace SummerBuster.Data
{
    [CreateAssetMenu(fileName = "Game Data", menuName = "Summer Buster/Game Data", order = 0)]
    public class GameData : SerializedScriptableObject
    {
        [Header("Settings")]
        public float MovingYOffset;
        
        [Header("Prefabs")]
        public Ring pinkRingPrefab;
        public Ring yellowRingPrefab;
        public Ring greenRingPrefab;
        public Ring blueRingPrefab;
    }
}