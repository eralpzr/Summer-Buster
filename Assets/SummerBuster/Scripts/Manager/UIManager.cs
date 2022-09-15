using MoneyBuster.UI;
using UnityEngine;

namespace SummerBuster.Manager
{
    public sealed class UIManager : Singleton<UIManager>
    {
        public UIObject levelCompletedPanel;
        public UIObject gamePanel;

        [Space] public UIText levelText;
        public UIScoreText scoreText;
    }
}