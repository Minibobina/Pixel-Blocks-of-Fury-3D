using UnityEngine;
using System;
using System.Collections;

public class MessageEventArgs : EventArgs 
{
	public readonly string message;

	public MessageEventArgs (string m)
	{
		message = m;
	}
}
