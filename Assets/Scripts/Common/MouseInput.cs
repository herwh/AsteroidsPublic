using UnityEngine;

namespace Common
{
    public class MouseInput : IGameInput
    {
        public string Name => "Keyboard + mouse";
        public bool IsEnabled { get; set; }

        public void Update(Spaceship spaceship)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(1) || Input.GetKey(KeyCode.UpArrow))
            {
                spaceship.IncreaseAccelerationForce();
            }
            else
            {
                spaceship.DecreaseAccelerationForce();
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                spaceship.Fire();
            }

            var direction = Vector3.zero;
            var mousePosition = Input.mousePosition;
            var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var spaceshipDirection = spaceship.transform.right;
            var spaceshipToMouseDirection = (worldMousePosition - spaceship.transform.position).normalized;
            var directionRatio = Vector3.Dot(spaceshipDirection, spaceshipToMouseDirection);

            if (directionRatio < 0)
            {
                direction = Vector3.forward;
            }

            if (directionRatio > 0)
            {
                direction = Vector3.back;
            }

            spaceship.RotateByDirection(direction);
        }
    }
}