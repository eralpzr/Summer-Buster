using TMPro;
using UnityEngine;

namespace MoneyBuster.UI
{
    public class UIText : UIObject
    {
        [SerializeField] protected TMP_Text _textObject;

        public string Text
        {
            set => _textObject.text = value;
        }
    }
}