using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class ServerHUD : MonoBehaviour {

    public GameObject stopServer, startServer, resetSettings, getIP, checking, clientsInfo;
    public Text serverInfoText, portPlaceholderText, paswPlaceholderText, clientsInfoText;
    // public InputField portText, passwordText, maxConnText;
    public InputField portText, maxConnText;


    private NetworkManager manager;
    private bool noConnection, setText, checkIP;
    private string externalip="?", localIP="?";
    private int maximumConnections;

    // Use this for initialization
    void Start () {
        if (!manager)
            manager = GetComponent<NetworkManager>();
        
        //Comprobando si hemos guardado información del servidor y rellenando los campos de texto.
        if (PlayerPrefs.HasKey("nwPortS"))
        {
            manager.networkPort = Convert.ToInt32(PlayerPrefs.GetString("nwPortS"));
            portPlaceholderText.text = manager.networkPort.ToString();
        }
        if (PlayerPrefs.HasKey("IPAddressS"))
        {
            externalip = PlayerPrefs.GetString("IPAddressS");
            localIP = PlayerPrefs.GetString("LocalIP");
            getIP.GetComponentInChildren<Text>().text = "Server IP Address\nExternal :" + externalip + "\nLocal :" + localIP;
        }
        // if (PlayerPrefs.HasKey("Password"))
        // {
        //     passwordText.text = PlayerPrefs.GetString("Password");
        //     if (passwordText.text == "")
        //         paswPlaceholderText.text = "(not needed)";
        // }
        if (PlayerPrefs.HasKey("MaxConnections"))
        {
            maxConnText.text = PlayerPrefs.GetString("MaxConnections");
        }

        clientsInfoText = clientsInfo.GetComponentInChildren<Text>();
        setText = true;       
    } 
	
	// Update is called once per frame
	void Update () {
        
        noConnection = (manager.client == null || manager.client.connection == null ||
                     manager.client.connection.connectionId == -1);

        //Mostrar y ocultar los botones y el texto apropiados dependiendo de si el servidor está en ejecución o no.
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            if (noConnection)
            {
                stopServer.SetActive(false);
                clientsInfo.SetActive(false);

                if (setText)
                {
                    serverInfoText.color = Color.red;
                    serverInfoText.text = "Server Not Running !";
                    setText = false;
                }
            }
        }
        else
        {
            if (setText)
            {
                serverInfoText.color = new Color(0.2f, 0.6f, 0.2f, 1f);
                // string pw="";               
                // if (passwordText.text == "")               
                //     pw = "(no password)";                
                // else pw = passwordText.text;

                string maxConn = "";
                if (maxConnText.text == "")
                    maxConn = "8";
                else maxConn = maxConnText.text;

                serverInfoText.text = "Servidor ejecutando !\n" +"\nIP Address\nExternal : " +externalip+"\nLocal :"+localIP+"\n\nServer Port : " + manager.networkPort+"\nMax Connections : "+ maxConn;
                // "+localIP+"\n\nServer Port : " + manager.networkPort+"\nPassword : "+pw+"\nMax Connections : "+ maxConn;
                setText = false;
            }
        }
    }

    //Apagar el servidor.
    public void StopHostCustom()
    {
        startServer.SetActive(true);
        portText.transform.parent.gameObject.SetActive(true);
        resetSettings.SetActive(true);
        getIP.SetActive(true);
        setText = true;
        manager.StopHost();
    }

    public void StartServerCustom()
    {
        //configuración del puerto de administradores de red que se usará.
        if (portText.text == "")//No hemos establecido un número de puerto ?
        {
            if (PlayerPrefs.HasKey("nwPortS"))//¿hemos guardado uno anterior?
            {
                manager.networkPort = Convert.ToInt32(PlayerPrefs.GetString("nwPortS"));
            }
            else//Si no es así, utilice el puerto predeterminado.
            {
                manager.networkPort = 7777;
                portPlaceholderText.text = manager.networkPort.ToString()+"(Default)";
            }
        }
        else
        {
            PlayerPrefs.SetString("nwPortS", portText.text);//guardamos el puerto que estamos usando.     
            manager.networkPort = Convert.ToInt32(portText.text);
            portPlaceholderText.text = manager.networkPort.ToString();
        }

        // PlayerPrefs.SetString("Password", passwordText.text);//guardar los servidores de contraseña.
        PlayerPrefs.SetString("MaxConnections", maxConnText.text.ToString());
        //Mostrar y ocultar los botones y el texto apropiados. 
        resetSettings.SetActive(false);
        portText.transform.parent.gameObject.SetActive(false);
        getIP.SetActive(false);
        startServer.SetActive(false);
        stopServer.SetActive(true);
        clientsInfo.SetActive(true);
        setText = true;

        if (maxConnText.text != "")
        {
            maximumConnections = Convert.ToInt32(maxConnText.text);
        }
        else maximumConnections = 8;
        manager.maxConnections = maximumConnections;

        manager.StartServer();

        //var config = new ConnectionConfig();
        //config.AddChannel(QosType.Reliable);
        //config.AddChannel(QosType.Unreliable);

        //manager.StartServer(config, maximumConnections);
    }

    public void ResetToDefault()
    {
        //borrando toda la información guardada y reconfigurando para usar los valores predeterminados.
        PlayerPrefs.DeleteKey("IPAddressS");
        getIP.GetComponentInChildren<Text>().text = "Find Server IP Address.";
        externalip = "?";
        PlayerPrefs.DeleteKey("nwPortS");
        portPlaceholderText.text = "7777(Default)";
        portText.text = "";
        PlayerPrefs.DeleteKey("LocalIP");
        localIP = "?";
        // PlayerPrefs.DeleteKey("Password");
        // paswPlaceholderText.text = "(not needed)";
        // passwordText.text = "";
        PlayerPrefs.DeleteKey("MaxConnections");
        maxConnText.text = "";
    }

    //Encontrar las direcciones IP de los servidores.
    public void GetIP()
    {
        getIP.GetComponentInChildren<Text>().text = "If this takes too long\nClick again.";
        StartCoroutine(GetPublicIP());//iniciando la comprobacion
        checking.SetActive(true);
        Debug.Log("Obteniendo IP Local...");
    }

    IEnumerator GetPublicIP()
    {
        WWW www = new WWW("http://checkip.dyndns.org");//el sitio web a utilizar para encontrar su IP externa, utilizar cualquier "encontrar mi IP" sitio que desee.
        yield return www;//Espere hasta que obtenga una respuesta.
        if (www.error==null)
        {
            //Filtrar el mensaje de respuesta a la dirección IP.
            string response = www.text;
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            externalip = a4;//Obtenemos nuestra IP Externa

            // Obtener la IP del PC en el que se está ejecutando el servidor.
            localIP = Network.player.ipAddress;

            getIP.GetComponentInChildren<Text>().text = "Server IP Address\nExternal :" + externalip+"\nLocal :"+localIP;
            //guardando las direcciones IP
            PlayerPrefs.SetString("IPAddressS", externalip);
            PlayerPrefs.SetString("LocalIP", localIP);
            checking.SetActive(false);
            Debug.Log("Obteniendo IP Externa...");
        }
        else
        {
            getIP.GetComponentInChildren<Text>().text = "Someting went wrong\nPlease try again";
            checking.SetActive(false);
        }
    }
}
