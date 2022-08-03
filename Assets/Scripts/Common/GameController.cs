using System;
using UI;
using UnityEngine;

namespace Common
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private SpaceshipController _spaceshipController;
        [SerializeField] private AsteroidGenerator _asteroidGenerator;
        [SerializeField] private UFOGenerator _UFOGenerator;
        [SerializeField] private GameUI _gameUI;
        public UFOGenerator UFOGenerator => _UFOGenerator;
        public AsteroidGenerator AsteroidGenerator => _asteroidGenerator;
        public SpaceshipController SpaceshipController => _spaceshipController;
        public Action NewGameStarted;

        private float _screenWidth;
        private Vector2 _minPosition;
        private Vector2 _maxPosition;

        public void StartNewGame()
        {
            NewGameStarted();

            _asteroidGenerator.StartGenerate();
            _spaceshipController.SpaceshipSpawn();
            _UFOGenerator.StartGenerate();
            _UFOGenerator.SetUFOTarget(_spaceshipController.SpaceshipTransform);
        }

        public void RemoveAllObjects()
        {
            _asteroidGenerator.Disable();
            _UFOGenerator.Disable();
            _spaceshipController.Disable();
        }

        private void Start()
        {
            CalculateScreenWidth();

            _gameUI.ShowMenuBar();

            _spaceshipController.SpaceshipSpawned += OnSpaceshipSpawn;
            _asteroidGenerator.AsteroidOnUpdate += ControlObjectPosition;
            _UFOGenerator.UFOOnUpdate += ControlObjectPosition;
            _UFOGenerator.UFOSpawned += OnUFOSpawn;

            _asteroidGenerator.SetBorder(_minPosition, _maxPosition);
            _UFOGenerator.SetBorder(_minPosition, _maxPosition);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                _gameUI.ShowMenuBar();
            }
        }

        private void OnUFOSpawn(UFO ufo)
        {
            ufo.Weapon.SetBulletLifetime(_screenWidth);
        }

        private void OnSpaceshipSpawn(Spaceship spaceship)
        {
            spaceship.Weapon.SetBulletLifetime(_screenWidth);
            spaceship.OnUpdate += ControlObjectPosition;
        }

        private void CalculateScreenWidth()
        {
            _maxPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            _minPosition = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            _screenWidth = _maxPosition.x - _minPosition.x;
        }

        private void ControlObjectPosition(GameObject obj)
        {
            Vector2 position = obj.transform.position;
            if (obj.transform.position.x < _minPosition.x)
            {
                position.x = _maxPosition.x;
            }

            if (obj.transform.position.y < _minPosition.y)
            {
                position.y = _maxPosition.y;
            }

            if (obj.transform.position.x > _maxPosition.x)
            {
                position.x = _minPosition.x;
            }

            if (obj.transform.position.y > _maxPosition.y)
            {
                position.y = _minPosition.y;
            }

            obj.transform.position = position;
        }

        private void OnDestroy()
        {
            var spaceship = _spaceshipController.GetSpaceship();
            if (spaceship != null)
            {
                spaceship.OnUpdate -= ControlObjectPosition;
            }

            _spaceshipController.SpaceshipSpawned -= OnSpaceshipSpawn;
            _asteroidGenerator.AsteroidOnUpdate -= ControlObjectPosition;
            _UFOGenerator.UFOOnUpdate -= ControlObjectPosition;
            _UFOGenerator.UFOSpawned -= OnUFOSpawn;
        }
    }
}
 