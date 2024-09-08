using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Messenger : NetworkBehaviour {

	public string identification = "";

	public string user = ""; //name displayed next to message

	public GameObject bar;
	public GameObject message;
		

	public virtual void Inicialize(){
		if (!isLocalPlayer) // Force other players to sync properly at first 
		{
			GameObject[] other = GameObject.FindGameObjectsWithTag ("Player");

			for (int i = 0; i < other.Length; i++) 
			{
				if (other [i].GetComponent<NetworkIdentity>().hasAuthority) 
				{
					if (other [i].GetComponent<MessengerAdmin> ()) 
					{
						other [i].GetComponent<MessengerAdmin> ().Sync ();

					}
					else if (other [i].GetComponent<Messenger> ()) 
					{
						other [i].GetComponent<Messenger> ().Sync ();

					}
				}
			}
		}
		Debug.Log ("Inicialize");
	}

	public void Start()
	{
		Inicialize ();

		if (!isLocalPlayer) {
			return;
		}
			
		//Client client = GenerateClient ();

		InputField inputField = GameObject.Instantiate (bar, GameObject.Find("Canvas").transform).transform.Find("MessageField").GetComponent<InputField>();
		inputField.onEndEdit.AddListener (delegate {OnSend (inputField);});
		Sync ();
	}

	public virtual void Sync()
	{
		identification = PlayerPrefs.GetString ("identification"); //
		user = identification; // natively messages will be sent with ID if noone logged in
		CmdServerSync (identification); // Synchronize all data from server
	}

	void OnSend(InputField input)
	{
		if (input.text != "") {
			
			if (isClearText(input.text)) {

				CmdSendMessage (input.text);
			} 
			else // It means you are calling command - not sending message to others
			{
				CommandRecognition (input.text);
			}
				
			input.text = "";
		}
	}

	void CommandRecognition(string content) // Gets whole command
	{
		string command = "";

		for (int i = 0; i < content.Length; i++) {
			if (content [i] == '/') {
				for (int a = i; content [a] != ';'; a++) {
					command = command + content [a];
				}
				break;
			}

		}
	}

	void UploadMessage(string content){
		MessengerAdmin admin = FindAdmin ();

		admin.CmdSyncTime ();
		admin.lastMessage.AuthorIdentification = this.identification;
		admin.lastMessage.Author = this.identification;
		admin.lastMessage.Content = content;
	}
		
	bool isClearText(string text) //looking for command starting with '/'
	{
		foreach(char symbol in text)
		{
			if (symbol == '/') {
				return false;
			}
		}
		return true;
	}

	public virtual MessengerAdmin FindAdmin(){
		return GameObject.Find ("Admin(Clone)").GetComponent<MessengerAdmin> ();
	}

	Client GenerateClient() // Generate Random LCinet
	{
		Client client = new Client();
		client.Identification = this.identification;
		client.Users = new string[20];
		client.Users[0] = "Player ";
		client.UsersColor = new Color[20];

		client.UsersColor [0] = new Color(Random.Range (0, 255), Random.Range (0, 255), Random.Range (0, 255), Random.Range (0, 255));

		return client;

	}
	//************** COMMANDS ******************** // Synchronizing woth server

	[Command]
	void CmdSendMessage(string content){
		
		UploadMessage (content); // Upload it on server
		RpcReceiveMessage (this.identification, content); // Upload it on all clients
	}

	[Command]
	void CmdServerSync(string ID){

		identification = ID; //
		user = identification;
		RpcServerSync(identification);

	}

	//************ CLIENTRPC *********************** // Synchronizing on all clients

	[ClientRpc]
	void RpcServerSync(string ID){
		
		identification = ID;
		user = identification;
	}

	[ClientRpc]
	void RpcReceiveMessage(string identification, string content) // Synchronizing message with all clients
	{
		
		GameObject message = GameObject.Instantiate (this.message);
		GameObject parent = GameObject.Find ("Content");

		UploadMessage (content);

		MessengerAdmin admin = FindAdmin ();
		message.transform.Find ("Time").GetComponent<Text> ().text = "<b>" + admin.lastMessage.Time + "</b>";
		message.transform.Find ("Text").GetComponent<Text> ().text = "<b>" + admin.lastMessage.Author + ": </b>" + admin.lastMessage.Content;
		message.transform.SetParent (parent.transform);
	}
}