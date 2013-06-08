// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueManager.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Implementation of the IQueueManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.RabbitMQ
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using global::RabbitMQ.Client;

    using global::RabbitMQ.Client.MessagePatterns;

    using STrackerBackgroundWorker.RabbitMQ.Core;

    /// <summary>
    /// The queue manager.
    /// </summary>
    public class QueueManager : IQueueManager
    {
        /// <summary>
        /// The queue name.
        /// </summary>
        private const string QueueName = "Queue";

        /// <summary>
        /// The conn factory.
        /// </summary>
        private readonly ConnectionFactory connFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManager"/> class.
        /// </summary>
        /// <param name="connFactory">
        /// The conn factory.
        /// </param>
        public QueueManager(ConnectionFactory connFactory)
        {
            this.connFactory = connFactory;
        }

        /// <summary>
        /// The push.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        public void Push(Message msg)
        {
            // Open up a connection and a channel (a connection may have many channels)
            using (var conn = this.connFactory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                // Serialize the message to text.
                var stringWriter = new StringWriter();
                new XmlSerializer(msg.GetType()).Serialize(stringWriter, msg); 

                // the data put on the queue must be a byte array
                var data = Encoding.UTF8.GetBytes(stringWriter.ToString());

                // ensure that the queue exists before we publish to it
                channel.QueueDeclare(QueueName, false, false, false, null);

                // publish to the "default exchange", with the queue name as the routing key
                channel.BasicPublish(string.Empty, QueueName, null, data);
            }
        }

        /// <summary>
        /// The pull.
        /// </summary>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        public Message Pull()
        {
            using (var conn = this.connFactory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                // ensure that the queue exists before we access it
                channel.QueueDeclare(QueueName, false, false, false, null);

                // subscribe to the queue
                var subscription = new Subscription(channel, QueueName);

                // this will block until a messages has landed in the queue
                var message = subscription.Next();

                // deserialize the message body
                var text = Encoding.UTF8.GetString(message.Body);

                // Deserialize the text to message object.
                var stringReader = new StringReader(text);
                var msg = (Message)new XmlSerializer(typeof(Message)).Deserialize(stringReader);

                // ack the message, ie. confirm that we have processed it
                // otherwise it will be requeued a bit later
                subscription.Ack(message);

                return msg;
            }
        }
    }
}