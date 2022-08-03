using System;
using System.Collections;
using UnityEngine;

namespace Common
{
    public class SpaceshipController : MonoBehaviour
    {
        [SerializeField] private Spaceship _spaceship;
        [SerializeField] private float _gameEndTime;
        public Spaceship Spaceship => _spaceship;
        public event Action<Spaceship> SpaceshipSpawned;
        public event Action SpaceshipDestroyed;
        public event Action SpaceshipOnHit;
        public Transform SpaceshipTransform => _currentSpaceship.transform;

        private IGameInput _currentGameInput = new MouseInput();
        private Spaceship _currentSpaceship;

        public Spaceship GetSpaceship()
        {
            return _currentSpaceship;
        }

        public void SpaceshipSpawn()
        {
            _currentSpaceship = Instantiate(_spaceship);
            _currentSpaceship.SetInitialHealthCount();
            _currentSpaceship.OnHit += OnHit;
            _currentSpaceship.OnDestroy += SpaceshipOnDestroy;

            if (SpaceshipSpawned != null) SpaceshipSpawned(_currentSpaceship);

            _currentSpaceship.SetInvincible();
        }

        public void SetInputState(bool state)
        {
            _currentGameInput.IsEnabled = state;
        }

        public void ChangeInput()
        {
            if (_currentGameInput is MouseInput)
            {
                _currentGameInput = new KeyboardInput();
            }
            else
            {
                _currentGameInput = new MouseInput();
            }
        }

        public string GetInputName()
        {
            return _currentGameInput.Name;
        }

        public void Disable()
        {
            if (_currentSpaceship != null)
            {
                _currentSpaceship.OnHit -= OnHit;
                _currentSpaceship.OnDestroy += SpaceshipOnDestroy;

                Destroy(_currentSpaceship.gameObject);
            }
        }

        private void Update()
        {
            if (_currentSpaceship != null)
            {
                _currentGameInput.Update(_currentSpaceship);
            }
        }

        private void SpaceshipOnDestroy()
        {
            StartCoroutine(DestroyCoroutine());
            _currentGameInput.IsEnabled = false;
            _currentSpaceship.OnHit -= OnHit;
            _currentSpaceship.OnDestroy -= SpaceshipOnDestroy;
        }

        private void OnHit()
        {
            if (SpaceshipOnHit != null) SpaceshipOnHit();
        }

        private IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(_gameEndTime);
            if (SpaceshipDestroyed != null) SpaceshipDestroyed();
            _currentSpaceship.gameObject.SetActive(false);
        }
    }
}