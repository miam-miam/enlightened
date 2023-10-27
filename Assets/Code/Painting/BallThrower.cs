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
        [SerializeField] private float gravity = 9.81f;

        public Ball ballPrefab;
        // Using 0 to indicate
        private float mousePressedAt = 0;
        private float thrust = 0;
        public float maxThrust = 10;
        public float initialThrust = 10;
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
            // Remove static paint child.
            Destroy(ghostBall.transform.GetChild(0).gameObject);
            SceneManager.MoveGameObjectToScene(ghostBall.gameObject, simulationScene);
        }

        // Calculate Thrust by taking in the world space mouse position and adding an
        // approximation for the loss in height due to gravity
        private Vector3 CalculateVelocity(Vector3 mousePosition)
        {
            var actualDirection = (mousePosition - throwPosition.position);
            var actualVelocity = actualDirection.normalized * thrust;
            
            // 1/2 * a * t^2 where t = s / v
            var gravityDisplacement = actualVelocity.x == 0 ? 0 : gravity * Mathf.Pow(actualDirection.x / actualVelocity.x, 2) / 2;
            
            var simulatedDirection = actualDirection + new Vector3(0, gravityDisplacement, 0);
            var simulatedVelocity = simulatedDirection.normalized * thrust;
            
            return simulatedVelocity;
        }

        private void Update()
        {
            if (Input.GetButtonDown(fireKey))
            {
                mousePressedAt = Time.time;
                thrust = initialThrust;
            }
            else if (Input.GetButtonUp(fireKey))
            {
                var ball = Instantiate(ballPrefab, throwPosition.position, Quaternion.identity);
                var worldMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                worldMousePosition.z = throwPosition.position.z;

                ball.Init(CalculateVelocity(worldMousePosition), false);
                
                mousePressedAt = 0;
                thrust = 0;
                line.positionCount = 0;
            }
            else if (Input.GetButton(fireKey))
            {
                thrust = Math.Min(maxThrust, thrust + thrustIncrease * Time.deltaTime);
                
                var worldMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                worldMousePosition.z = throwPosition.position.z;
                
                SimulateTrajectory(throwPosition.position, CalculateVelocity(worldMousePosition));
            }
            else
            {
                line.positionCount = 0;
            }
        }
    }
}
