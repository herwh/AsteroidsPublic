using System;
using UnityEngine;

namespace Common
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _speed;
        [SerializeField] private LayerMask _enemyLayerMasks;

        public event Action<GameObject> OnUpdate;
        public event Action<Bullet> OnDestroy;

        private Vector3 _direction;
        private float _lifeTime;
        private float _totalLifetime;

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        public void SetLifetime(float totalLifetime)
        {
            _totalLifetime = totalLifetime;
        }

        public void Spawn()
        {
            gameObject.SetActive(true);
        }

        public void DeSpawn()
        {
            _lifeTime = 0;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            MoveForward();
        }

        private void MoveForward()
        {
            transform.position += _direction * _speed * Time.deltaTime;
            _lifeTime += Time.deltaTime;
            if (_speed * _lifeTime >= _totalLifetime)
            {
                if (OnDestroy != null) OnDestroy(this);
            }

            if (OnUpdate != null) OnUpdate(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((_enemyLayerMasks.value & (1 << other.gameObject.layer)) > 0)
            {
                other.gameObject.GetComponent<IBulletHit>().Hit();
                if (OnDestroy != null) OnDestroy(this);
            }
        }
    }
}