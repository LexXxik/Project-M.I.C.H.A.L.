//Custom class to hold message data
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message 
{
	public string Author; // Shown name of writter
	public string AuthorIdentification; // Address of PC writting message
	public string Time; // At what time it was sent
	public string Content; // Content of message
}