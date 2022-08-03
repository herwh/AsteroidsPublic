using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public class UFOGenerator : MonoBehaviour
    {
        [SerializeField] private UFO _UFO;
        [SerializeField] private float _minSpawnTime;
        [SerializeField] private float _maxSpawnTime;
        [SerializeField] private int _spawnPercentagesBorder;

        public event Action<GameObject> UFOOnUpdate;
        public event Action<UFO> UFOSpawned;
        public event Action UFODestroyedFromBullet;
        
        private Coroutine _generationCoroutine;
        private Pool<UFO> _UFOPool;
        private Vector2 _minPosition;
        private Vector2 _maxPosition;
        private Transform _target;

        public void Disable()
        {
            if (_generationCoroutine != null)
            {
                StopCoroutine(_generationCoroutine);
                _generationCoroutine = null;
            }

            _UFOPool.Clear();
        }

        public void SetBorder(Vector2 minPosition, Vector2 maxPosition)
        {
            _minPosition = minPosition;
            _maxPosition = maxPosition;
        }

        public void SetUFOTarget(Transform target)
        {
            _target = target;
        }

        public void StartGenerate()
        {
            if (_generationCoroutine != null)
            {
                StopCoroutine(_generationCoroutine);
                _generationCoroutine = null;
            }

            SpawnUFO();
        }
        
        private void Awake()
        {
            _UFOPool = new Pool<UFO>(_UFO);
        }

        private void CreateUFO()
        {
            var borderArea = 100f / _spawnPercentagesBorder;
            var newUFOObject = _UFOPool.Instantiate();
            var newUFO = newUFOObject.GetComponent<UFO>();

            newUFO.Destroyed += UFODestroyed;
            newUFO.OnUpdate += UFOOnUpdate;
            newUFO.DestroyedFromBullet += DestroyedFromBullet;

            var minSpawnPositionByY = _minPosition.y + (_maxPosition.y / borderArea);
            var maxSpawnPositionByY = _maxPosition.y - (_maxPosition.y / borderArea);
            var randomSpawnPositionByX = Random.Range(_minPosition.x, _maxPosition.x);
            var randomSpawnPositionByY = Random.Range(minSpawnPositionByY, maxSpawnPositionByY);
            var randomSpawnPosition = new Vector2(randomSpawnPositionByX, randomSpawnPositionByY);
            newUFO.transform.position = randomSpawnPosition;

            newUFO.SetSpeed(_maxPosition.x - _minPosition.x);
            newUFO.SetTarget(_target);

            var randomDirectionByX = new Vector2(GetRandomDirection(), 0);
            newUFO.SetDirection(randomDirectionByX);

            if (UFOSpawned != null) UFOSpawned(newUFO);
        }

        private int GetRandomDirection()
        {
            int[] sides = {-1, 1}; //1 = right; -1 left
            int randomSide = Random.Range(0, sides.Length);
            return sides[randomSide];
        }

        private float GetRandomSpawnTime()
        {
            return Random.Range(_minSpawnTime, _maxSpawnTime);
        }

        private void UFODestroyed(UFO ufo)
        {
            ufo.Destroyed -= UFODestroyed;
            ufo.OnUpdate -= UFOOnUpdate;
            ufo.DestroyedFromBullet -= DestroyedFromBullet;
            _UFOPool.Destroy(ufo);
            SpawnUFO();
        }

        private void SpawnUFO()
        {
            _generationCoroutine = StartCoroutine(SpawnDelay());
        }

        private void DestroyedFromBullet(UFO ufo)
        {
            UFODestroyed(ufo);
            if (UFODestroyedFromBullet != null) UFODestroyedFromBullet();
        }

        private IEnumerator SpawnDelay()
        {
            yield return new WaitForSeconds(GetRandomSpawnTime());
            CreateUFO();
        }
    }
}