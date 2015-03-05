﻿/*
 *  Copyright 2015 Sebastian Heinz <sebastian.heinz.gt@googlemail.com>
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */
namespace MarrySocket.MClient
{
    using MarrySocket.MExtra.Logging;
    using System;
    using System.Net;
    using System.Net.Sockets;

    public class MarryClient
    {
        private SocketManager socketManager;

        private bool isValidConfiguration;

        public MarryClient(ClientConfig clientConfig)
        {
            this.ClientConfig = clientConfig;
            this.Logger = this.ClientConfig.Logger;
        }

        public ServerSocket ServerSocket { get; private set; }
        public ClientConfig ClientConfig { get; private set; }
        public Logger Logger { get; private set; }
        public bool IsConnected { get { return this.ClientConfig.IsConnected; } }

        public event EventHandler<ConnectedEventArgs> Connected
        {
            add { this.ClientConfig.Connected += value; }
            remove { this.ClientConfig.Connected -= value; }
        }

        public event EventHandler<ReceivedPacketEventArgs> ReceivedPacket
        {
            add { this.ClientConfig.ReceivedPacket += value; }
            remove { this.ClientConfig.ReceivedPacket -= value; }
        }

        public event EventHandler<DisconnectedEventArgs> Disconnected
        {
            add { this.ClientConfig.Disconnected += value; }
            remove { this.ClientConfig.Disconnected -= value; }
        }


        private bool SanityCheck()
        {
            //TODO check clientConfig
            return true;
        }

        private Socket CreateSocket()
        {
            Socket socket;
            if (this.ClientConfig.ServerIP.AddressFamily == AddressFamily.InterNetworkV6)
            {
                this.Logger.Write("Creating IPv4 Socket...", LogType.CLIENT);
                socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                this.Logger.Write("Creating IPv6 Socket...", LogType.CLIENT);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            return socket;
        }

        public void Connect()
        {
            if (!this.ClientConfig.IsConnected)
            {
                try
                {
                    this.isValidConfiguration = SanityCheck();
                    if (this.isValidConfiguration)
                    {
                        this.ServerSocket = new ServerSocket(this.CreateSocket(), this.Logger, this.ClientConfig.Serializer);
                        this.socketManager = new SocketManager(this.ClientConfig, this.ServerSocket);

                        IPEndPoint remoteEP = new IPEndPoint(this.ClientConfig.ServerIP, this.ClientConfig.ServerPort);
                        this.ServerSocket.Socket.Connect(remoteEP);
                        this.socketManager.Start();
                        this.ClientConfig.IsConnected = true;
                        this.ClientConfig.OnConnected(this.ServerSocket);
                        this.Logger.Write("Client connected", LogType.CLIENT);
                    }
                    else
                    {
                        this.Logger.Write("Bad Configuration", LogType.ERROR);
                    }

                }
                catch (SocketException socketException)
                {
                    this.Disconnect(socketException.SocketErrorCode.ToString());
                }
                catch (Exception ex)
                {
                    this.Disconnect(ex.ToString());
                }
            }
        }

        public void Disconnect(string reason)
        {
            this.socketManager.Stop();
            this.ServerSocket.Close();
            this.ClientConfig.IsConnected = false;
            this.ClientConfig.OnDisconnected(this.ServerSocket);
            this.Logger.Write("Client disconnected: {0}", reason, LogType.CLIENT);
        }



    }
}
