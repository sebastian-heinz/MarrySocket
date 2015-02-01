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
namespace MarrySocket.MServer
{
    using MarrySocket.MBase;
    using MarrySocket.MExtra.Logging;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    public class PacketManager
    {
        private EntitiesContainer entitiesContainer;
        private Logger serverLog;
        private event EventHandler<ReceiveObjectEventArgs> receivedObjectPacket;

        public PacketManager(EntitiesContainer entitiesContainer)
        {
            this.entitiesContainer = entitiesContainer;
            this.serverLog = this.entitiesContainer.ServerLog;
            this.receivedObjectPacket = this.entitiesContainer.ReceivedObjectPacket;
        }

        public void Handle(ClientSocket clientSocket, Packet packet)
        {
            IFormatter formatter = new BinaryFormatter();
            object myObject = null;
            try
            {
                myObject = (object)formatter.Deserialize(packet.GetPacketForReading());
            }
            catch (SerializationException e)
            {
                this.serverLog.Write("Failed to serialize. Reason: {0}" , e.Message, LogType.ERROR);
            }

            if (myObject != null)
            {
                this.receivedObjectPacket(this, (new ReceiveObjectEventArgs(packet.PacketId, clientSocket, myObject)));
                this.serverLog.Write("Client[{0}]: Handled Packet: {0}", clientSocket.Id, packet.PacketId, LogType.PACKET);
            }
        }

    }
}
