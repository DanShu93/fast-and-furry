﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJson;
using SocketIOClient;
using SocketIOClient.Messages;

public class SocketIOC : MonoBehaviour {
	Client socket;

    public string serverAddress;

    private Queue<IMessage> messageQueue;
    
    void Start ()
	{
        messageQueue = new Queue<IMessage>();

        socket = new Client(serverAddress);
		socket.On("connect", (fn) => {
			Debug.Log ("connect - socket");
			socket.Emit("add user", "user_unity");
		});
		socket.On("user joined", (data) => {
 //           Debug.Log(data);
            messageQueue.Enqueue(data);
        });
        socket.On("set vector2D", (data) => {
//            Debug.Log(data);
            messageQueue.Enqueue(data);
        });
        socket.Error += (sender, e) => {
	//		Debug.Log("socket Error: " + e.Message.ToString ());
		};
		
		socket.Connect();
//		Debug.Log("Connected to socket");
	}

    void Update()
    {
        for (int i = 0; i < messageQueue.Count; i++)
        {
            IMessage message = messageQueue.Dequeue();

            JsonObject args = (JsonObject) message.Json.args[0];

            switch (message.Json.name)
            {
                case "user joined":
                    GetComponent<GameController>().spawnPlayer( (string)args.Values.ElementAt(0));
                    break;
                case "set vector2D":

                    Vector2 direction = new Vector2(Convert.ToSingle(args.Values.ElementAt(0)), Convert.ToSingle(args.Values.ElementAt(1)));
                    GetComponent<GameController>().movePlayer((string)args.Values.ElementAt(2), direction);
                    break;
            }
        }
    }

    void OnDestroy() {
		socket.Close();
		Debug.Log("Disconnected socket");
	}
}