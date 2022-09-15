using System;
using System.Linq;
using SummerBuster.Manager;
using SummerBuster.Models;
using UnityEngine;

namespace SummerBuster.Gameplay
{
    public sealed class Level : MonoBehaviour
    {
        private const float LerpSpeed = 10f;
        
        [SerializeField] private RingStack[] _ringStacks;
        private HoldingRingInfo HoldingRingInfo { get; set; }

        private GameObject _ghostRingObject;
        
        private void Awake()
        {
            GameManager.Instance.Level = this;
        }

        private void Update()
        {
            if (HoldingRingInfo == null && Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(GameManager.Instance.mainCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, 50f, LayerMask.GetMask("RingStack")))
                {
                    var ringStack = hitInfo.transform.GetComponent<RingStack>();
                    if (!ringStack.CanBePop)
                        return;
                    
                    var ring = ringStack.Pop();
                    if (ring)
                    {
                        HoldingRingInfo = new HoldingRingInfo(ringStack, ring);
                    }
                }
            }
            else if (HoldingRingInfo != null && Input.GetMouseButton(0))
            {
                if (Physics.Raycast(GameManager.Instance.mainCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, 50f, LayerMask.GetMask("Ground")))
                {
                    var ringTransform = HoldingRingInfo.ring.transform;
                    ringTransform.position = Vector3.Lerp(ringTransform.position, new Vector3(hitInfo.point.x, GameManager.Instance.gameData.MovingYOffset, ringTransform.position.z), Time.deltaTime * LerpSpeed);
                }
                
                // Checking if we touching any ring stack and showing ghost ring to player
                if (Physics.Raycast(GameManager.Instance.mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, 50f, LayerMask.GetMask("RingStack")))
                {
                    if (_ghostRingObject)
                        return;
                    
                    var ringStack = hitInfo.transform.GetComponent<RingStack>();
                    _ghostRingObject = ObjectPooler.Spawn(HoldingRingInfo.ring.ghostPoolTag, ringStack.GhostRingPosition, Quaternion.identity, ringStack.transform);
                    _ghostRingObject.transform.localScale = HoldingRingInfo.ring.transform.localScale;
                }
                else if (_ghostRingObject)
                {
                    ObjectPooler.Destroy(_ghostRingObject);
                    _ghostRingObject = null;
                }
            }
            else if (HoldingRingInfo != null && Input.GetMouseButtonUp(0))
            {
                if (_ghostRingObject)
                    ObjectPooler.Destroy(_ghostRingObject);

                if (Physics.Raycast(GameManager.Instance.mainCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, 50f, LayerMask.GetMask("RingStack")))
                {
                    var ringStack = hitInfo.transform.GetComponent<RingStack>();
                    ringStack.Push(HoldingRingInfo.ring, true);
                    HoldingRingInfo = null;
                    
                    CheckRings();
                    return;                    
                }
                
                HoldingRingInfo.previousRingStack.Push(HoldingRingInfo.ring, false);
                HoldingRingInfo = null;
            }
        }

        private void CheckRings()
        {
            var success = _ringStacks.All(x => x.Check());
            if (!success)
                return;

            var emptyRingStack = _ringStacks.FirstOrDefault(x => x.IsEmpty);
            if (emptyRingStack)
                emptyRingStack.StartDance();
                
            UIManager.Instance.scoreText.Show("NICE", 25);
            GameManager.Instance.GiveScore(25);
            GameManager.Instance.StartCoroutine(GameManager.Instance.CompleteLevelCoroutine(false));
        }
    }
}