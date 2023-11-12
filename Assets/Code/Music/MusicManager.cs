using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TransientComponent))]
public class MusicManager : MonoBehaviour
{

	public AudioSource dronePrefab;

	private AudioSource playingDrone;

    [Tooltip("Drone sound clips that we can play")]
    public AudioClip[] droneSounds;

    [Tooltip("Minimum time between drones that we can play.")]
    public float droneTimeMinimum;

	[Tooltip("Upper bound time between drones playing.")]
	public float droneTimeMaximum;

	private float nextDrone;

	public static bool weWantMusic = false;

	[Tooltip("The intense music clip")]
	public AudioClip intenseMusic;

	private bool isDroneMusic = false;

	private void Start()
	{
		SceneManager.activeSceneChanged += (oldScene, newScene) =>
		{
			switch (newScene.name)
			{
				case "depths_02":
				case "Credits":
				case "Introduction":
					weWantMusic = false;
					break;
				case "depths_03":
					weWantMusic = true;
					break;
			}
		};
	}

	private void Update()
	{
		if (weWantMusic)
		{
			if (isDroneMusic)
				return;
			// Fade out the drone
			if (playingDrone != null)
			{
				playingDrone.volume -= Time.deltaTime;
				if (playingDrone.volume <= 0)
				{
					Destroy(playingDrone.gameObject);
					playingDrone = null;
				}
			}
			else
			{
				playingDrone = Instantiate(dronePrefab);
				playingDrone.clip = intenseMusic;
				playingDrone.Play();
				DontDestroyOnLoad(playingDrone);
				isDroneMusic = true;
			}
			// Fade in the music
			return;
		}
		if (isDroneMusic)
		{
			if (playingDrone != null)
			{
				playingDrone.volume -= Time.deltaTime;
				if (playingDrone.volume <= 0)
				{
					Destroy(playingDrone.gameObject);
					playingDrone = null;
					isDroneMusic = false;
				}
			}
			return;
		}
		if (Time.time > nextDrone)
		{
			if (playingDrone == null)
			{
				playingDrone = Instantiate(dronePrefab);
				playingDrone.clip = droneSounds[Random.Range(0, droneSounds.Length)];
				playingDrone.Play();
				DontDestroyOnLoad(playingDrone);
			}
			else if (!playingDrone.isPlaying)
			{
				Destroy(playingDrone.gameObject);
				playingDrone = null;
				nextDrone = Time.time + Random.Range(droneTimeMinimum, droneTimeMaximum);
			}
		}
	}

}
