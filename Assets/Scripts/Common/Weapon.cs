using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Bullet _bullet;
        [SerializeField] private float _bulletsPerTime;
        [SerializeField] private float _minTimeForShots;
        [SerializeField] private float _maxTimeForShots;
        [SerializeField] private AudioSource _fireSound;

        public event Action<GameObject> OnUpdate;

        private Pool<Bullet> _bulletPool;
        private float _shotsPerTime;
        private float _bulletLifetime;
        private float _lastTimeShoot;
        private Transform _startBulletPosition;

        public void Shoot(Vector2 direction)
        {
            if (Time.time - _lastTimeShoot >= _shotsPerTime)
            {
                _fireSound.Play();
                CreateBullet(direction);
                SetShotsNumberPerTime();
                _lastTimeShoot = Time.time;
            }
        }

        public void SetStartBulletPosition(Transform position)
        {
            _startBulletPosition = position;
        }

        public void SetBulletLifetime(float width)
        {
            _bulletLifetime = width;
        }

        private void Start()
        {
            _lastTimeShoot = Time.time;
            _bulletPool = new Pool<Bullet>(_bullet);
            SetShotsNumberPerTime();
        }

        private void SetShotsNumberPerTime()
        {
            var timeForShots = Random.Range(_minTimeForShots, _maxTimeForShots);
            _shotsPerTime = timeForShots / _bulletsPerTime;
        }

        private void CreateBullet(Vector2 direction)
        {
            var newBulletObject = _bulletPool.Instantiate();
            var newBullet = newBulletObject.GetComponent<Bullet>();
            newBullet.OnUpdate += OnUpdate;
            newBullet.OnDestroy += OnWeaponDestroy;
            newBullet.transform.position = _startBulletPosition.position;

            newBullet.SetDirection(direction);
            newBullet.SetLifetime(_bulletLifetime);
        }

        private void OnWeaponDestroy(Bullet bullet)
        {
            bullet.OnUpdate -= OnUpdate;
            bullet.OnDestroy -= OnWeaponDestroy;
            _bulletPool.Destroy(bullet);
        }
    }
}