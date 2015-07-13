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
namespace SvrKit.Networking.MarrySocket.MClient
{
    using SvrKit.Logging;
    using SvrKit.Networking.MarrySocket.MBase;
    using SvrKit.Networking.MarrySocket.Serialization;
    using System.Net.Sockets;

    /// <summary>
    /// TODO SUMMARY
    /// </summary>
    public class ServerSocket : BaseSocket
    {
        /// <summary>
        /// TODO SUMMARY
        /// </summary>
        public ServerSocket(Socket socket, Logger logger, ISerialization serializer)
            : base(socket, logger, serializer)
        {

        }

        internal void Close()
        {
            base.Disconnect();
        }

        /// <summary>
        /// TODO SUMMARY
        /// </summary>
        protected override void Error(string error)
        {
            this.logger.Write(error, LogType.ERROR);
        }
    }
}