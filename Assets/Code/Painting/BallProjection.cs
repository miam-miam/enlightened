using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Code.Painting
{
    public class BallProjection : MonoBehaviour
    {
        [SerializeField] private LineRenderer line;
        [SerializeField] private int maxPhysicsFrameIterations = 100;
    
        private Scene simulationScene;
        private PhysicsScene2D physicsScene;


        private void Start()
        {
            simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            physicsScene = simulationScene.GetPhysicsScene2D();
        }

        public void SimulateTrajectory(Ball ballPrefab, Vector3 pos, Vector3 velocity) {
            var ghostBall = Instantiate(ballPrefab, pos, Quaternion.identity);
            ghostBall.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostBall.gameObject, simulationScene);

            ghostBall.Init(velocity, true);

            line.positionCount = maxPhysicsFrameIterations;

            for (var i = 0; i < maxPhysicsFrameIterations; i++) {
                physicsScene.Simulate(Time.fixedDeltaTime);
                line.SetPosition(i, ghostBall.transform.position);
            }

            Destroy(ghostBall.gameObject);
        }
    }
}
