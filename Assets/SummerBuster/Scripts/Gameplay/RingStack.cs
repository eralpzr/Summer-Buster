using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SummerBuster.Enums;
using SummerBuster.Manager;
using UnityEngine;

namespace SummerBuster.Gameplay
{
    public sealed class RingStack : MonoBehaviour
    {
        [SerializeField] private float _stackSpace;
        [Space, SerializeField] private RingColor[] _initialRings;
        
        private readonly Stack<Ring> _rings = new Stack<Ring>();

        private void Awake()
        {
            Initialize();
        }

        public Ring Pop()
        {
            return _rings.Count == 0 ? null : _rings.Pop();
        }

        public void Push(Ring ring, bool withAnimation)
        {
            _rings.Push(ring);
            CanBePop = false;

            Tweener tweener;
            if (withAnimation)
            {
                ring.transform.position = new Vector3(transform.position.x, ring.transform.position.y, transform.position.z);
                tweener = ring.transform.DOMove(GetRingPosition(_rings.Count - 1), .75f).SetEase(Ease.OutBounce); 
                tweener.onComplete = () => CanBePop = true;
                return;
            }

            tweener = ring.transform.DOMove(GetRingPosition(_rings.Count - 1), .5f);
            tweener.onComplete = () => CanBePop = true;
        }

        public bool Check()
        {
            if (_rings.Count == 1)
                return false;
            
            return _rings.Count == 0 || _rings.Select(x => x.ringColor).Distinct().Count() == 1;
        }
        
        private void Initialize()
        {
            for (int i = 0; i < _initialRings.Length; i++)
            {
                Ring prefab;
                switch (_initialRings[i])
                {
                    case RingColor.Pink:
                        prefab = GameManager.Instance.gameData.pinkRingPrefab;
                        break;

                    case RingColor.Yellow:
                        prefab = GameManager.Instance.gameData.yellowRingPrefab;
                        break;

                    case RingColor.Green:
                        prefab = GameManager.Instance.gameData.greenRingPrefab;
                        break;

                    case RingColor.Blue:
                        prefab = GameManager.Instance.gameData.blueRingPrefab;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var ring = Instantiate(prefab, GetRingPosition(i), Quaternion.identity, transform);
                _rings.Push(ring);
            }

            CanBePop = true;
        }

        private Vector3 GetRingPosition(int idx)
        {
            return transform.TransformPoint(new Vector3(0f, 20f + _stackSpace * idx, 0f));
        }

        public Vector3 GhostRingPosition => GetRingPosition(_rings.Count);
        public bool CanBePop { get; private set; }
    }
}