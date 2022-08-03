using UnityEngine;

namespace Common
{
    public class KeyboardInput : IGameInput
    {
        public string Name => "Keyboard";
        public bool IsEnabled { get; set; }

        public void Update(Spaceship spaceship)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (Input.GetKey(KeyCode.W))
            {
                spaceship.IncreaseAccelerationForce();
            }
            else
            {
                spaceship.DecreaseAccelerationForce();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                spaceship.Fire();
            }

            var direction = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
            {
                direction = Vector3.forward;
            }

            if (Input.GetKey(KeyCode.D))
            {
                direction = Vector3.back;
            }

            spaceship.RotateByDirection(direction);
        }
    }
}