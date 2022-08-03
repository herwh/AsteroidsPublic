using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpaceshipHealthPanel : MonoBehaviour
    {
        [SerializeField] private Image _spaceshipSprite;
        [SerializeField] private SpaceshipController _spaceshipController;

        private List<Image> _spaceshipSprites;
        private int _healthLeft;

        private void Start()
        {
            _spaceshipSprites = new List<Image>();
            SpaceshipPanelBuild(_spaceshipController.Spaceship.HealthCount);
            _spaceshipController.SpaceshipOnHit += RemoveSpaceshipSprite;
            _spaceshipController.SpaceshipSpawned += RefreshHealthPanel;
        }

        private void SpaceshipPanelBuild(int spaceshipsCount)
        {
            for (int spaceshipIndex = 0; spaceshipIndex < spaceshipsCount; spaceshipIndex++)
            {
                var spaceship =
                    Instantiate(_spaceshipSprite, transform);
                _spaceshipSprites.Add(spaceship);
            }
        }

        private void RemoveSpaceshipSprite()
        {
            _healthLeft--;
            var lastSpaceship = _spaceshipSprites[_healthLeft];
            lastSpaceship.color = new Color(1f, 1f, 1f, 0.3f);
        }

        private void RefreshHealthPanel(Spaceship spaceship)
        {
            _healthLeft = spaceship.HealthCount;

            for (int i = 0; i < spaceship.HealthCount; i++)
            {
                _spaceshipSprites[i].color = new Color(1f, 1f, 1f, 1f);
            }
        }

        private void OnDestroy()
        {
            _spaceshipController.SpaceshipOnHit -= RemoveSpaceshipSprite;
            _spaceshipController.SpaceshipSpawned -= RefreshHealthPanel;
        }
    }
}
