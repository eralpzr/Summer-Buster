using System.Collections;
using SummerBuster.Data;
using SummerBuster.Enums;
using SummerBuster.Gameplay;
using SummerBuster.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SummerBuster.Manager
{
    public sealed class GameManager : Singleton<GameManager>
    {
        public bool isDebug;
        public int debugLevel;
        
        [Space] public Camera mainCamera;
        public GameData gameData;
        
        [Space, SerializeField] private string[] _levels;

        private string _currentLevelScene;

        public GameState GameState { get; set; }
        public Level Level { get; set; }

        protected override void Awake()
        {
            base.Awake();
            GameState = GameState.None;
        }
        
        private void Start()
        {
            if (isDebug)
            {
                StartLevel(_levels[(debugLevel - 1) % _levels.Length]);
                return;
            }
            
            // We are looping levels when player reached last level
            StartLevel(_levels[(Progression.Level - 1) % _levels.Length]);
        }
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            // Checking if only level scene loaded
            if (scene.name.Equals(_currentLevelScene))
            {
                UIManager.Instance.gamePanel.gameObject.SetActive(true);
                GameState = GameState.Playing;
            }
        }
        
        public void StartLevel(string levelScene)
        {
            // Unload if any active level scene and start new level
            if (!string.IsNullOrWhiteSpace(_currentLevelScene))
            {
                SceneManager.UnloadSceneAsync(_currentLevelScene);
                _currentLevelScene = null;
            }
            
            UIManager.Instance.gamePanel.gameObject.SetActive(true);
            UIManager.Instance.levelCompletedPanel.gameObject.SetActive(false);

            SceneManager.LoadScene(_currentLevelScene = levelScene, LoadSceneMode.Additive);
            RefreshLevelText();
        }
        
        public IEnumerator CompleteLevelCoroutine(bool failed)
        {
            GameState = GameState.Completed;

            if (!isDebug)
                Progression.Level++;

            yield return new WaitForSeconds(1f);
            
            UIManager.Instance.gamePanel.gameObject.SetActive(false);
            UIManager.Instance.levelCompletedPanel.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1.5f);
            
            Start();
        }
        
        public void RefreshLevelText()
        {
            UIManager.Instance.levelText.Text = $"Level {Progression.Level}\n{Progression.Score}";
        }

        public void GiveScore(int score)
        {
            Progression.Score += score;
            UIManager.Instance.scoreText.Show(score < 0 ? "OH NO!" : "NICE", score);
            RefreshLevelText();
        }
    }
}