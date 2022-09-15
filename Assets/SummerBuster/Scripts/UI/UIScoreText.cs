using System;
using UnityEngine;

namespace MoneyBuster.UI
{
    public class UIScoreText : UIText
    {
        [SerializeField] private Color _redColor;
        [SerializeField] private Color _greenColor;
        
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Show(string comment, int score)
        {
            Text = $"{comment}\n{(score > 0 ? "+" : "")}{score}";
            _textObject.color = score < 0 ? _redColor : _greenColor;
            
            _animator.SetTrigger("Fade");
        }
    }
}