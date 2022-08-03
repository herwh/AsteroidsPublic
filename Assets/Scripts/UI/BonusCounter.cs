using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BonusCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _bonusText;
        [SerializeField] private GameController _gameController;
        [SerializeField] private int _UFOBonus;
        [SerializeField] private List<int> _asteroidsBonuses;

        private int _bonusCount;

        private void OnEnable()
        {
            _gameController.NewGameStarted += ResetBonusCount;
            _gameController.UFOGenerator.UFODestroyedFromBullet += UpdateBonusCountForUFO;
            _gameController.AsteroidGenerator.AsteroidDestroyedFromBullet += UpdateBonusCountForAsteroid;
            DisplayBonusCount();
        }

        private void UpdateBonusCountForUFO()
        {
            _bonusCount += _UFOBonus;
            DisplayBonusCount();
        }

        private void ResetBonusCount()
        {
            _bonusCount = 0;
            DisplayBonusCount();
        }

        private void UpdateBonusCountForAsteroid(Asteroid asteroid)
        {
            var currentBonus = _asteroidsBonuses[asteroid.CurrentHealthCount];
            _bonusCount += currentBonus;

            DisplayBonusCount();
        }

        private void DisplayBonusCount()
        {
            _bonusText.text = _bonusCount.ToString();
        }

        private void OnDisable()
        {
            _gameController.UFOGenerator.UFODestroyedFromBullet -= UpdateBonusCountForUFO;
            _gameController.AsteroidGenerator.AsteroidDestroyedFromBullet -= UpdateBonusCountForAsteroid;
            _gameController.NewGameStarted -= ResetBonusCount;
        }
    }
}
