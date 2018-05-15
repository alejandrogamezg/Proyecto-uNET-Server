using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using System;

public class CustomNetworkManager : NetworkManager
{
    public Text clientsInfoText;
    // public ClientHUD clientHudScript;
    public ServerHUD serverHudScript;

    private int connectedClients = 0;

    // [HideInInspector]
    // public string serverPassword;

    //--------------------------------------------------------------------------------------
    //--------------------- SERVIDOR -------------------------------------------------------
    public override void OnStartServer()
    {
        base.OnStartServer();
        // RegisterServerHandles();

        // serverPassword = serverHudScript.passwordText.text;
        connectedClients = 0;
        clientsInfoText.text = "Clientes conectados: " + connectedClients;
        Debug.Log("Servidor iniciado :) ...");
    }

    //Mantener un seguimiento de los clientes que se conecten.
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        connectedClients += 1;
        clientsInfoText.text = "Clientes conectados : " + connectedClients;
        Debug.Log("Servidor conectado :) ...");

        //Enviando información de contraseña al cliente.
        // StringMessage msg = new StringMessage(serverPassword);
        // NetworkServer.SendToClient(conn.connectionId, MsgType.Highest + 1, msg);
        // Debug.Log("Un cliente conectado al servidor: " + conn);
    }

    //Mantener un seguimiento de los clientes a la desconexión.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        connectedClients -= 1;
        clientsInfoText.text = "Clientes conectados: " + connectedClients;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
         Debug.Log("Servidor detenido :( ...");
    }

    public override void OnServerError(NetworkConnection conn, int errorCode) {

        Debug.Log("Error de red en servidor: " + (NetworkError)errorCode);

    }


    //--------------------------------------------------------------------------------------
    //--------------------- CLIENTE -------------------------------------------------------
    // public override void OnStartClient(NetworkClient client)
    // {
    //     base.OnStartClient(client);
    //     RegisterClientHandles();
    // }

    // public override void OnClientConnect(NetworkConnection conn)
    // {
    //     base.OnClientConnect(conn);
    //     clientHudScript.ConnectSuccses();
    // }

    // //Cuando el cliente recibe la información de contraseña del servidor.
    // public void OnReceivePassword(NetworkMessage netMsg)
    // {
    //     //Lea la contraseña del servidor.
    //     var msg = netMsg.ReadMessage<StringMessage>().value;
    //     //serverPassword = msg;
    //     if (msg != clientHudScript.passwordText.text)
    //         clientHudScript.DisConnect(true);
    // }

    // public override void OnClientDisconnect(NetworkConnection conn)
    // {
    //     base.OnClientDisconnect(conn);
    //     clientHudScript.DisConnect(false);
    // }

    //--------------------------------------------------------------------------------------
    //--------------------- MENSAJES -------------------------------------------------------
    //Mensajes que se deben registrar en el inicio del servidor y del cliente.
    // void RegisterServerHandles()
    // {
    //     NetworkServer.RegisterHandler(MsgType.Highest + 1, OnReceivePassword);
    // }

    // void RegisterClientHandles()
    // {
    //     client.RegisterHandler(MsgType.Highest + 1, OnReceivePassword);
    // }
}
