using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPTestClient : MonoBehaviour
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion
    // Use this for initialization 	

    GameObject player1;
    GameObject player1spawn;
    public bool temJogador=false;
    GameObject player2spawn;
    


    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ConnectToTcpServer();

            if (!temJogador)
            {
                player1spawn = GameObject.Find("Spawn1");
                player1 = (GameObject)Instantiate(Resources.Load("Jogador1"),player1spawn.transform);
            }
            else
            {
                player2spawn = GameObject.Find("Spawn2");
                Vector3 local = new Vector3(player2spawn.transform.position.x, player2spawn.transform.position.y,
               player2spawn.transform.position.z);
                player1 = (GameObject)Instantiate(Resources.Load("Jogador1"),local,Quaternion.Euler(0f, 180f, 0f));
            }
            
            


        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SendMessage();
        }
            
        
    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        
            try
            {


                clientReceiveThread = new Thread(ListenForData);
                clientReceiveThread.Start();
                
            
                
                if (!temJogador) Debug.Log("Jogador 1 Conectado!!");
                else Debug.Log("Jogador 2 Conectado");
            }
            catch (Exception e)
            {
                Debug.Log("On client connect exception " + e);
            }
        
       
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        
        try
        {
            
            socketConnection = new TcpClient("localhost", 8052);
            Byte[] bytes = new Byte[1024];
            Debug.Log("TAMO AQUI");
            while (true)
            {
                
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log(serverMessage);
                        temJogador = Boolean.Parse(serverMessage);
                        Console.WriteLine(temJogador);
                        
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    private void SendMessage()
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
        
            MovimentaChar obj = player1.GetComponent<MovimentaChar>();
            
            

            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = obj.transform.position.x +"|"+obj.transform.position.y+"|"+obj.vida; //enviando posições
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}
