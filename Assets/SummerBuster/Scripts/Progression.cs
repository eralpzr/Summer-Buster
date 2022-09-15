using UnityEngine;

namespace SummerBuster
{
    public static class Progression
    {
        public static int Level
        {
            get => PlayerPrefs.GetInt("level", 1);
            set => PlayerPrefs.SetInt("level", value);
        }
        
        public static int Score
        {
            get => PlayerPrefs.GetInt("score", 0);
            set => PlayerPrefs.SetInt("score", value);
        }
    }
}