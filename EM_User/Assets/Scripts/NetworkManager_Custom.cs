using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.NetworkSystem;

public class NetworkManager_Custom : NetworkManager {

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void SetupSimulation()
	{
		singleton.playerPrefab = spawnPrefabs[0]; // Set to Admin
		SetIPadress ();
		SetPort ();

		Debug.Log (singleton.networkAddress);
		Debug.Log (singleton.networkPort);

		singleton.StartHost ();
	}

	void JoinSimulation()
	{
		SetIPadress ();
		SetPort ();

		Debug.Log (singleton.networkAddress);
		Debug.Log (singleton.networkPort);

		singleton.StartClient();
	}

	void SetPort()
	{
		int port = 7777;
		if (GameObject.Find ("Port").transform.Find("Text").GetComponent<Text>().text != "") {
			port = int.Parse (GameObject.Find ("Port").transform.Find("Text").GetComponent<Text>().text);
		}
		singleton.networkPort = port;
	}

	void SetIPadress()
	{
		string ip = GameObject.Find ("IP").transform.Find ("Text").GetComponent<Text> ().text;
		if (ip == "") {
			ip = "127.0.0.1";
		}

		singleton.networkAddress = ip;
	}

	void ExitProgram()
	{
		Application.Quit ();
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (SceneManager.GetActiveScene().name == "Menu") 
		{

			if (!PlayerPrefs.HasKey ("identification")) // generate own identification for each version of app
			{
				PlayerPrefs.SetString ("identification", Random.Range (0, 999) + "." + Random.Range (0, 999) + "." + Random.Range (0, 999) + "." + Random.Range (0, 999));
				Debug.Log ("Generating : identification" + PlayerPrefs.GetString ("identification"));
			}
			SetupMenu ();

		}
		if (SceneManager.GetActiveScene ().name == "Game") {
			SetupGame ();
		}

	}

	void SetupMenu() // Add listeners to all buttons
	{
		GameObject.Find ("HostButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("HostButton").GetComponent<Button> ().onClick.AddListener (SetupSimulation);

		GameObject.Find ("JoinButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("JoinButton").GetComponent<Button> ().onClick.AddListener (JoinSimulation);

		GameObject.Find ("ExitButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ExitButton").GetComponent<Button> ().onClick.AddListener (ExitProgram);
	}

	void SetupGame() // Add listeners to all buttons
	{
		GameObject.Find ("DisconnectButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("DisconnectButton").GetComponent<Button> ().onClick.AddListener (NetworkManager.singleton.StopHost);

	}
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		base.OnServerAddPlayer (conn, playerControllerId);
		singleton.playerPrefab = spawnPrefabs[1]; // set to player
	}
}