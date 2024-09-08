using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MessengerAdmin : Messenger {


	public Message lastMessage; // last message - finish actuailization of it to .txt

	public Client[] clients; // stores all logins, their colors and identifications of all PCs ever joining this session

	[SyncVar]
	int minutes;

	[SyncVar]
	int hours;

	public override void Sync ()
	{
		Debug.Log ("Admin Sync...");
		base.Sync ();
		SyncClients ();
	}

	public override void Inicialize ()
	{
		if (isLocalPlayer) // Admnin staff goes here
		{
			lastMessage = new Message();
			clients = new Client[3]; // max number of players
			for(int i = 0; i < clients.Length; i++)
			{
				clients [i] = new Client ();
				clients [i].Users = new string[20];
				clients [i].UsersColor = new Color[20];
			}
			Debug.Log ("Instancing...");
		}
		Debug.Log ("AFTER Instancing...");
		base.Inicialize ();
	}

	string GetTime(){
		
		hours = System.DateTime.Now.Hour; // Synchrinze this with Admin!
		minutes = System.DateTime.Now.Minute;
		string result;

		if (hours > 9) {
			result = hours.ToString() + ":";
		}
		else {
			result = "0" + hours.ToString() + ":";
		}

		if (minutes > 9)
			result += minutes.ToString();
		else {
			result += "0" + minutes.ToString();
		}

		return result;
	}

	void SyncClients()
	{
		
		CmdSyncClientsCount (clients.Length);

		for (int i = 0; i < clients.Length; i++) 
		{
			CmdSyncClientsID (i, clients[i].Identification);
			CmdSyncClientsUsers (i, clients [i].Users);

			for (int a = 0; a < 20; a++) 
			{
				//Color color = new Color();
				CmdSyncClientsColor (i, a, clients[i].UsersColor[a]);
			}
		}
	}

	//***************** COMMANDS *************

	public void CmdSyncTime(){
		lastMessage.Time = GetTime ();
	}

	[Command]
	void CmdSyncClientsCount(int length)
	{
		for(int i = clients.Length; i != length; i++)
		{
			clients[i] = new Client();
			clients [i].Users = new string[20];
			clients[i].UsersColor = new Color[20];
		}
		RpcSyncClientsCount (clients.Length);
	}

	[Command]
	void CmdSyncClientsID(int index, string ID){
		clients [index].Identification = ID;
		RpcSyncClientsID (index, ID);
		
	}

	void CmdSyncClientsUsers(int index, string[] names){
		clients [index].Users = names;
		RpcSyncClientsIdentification (index, names);
	}

	[Command]
	void CmdSyncClientsColor(int clientIndex, int userIndex, Color color){

		clients [clientIndex].UsersColor [userIndex] = color;
		RpcSyncClientsColor (clientIndex, userIndex, color);
	}
		
	//**************** CLIENTRPC **************

	[ClientRpc]
	void RpcSyncClientsCount(int length)
	{
		for(int i = clients.Length; i != length; i++)
		{
			clients[i] = new Client();
			clients [i].Users = new string[20];
			clients[i].UsersColor = new Color[20];
		}
	}

	[ClientRpc]
	void RpcSyncClientsID(int index, string ID){

		clients [index].Identification = ID;

	}

	[ClientRpc]
	void RpcSyncClientsIdentification (int index, string[] names)
	{
		clients [index].Users = names;
	}

	[ClientRpc]
	void RpcSyncClientsColor(int clientIndex, int userIndex, Color color){

		clients [clientIndex].UsersColor [userIndex] = color;
	}

}
