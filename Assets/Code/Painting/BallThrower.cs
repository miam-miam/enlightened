using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Code.Painting
{
    public class BallThrower : MonoBehaviour
    {
        [SerializeField] private String fireKey = "Fire1";
        [SerializeField] private Transform throwPosition;

        public Ball ballPrefab;
        // Using 0 to indicate
        private float mousePressedAt = 0;
        private float thrust = 0;
        public float maxThrust = 10;
        [Tooltip("How much to increase the thrust by second")] public float thrustIncrease = 1;
        private Camera mainCamera;
        
        [SerializeField] private LineRenderer line;
        [SerializeField] private int maxPhysicsFrameIterations = 100;
    
        private Scene simulationScene;
        private PhysicsScene2D physicsScene;
        private Ball ghostBall;

        private Vector3 lastPos;
        private Vector3 lastVelocity;
        

        public void SimulateTrajectory(Vector3 pos, Vector3 velocity)
        {
            if (lastPos == pos && lastVelocity == velocity) return;
            lastPos = pos;
            lastVelocity = velocity;

            var ghostTransform = ghostBall.transform;
            ghostTransform.position = pos;
            ghostBall.Init(velocity, true);

            line.positionCount = maxPhysicsFrameIterations;

            for (var i = 0; i < maxPhysicsFrameIterations; i++) {
                physicsScene.Simulate(Time.fixedDeltaTime);
                line.SetPosition(i, ghostTransform.position);
            }
        }

        private void Start()
        {
            mainCamera = Camera.main;
            simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            physicsScene = simulationScene.GetPhysicsScene2D();
            ghostBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            ghostBall.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostBall.gameObject, simulationScene);
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
                SimulateTrajectory(position, direction * thrust);
            }
        }
    }
}
