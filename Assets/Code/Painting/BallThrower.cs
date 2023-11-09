using System;
using System.Linq;
using Assets.Code.Helpers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Code.Painting
{
    public class BallThrower : MonoBehaviour, ITransientStart
	{
        [SerializeField] private String fireKey = "Fire1";
        [SerializeField] private Transform throwPosition;
        [SerializeField] private float gravity = 9.81f;
        private const String SceneName = "Simulation";

        public Ball ballPrefab;
        // Using 0 to indicate
        private float mousePressedAt = 0;
        private float thrust = 0;
        private float gravityScale = 1;
        public float maxThrust = 10;
        public float maxThrustIncrease = 3;
        public float initialThrust = 10;
        [Tooltip("How much to increase the thrust by second")] public float thrustIncrease = 1;
        private Camera mainCamera;
        
        [SerializeField] private LineRenderer line;
        [SerializeField] private int maxPhysicsFrameIterations = 100;

        [SerializeField] [Tooltip("Time it takes before the player can shoot again")] float chargeTime = 1;
        private float lastThrown = 0;
    
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

            if (!CanShoot())
            {
                line.startColor = Color.gray;
                line.endColor = Color.gray;
            }
            else
            {
                line.startColor = Color.white;
                line.endColor = Color.white;
            }
        }

        private void Start()
        {
            mainCamera = Camera.main;
            TransientStart(false);
        }

        public void TransientStart(bool calledAgain)
        {
            if (calledAgain) return;
            var scene = SceneManager.GetSceneByName(SceneName);
            if (scene.IsValid())
            {
                simulationScene = scene;
                physicsScene = simulationScene.GetPhysicsScene2D();
                ghostBall = null;
                foreach (var obj in simulationScene.GetRootGameObjects())      
                {
                    if (ghostBall != null || !obj.TryGetComponent(out ghostBall)) Destroy(obj);
                }
            }
            else
            {
                simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
                physicsScene = simulationScene.GetPhysicsScene2D();
                ghostBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
                ghostBall.GetComponent<Renderer>().enabled = false;
                gravityScale = ghostBall.GetComponent<Rigidbody2D>().gravityScale;
                // Remove static paint child.
                Destroy(ghostBall.transform.GetChild(0).gameObject);
                SceneManager.MoveGameObjectToScene(ghostBall.gameObject, simulationScene);
            }
        }

        // Calculate Thrust by taking in the world space mouse position and adding an
        // approximation for the loss in height due to gravity
        private Vector3 CalculateVelocity(Vector3 mousePosition)
        {
            var calcThrust = thrust == maxThrust ? thrust + maxThrustIncrease : thrust;
            var actualDirection = (mousePosition - throwPosition.position);
            var actualVelocity = actualDirection.normalized * calcThrust;
            
            // 1/2 * a * t^2 where t = s / v
            var gravityDisplacement = actualVelocity.x == 0 ? 0 : gravity * gravityScale * Mathf.Pow(actualDirection.x / actualVelocity.x, 2) / 2;
            
            var simulatedDirection = actualDirection + new Vector3(0, gravityDisplacement, 0);
            var simulatedVelocity = simulatedDirection.normalized * calcThrust;
            
            return simulatedVelocity;
        }

        private bool CanShoot()
        {
            return (mousePressedAt - lastThrown > chargeTime || thrust == maxThrust);
        }

        private void Update()
        {
            if (Input.GetButtonDown(fireKey))
            {
                mousePressedAt = Time.time;
                thrust = initialThrust;
            }
            else if (Input.GetButtonUp(fireKey) && CanShoot())
            {
                ThrowBall();
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

        private void ThrowBall()
        {
            var ball = Instantiate(ballPrefab, throwPosition.position, Quaternion.identity);
            var worldMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            worldMousePosition.z = throwPosition.position.z;

            ball.Init(CalculateVelocity(worldMousePosition), false);

            lastThrown = Time.time;
            mousePressedAt = 0;
            thrust = 0;
            line.positionCount = 0;
        }
    }
}
