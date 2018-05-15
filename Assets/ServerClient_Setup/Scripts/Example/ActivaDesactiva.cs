using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ActivaDesactiva : NetworkBehaviour {

    //A efectos de ejemplo
    public InputField inputField;
    public Renderer cubeMat;
    public bool isGreen;
    

    //Configurando el SyncVar.
    [SyncVar(hook = "OnVarSynced")]//Esto debe estar por encima de la variable que desea sincronizar. (puedes nombrar el "Hook" todo lo que quieras, pero ten en cuenta que este es el nombre de una función que se llama a los clientes cuando el SyncVar cambia en el servidor)
    public string varToSync; //Si esto cambia en el servidor un "mensaje" se enviará a los clientes automáticamente. 
                             //(si cambia esto en un cliente sólo cambiará en ese cliente, no se enviará a ninguna parte).

    //la función "Hook" se ejecuta en los clientes cuando se recibe un cambio de SyncVar.
    //el valor real de SyncVar en los clientes no se cambia cuando se recibe un mensaje de cambio desde el servidor.

    //Es más como un disparador con un "mensaje/variable" adjunta,
    //Se permitirá a los clientes conocer "varToSync" ha cambiado en el servidor y decirles que el nuevo valor que tiene actualmente en el servidor.

    //La cadena "syncedVar" es el nuevo valor de "varToSync" que ahora tiene en el servidor.
    public void OnVarSynced(string syncedVar)//Esta es la función "Hook" que se llama a los clientes cuando el Syncvar cambia en el servidor.
    {
        //Todo aquí se ejecutará en los clientes.
        inputField.text = syncedVar;

        // puede utilizar la nueva variable de "syncedVar" en la función "Hook" inmediatamente o, 
        // si desea que el "varToSync" Mantenga el mismo valor que el que está en el servidor (por lo que puede usarlo más tarde en la función de actualización o en otro lugar), 
        // tendrá que c colgarlo en los clientes en la función "Hook" justo aquí.
        varToSync = syncedVar;

        //Si desea hacer algo en el reproductor local sólo (el que está controlando)
        if (isLocalPlayer)
        {

        }

        //Si quieres hacer algo sobre los jugadores remotos únicamente (los copys de usted en los otros clientes).
        if (!isLocalPlayer)
        {

        }
    }

    void Update()
    {
        // para este ejemplo en el servidor el Syncvar "varToSync" es el texto de un campo de la entrada. 
        // si el nuevo texto es introducido los cambios de Syncvar y se envía a los clientes.
        if (isServer)
            varToSync = inputField.text;
    }

    //Esto se utiliza para que un jugador local haga algo en el servidor. (dígale al servidor que haga algo)
    [Command]//Enviar desde el cliente local (llamado en el cliente local) ejecutado (recibido) en el servidor.
    void CmdChangeColor()
    {
        //todo aquí será ejecutado en el servidor.
        ChangeColor();

        // como se trata de enviar desde un cliente local, ese cliente básicamente tiene "acceso" a todo lo que hay en el servidor, 
        // y puede decirle/pedir al servidor que haga todo lo que sólo se puede hacer al lado del servidor.
    }

    //Esto se utiliza para que el servidor haga algo en los clientes. (Indicar a los clientes que hagan algo)
    [ClientRpc]//Enviar desde el servidor (llamado en el servidor) ejecutado (recibido) en los clientes
    void RpcChangeColor()
    {
        //todo lo que hay aquí será ejecutado en todos los clientes (remotos y locales).
        ChangeColor();

        //como se envía desde el servidor a todos los clientes, el servidor básicamente le dice a los clientes que hagan algo de su lado.

        //Si desea hacer algo en el reproductor local sólo (el que está controlando)

        if (isLocalPlayer)
        {
            //Creo que se está implementando un "TargetRpc" para enviar llamadas RPC a un solo cliente, pero por ahora esto tendrá que hacer.
        }

        //Si desea hacer algo en los reproductores remotos sólo (las copias de usted en los otros clientes)
        if (!isLocalPlayer)
        {

        }
    }

    /*
        La combinación de una llamada RPC con un comando le permite hacer algo en otros clientes desde un cliente local. 
        enviar una llamada RPC al servidor que tiene un comando en él para decirle a los clientes que hagan algo.

        ---

        [ClientRpc]  //enviar desde cliente local al servidor.
        void RpcSendToServer()
        {
            // Hacer algo en el servidor. 
            // En este caso, enviar un comando a los clientes
            CmdSendToClients();
        }

        [Command]  //Enviar desde el servidor a los clientes
        void CmdSendToClients()
        {
            //Haz algo con los clientes.
        }

        ---

        Esto también podría hacerse cambiando un SyncVar desde un cliente local

        [ClientRpc]  //enviar desde cliente local al servidor.
        void RpcSendToServer()
        {
            //Cambiar el SyncVar en el servidor, y esto activará la función "Hook" en todos los clientes.         
        }
    */


    void ChangeColor()
    {
        if (isGreen)
            cubeMat.material.color = Color.yellow;
        else cubeMat.material.color = Color.green;

        isGreen = !isGreen;
    }


    //No se puede poner comandos y RpcCalls sobre un botón (no funciona).
    //Por lo tanto, ponemos estas funciones en los botones para activar el comando y RPC.
    public void ButtonCommand()
    {
        if(!isServer) //Si llama a un comando desde el servidor obtendrá un error. (los comandos se utilizan para permitir que los clientes hagan algo en el servidor)
            CmdChangeColor();
    }

    public void ButtonRPC()
    {
        if(isServer) //Si llama a un RPC desde un cliente, obtendrá un error. (RPC se utilizan para permitir que el servidor haga algo en los clientes)

            RpcChangeColor();
    }
}
