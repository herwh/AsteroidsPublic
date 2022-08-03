using System;
using System.Collections;
using UnityEngine;

namespace Common
{
    public class Spaceship : MonoBehaviour, IBulletHit
    {
        [SerializeField] private Transform _muzzle;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _accelerationLossRate;
        [SerializeField] private float _maxAccelerationForce;
        [SerializeField] private LayerMask _enemyLayerMasks;
        [SerializeField] private float _safeTime;
        [SerializeField] private float _flashingPerSec;
        [SerializeField] private Weapon _weapon;
        [SerializeField] private int _healthCount;
        [SerializeField] private AudioSource _destroySound;
        [SerializeField] private ParticleSystem _explosionParticle;
        public event Action<GameObject> OnUpdate;
        public event Action OnHit;
        public event Action OnDestroy;
        public Weapon Weapon => _weapon;
        public int HealthCount => _healthCount;
        public int CurrentHealthCount => _currentHealthCount;

        private const float INITIALSPEED = 0;
        private float _speed;
        private float _accelerationForce;
        private float _invincibleTimeElapsed;
        private int _currentHealthCount;

        public void SetInvincible()
        {
            StartCoroutine(InvincibleCoroutine());
        }

        public void RotateByDirection(Vector3 direction)
        {
            transform.Rotate(direction * Time.deltaTime * _rotationSpeed);
        }

        public void IncreaseAccelerationForce()
        {
            _accelerationForce += Time.deltaTime;
            _accelerationForce = Mathf.Clamp(_accelerationForce, 0, _maxAccelerationForce);
        }

        public void DecreaseAccelerationForce()
        {
            _accelerationForce -= Time.deltaTime * _accelerationLossRate; //_accelerationLossRate - inertia

            if (_accelerationForce < 0)
            {
                _accelerationForce = 0;
            }
        }

        public void Fire()
        {
            Vector2 distanceToMuzzle = _muzzle.position - transform.position;
            Vector2 direction = distanceToMuzzle.normalized;
            _weapon.Shoot(direction);
        }

        public void Hit()
        {
            _currentHealthCount--;
            if (OnHit != null) OnHit();
            if (_currentHealthCount <= 0)
            {
                Destroy();
            }
        }

        public void SetInitialHealthCount()
        {
            _currentHealthCount = _healthCount;
        }

        private void Start()
        {
            _weapon.OnUpdate += OnUpdate;
            _weapon.SetStartBulletPosition(_muzzle);
        }

        private void Update()
        {
            MoveForward();
            if (OnUpdate != null) OnUpdate(gameObject);
        }

        private void CalculateSpeed()
        {
            var acceleration = 0f;
            if (_accelerationForce != 0)
            {
                acceleration = (_maxSpeed - INITIALSPEED) / _accelerationForce;
            }

            _speed = INITIALSPEED * _accelerationForce + (acceleration * _accelerationForce * _accelerationForce) / 2;
        }

        private void MoveForward()
        {
            CalculateSpeed();
            transform.position += transform.up * _speed * Time.deltaTime; //transform.up, а не Vector3.up
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((_enemyLayerMasks.value & (1 << other.gameObject.layer)) > 0)
            {
                Hit();
            }
        }

        private void Destroy()
        {
            _destroySound.Play();
            _weapon.OnUpdate -= OnUpdate;
            if (OnDestroy != null) OnDestroy();
            _explosionParticle.Play();
        }

        private IEnumerator InvincibleCoroutine()
        {
            var flashTime = 1f / _flashingPerSec;
            var spriteColor = _sprite.color;
            var maxAlpha = 1f;

            _collider2D.enabled = false;

            while (_invincibleTimeElapsed < _safeTime)
            {
                _invincibleTimeElapsed += flashTime;
                yield return new WaitForSeconds(flashTime);

                if (spriteColor.a == maxAlpha)
                {
                    spriteColor.a = 0f;
                }
                else
                {
                    spriteColor.a = maxAlpha;
                }

                _sprite.color = spriteColor;
            }

            _invincibleTimeElapsed = 0;
            _collider2D.enabled = true;
        }
    }
}