using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public class AsteroidGenerator : MonoBehaviour
    {
        [SerializeField] private Asteroid _asteroid;
        [SerializeField] private int _startAsteroidCount;
        [SerializeField] private float _minSpeed;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private List<float> _smallerAsteroidRotationAngles;
        [SerializeField] private int _countOfSmallerAsteroids; //asteroids after bullet collision
        [SerializeField] private int _scaleLossFactor;

        public event Action<GameObject> AsteroidOnUpdate;
        public event Action<Asteroid> AsteroidDestroyedFromBullet;

        private Vector3 _initialScale;
        private Coroutine _generationCoroutine;
        private int _asteroidsCountInWave;
        private int _currentAsteroidCount;
        private Vector2 _minPosition;
        private Vector2 _maxPosition;
        private Pool<Asteroid> _asteroidPool;

        public void SetBorder(Vector2 minPosition, Vector2 maxPosition)
        {
            _minPosition = minPosition;
            _maxPosition = maxPosition;
        }

        public void StartGenerate()
        {
            _currentAsteroidCount = 0;

            if (_generationCoroutine != null)
            {
                StopCoroutine(_generationCoroutine);
                _generationCoroutine = null;
            }

            _asteroidsCountInWave = _startAsteroidCount;
            CreateAsteroidWave();
        }

        public void Disable()
        {
            if (_generationCoroutine != null)
            {
                StopCoroutine(_generationCoroutine);
                _generationCoroutine = null;
            }

            _asteroidPool.Clear();
        }

        private void Awake()
        {
            _asteroidPool = new Pool<Asteroid>(_asteroid);
        }

        private Asteroid CreateAsteroid()
        {
            _currentAsteroidCount++;

            var newAsteroidObject = _asteroidPool.Instantiate();
            var newAsteroid = newAsteroidObject.GetComponent<Asteroid>();
            newAsteroid.OnUpdate += AsteroidOnUpdate;
            newAsteroid.BulletCollision += BulletCollisionHandler;
            newAsteroid.DestroyedFromBullet += AsteroidDestroyedFromBullet;
            newAsteroid.ObstacleCollision += DestroyAsteroid;

            var randomSpawnPositionByX = Random.Range(_minPosition.x, _maxPosition.x);
            var randomSpawnPositionByY = Random.Range(_minPosition.y, _maxPosition.y);
            var randomSpawnPosition = new Vector2(randomSpawnPositionByX, randomSpawnPositionByY);
            newAsteroid.transform.position = randomSpawnPosition;

            var randomDirection = Random.insideUnitCircle.normalized;
            newAsteroid.SetDirection(randomDirection);

            newAsteroid.SetRandomSize();
            newAsteroid.SetSpeed(GetRandomSpeed());
            newAsteroid.SetHealth(_asteroid.MAXHealthCount);

            return newAsteroid;
        }

        private void DestroyAsteroid(Asteroid asteroid)
        {
            asteroid.OnUpdate -= AsteroidOnUpdate;
            asteroid.BulletCollision -= BulletCollisionHandler;
            asteroid.ObstacleCollision -= DestroyAsteroid;
            asteroid.DestroyedFromBullet -= AsteroidDestroyedFromBullet;

            _asteroidPool.Destroy(asteroid);
            _currentAsteroidCount--;

            if (_currentAsteroidCount <= 0)
            {
                _generationCoroutine = StartCoroutine(NewAsteroidWaveCoroutine());
            }
        }

        private void BulletCollisionHandler(Asteroid asteroid)
        {
            var health = asteroid.CurrentHealthCount;

            if (health > 0)
            {
                var speed = GetRandomSpeed();

                for (int i = 0; i < _countOfSmallerAsteroids; i++)
                {
                    var newSmallerAsteroid = CreateAsteroid();
                    var smallerAsteroidDirection =
                        Quaternion.Euler(0, 0, _smallerAsteroidRotationAngles[i]) * asteroid.Direction;

                    newSmallerAsteroid.transform.position = asteroid.transform.position;
                    newSmallerAsteroid.SetDirection(smallerAsteroidDirection);
                    newSmallerAsteroid.SetSpeed(speed);
                    newSmallerAsteroid.SetHealth(health);

                    var healthDifference = asteroid.MAXHealthCount - health;

                    if (healthDifference != 0)
                    {
                        newSmallerAsteroid.transform.localScale /= healthDifference * _scaleLossFactor;
                    }
                }
            }

            DestroyAsteroid(asteroid);
        }

        private float GetRandomSpeed()
        {
            return Random.Range(_minSpeed, _maxSpeed);
        }

        private void CreateAsteroidWave()
        {
            for (int i = 0; i < _asteroidsCountInWave; i++)
            {
                CreateAsteroid();
            }

            _asteroidsCountInWave++;
        }

        private IEnumerator NewAsteroidWaveCoroutine()
        {
            yield return new WaitForSeconds(2f);
            CreateAsteroidWave();
        }
    }
}