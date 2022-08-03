using System;
using UnityEngine;

namespace Common
{
    public class UFO : MonoBehaviour, IBulletHit, IPoolable
    {
        [SerializeField] private float _distanceTime;
        [SerializeField] private LayerMask _enemyLayerMasks;
        [SerializeField] private Weapon _weapon;

        public event Action<GameObject> OnUpdate;
        public event Action<UFO> Destroyed;
        public event Action<UFO> DestroyedFromBullet;
        public Weapon Weapon => _weapon;

        private float _speed;
        private Vector3 _direction;
        private Transform _target;

        public void SetSpeed(float distance)
        {
            _speed = distance / _distanceTime;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        public void Hit()
        {
            if (DestroyedFromBullet != null) DestroyedFromBullet(this);
        }

        private void Start()
        {
            _weapon.OnUpdate += OnUpdate;
            _weapon.SetStartBulletPosition(transform);
        }

        private void Update()
        {
            Move();
            Fire();
        }

        private void Move()
        {
            transform.position += _direction * _speed * Time.deltaTime;
            if (OnUpdate != null) OnUpdate(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((_enemyLayerMasks.value & (1 << other.gameObject.layer)) > 0)
            {
                if (Destroyed != null) Destroyed(this);
            }
        }

        private void Fire()
        {
            Vector2 distanceToSpaceship = _target.transform.position - transform.position;
            Vector2 direction = distanceToSpaceship.normalized;
            _weapon.Shoot(direction);
        }

        public void Spawn()
        {
            gameObject.SetActive(true);
        }

        public void DeSpawn()
        {
            _weapon.OnUpdate -= OnUpdate;
            gameObject.SetActive(false);
        }
    }
}
