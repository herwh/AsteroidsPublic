using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public class Asteroid : MonoBehaviour, IBulletHit, IPoolable
    {
        [SerializeField] private float _minSize;
        [SerializeField] private float _maxSize;
        [SerializeField] private int _maxHealthCount;
        [SerializeField] private LayerMask _obstacleLayerMasks;

        public event Action<GameObject> OnUpdate;
        public event Action<Asteroid> BulletCollision;
        public event Action<Asteroid> ObstacleCollision;
        public event Action<Asteroid> DestroyedFromBullet;
        public int CurrentHealthCount => _currentHealthCount;
        public int MAXHealthCount => _maxHealthCount;
        public Vector3 Direction => _direction;

        private float _speed;
        private float _size;
        private Vector3 _direction;
        private int _currentHealthCount;

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public void SetRandomSize()
        {
            _size = Random.Range(_minSize, _maxSize);
            transform.localScale = new Vector3(_size, _size, 1);
        }

        public void SetHealth(int health)
        {
            _currentHealthCount = health;
        }

        public void Hit()
        {
            _currentHealthCount--;
            if (DestroyedFromBullet != null) DestroyedFromBullet(this);
            if (BulletCollision != null) BulletCollision(this);
        }

        public void Spawn()
        {
            gameObject.SetActive(true);
        }

        public void DeSpawn()
        {
            gameObject.SetActive(false);
            _currentHealthCount = _maxHealthCount;

            OnUpdate = null;
            BulletCollision = null;
            ObstacleCollision = null;
            DestroyedFromBullet = null;
        }

        private void Awake()
        {
            _currentHealthCount = _maxHealthCount;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            transform.position += _direction * _speed * Time.deltaTime;
            if (OnUpdate != null) OnUpdate(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((_obstacleLayerMasks.value & (1 << other.gameObject.layer)) > 0)
            {
                if (ObstacleCollision != null) ObstacleCollision(this);
            }
        }
    }
}