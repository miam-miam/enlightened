using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Painting
{
    public class BallThrower : MonoBehaviour
    {
        [SerializeField] private String fireKey = "Fire1";
        [SerializeField] private BallProjection ballProjection;
        [SerializeField] private Transform throwPosition;

        public Ball ballPrefab;
        // Using 0 to indicate
        private float mousePressedAt = 0;
        private float thrust = 0;
        public float maxThrust = 10;
        [Tooltip("How much to increase the thrust by second")] public float thrustIncrease = 1;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetButtonDown(fireKey))
            {
                mousePressedAt = Time.time;
            }
            else if (Input.GetButtonUp(fireKey))
            {
                mousePressedAt = 0;
                thrust = 0;
            }

            if (Input.GetButton(fireKey))
            {
                thrust = Math.Min(maxThrust, thrust + thrustIncrease * Time.deltaTime);
                var position = throwPosition.position;
                var worldMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                worldMousePosition.z = position.z;
                var direction =  worldMousePosition - position;
                direction.Normalize();
                ballProjection.SimulateTrajectory(ballPrefab, position, direction * thrust);
            }
        }
    }
}
