using System;
using System.Timers;
using SharedClasses.Message;

namespace ChatClient.Services
{
    /// <summary>
    /// A Client could spam <see cref="T" /> messages to the server.
    /// This limits the amount of times able to do so. Messages will be lost if client sends too quickly.
    /// </summary>
    internal sealed class MessageThroughputLimiter<T> : IDisposable where T : class, IMessage
    {
        private readonly object queueLock = new object();
        private readonly Timer userTypingIntervalTimer = new Timer();
        private T lastMessageSent;

        /// <summary>
        /// Sets the time limit between the number of messages that can be sent and initialises the
        /// <see cref="DeferredSendLastMessage" /> invoker.
        /// </summary>
        /// <param name="minimumMillisecondsAllowedBetweenMessages">
        /// The minimum time allowed between <see cref="T" /> messages
        /// being sent.
        /// </param>
        public MessageThroughputLimiter(long minimumMillisecondsAllowedBetweenMessages)
        {
            userTypingIntervalTimer.Interval = minimumMillisecondsAllowedBetweenMessages;
            userTypingIntervalTimer.Elapsed += OnMinimumTimeElapsed;
            userTypingIntervalTimer.Start();
        }

        /// <summary>
        /// Clears the timer.
        /// </summary>
        public void Dispose()
        {
            userTypingIntervalTimer.Dispose();
        }

        /// <summary>
        /// Invokes when the last message requested is allowed to be sent.
        /// </summary>
        public event EventHandler<T> DeferredSendLastMessage;

        /// <summary>
        /// Calculates whether the client is allowed to send a <see cref="T" />.
        /// </summary>
        public void QueueMessageRequest(T message)
        {
            lock (queueLock)
            {
                lastMessageSent = message;
            }
        }

        private void OnMinimumTimeElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (lastMessageSent != null)
            {
                EventHandler<T> deferredSendAllowedCopy = DeferredSendLastMessage;

                if (deferredSendAllowedCopy != null)
                {
                    deferredSendAllowedCopy(null, lastMessageSent);
                }
            }

            lastMessageSent = null;
        }
    }
}