using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ClientHUD : MonoBehaviour
{

    public GameObject connectToServer, disConnect, addressPanel, connecting, menuCam, disConnectMessage;
    public InputField portText, ipText, passwordText;
    public Text connectingText;

    private NetworkManager manager;
    private float connectingTimer, connectionFaileTimer;
    private bool connected;

    // Use this for initialization
    void Start()
    {
        if (!manager)
            manager = GetComponent<NetworkManager>();

        //Comprobar si se ha guardado la información del servidor.
        if (PlayerPrefs.HasKey("nwPortC"))
        {
            manager.networkPort = Convert.ToInt32(PlayerPrefs.GetString("nwPortC"));
            portText.text = PlayerPrefs.GetString("nwPortC");
        }
        if (PlayerPrefs.HasKey("IPAddressC"))
        {
            manager.networkAddress = PlayerPrefs.GetString("IPAddressC");
            ipText.text = PlayerPrefs.GetString("IPAddressC");
        }
    }

    void Update()
    {
        if (!connected)
        {
            //muestra el mensaje que no se ha podido conectar después de un cierto tiempo en espera de conectarse.
            if (connectingTimer > 0)
                connectingTimer -= Time.deltaTime;
            else
            {
               // manager.StopClient();
                connectingText.text = "Failed To Connect !!";
                if (connectionFaileTimer > 0)
                    connectionFaileTimer -= Time.deltaTime;
                else connecting.SetActive(false);
            }
        }
    }

    public void ConnectToServer()
    {
        if (ipText.text != "" && portText.text != "")//¿está llena la información?
        {
            connected = false;
            disConnectMessage.SetActive(false);
            connectingText.text = "Connecting !!";
            connecting.SetActive(true);
            connectingTimer = 8;//¿Cuánto tiempo nos intenta conectarse hasta que el mensaje de error aparece.
            connectionFaileTimer = 2;//Cuánto tiempo se muestra el mensaje Fail.
            manager.networkAddress = ipText.text;
            manager.networkPort = Convert.ToInt32(portText.text);
            PlayerPrefs.SetString("IPAddressC", ipText.text);//guardando la ip ingresada
            PlayerPrefs.SetString("nwPortC", portText.text);//guardando el puerto ingresado

            manager.StartClient();
        }
    }

    //called by the CustomNetworkManager.
    public void ConnectSuccses()
    {
        connected = true;
        connecting.SetActive(false);
        disConnect.SetActive(true);
        connectToServer.SetActive(false);
        addressPanel.SetActive(false);
        //menuCam.SetActive(false);   //Si su jugador tiene una cámara en él éste debe ser apagado al entrar en el juego/vestíbulo.
    }

    public void ButtonDisConnect()
    {
        DisConnect(false);
    }

    public void DisConnect(bool showMessage)
    {
        if (showMessage)
            disConnectMessage.SetActive(true);
        connectToServer.SetActive(true);
        disConnect.SetActive(false);
        addressPanel.SetActive(true);
        //menuCam.SetActive(true);  //turn the camera on again when returning to menu scene.
        manager.StopClient();
    }
}
