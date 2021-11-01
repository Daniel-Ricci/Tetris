using UnityEngine;
using UnityEngine.UI;

namespace Score
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text levelText;

        private int _currentScore = 0;

        private void Start()
        {
            scoreText.text = _currentScore.ToString();
        }

        public void AddToScore(int score)
        {
            _currentScore += score;
            scoreText.text = _currentScore.ToString();
        }

        public int GetCurrentScore()
        {
            return _currentScore;
        }

        public void UpdateLevel(int level)
        {
            levelText.text = "Level " + level;
        }
    }
}
