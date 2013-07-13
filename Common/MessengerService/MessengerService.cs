using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MPDisplay.Common
{
    /// <summary>
    /// Provides loosely-coupled messaging between
    /// various objects.  All references to objects
    /// are stored weakly, to prevent memory leaks.
    /// </summary>
    public class MessengerService<MessageType>
    {
        #region Fields

        readonly MessageToActionsMap<MessageType> _messageToActionsMap = new MessageToActionsMap<MessageType>();

        #endregion

        #region Constructor

        public MessengerService()
        {
        }

        #endregion

        #region Register

        /// <summary>
        /// Registers a callback method, with no parameter, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register(MessageType message, Action callback)
        {
            this.Register(message, callback, null);
        }

        /// <summary>
        /// Registers a callback method, with a parameter, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register<T>(MessageType message, Action<T> callback)
        {
            this.Register(message, callback, typeof(T));
        }

        /// <summary>
        /// Registers a callback method, with 2 parameters, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register<T, U>(MessageType message, Action<T, U> callback)
        {
            this.Register(message, callback, typeof(T), typeof(U));
        }

        /// <summary>
        /// Registers a callback method, with 3 parameters, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register<T, U, V>(MessageType message, Action<T, U, V> callback)
        {
            this.Register(message, callback, typeof(T), typeof(U), typeof(V));
        }

        /// <summary>
        /// Registers the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <exception cref="System.ArgumentException">'message' cannot be null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">callback</exception>
        private void Register(MessageType message, Delegate callback, params Type[] parameterType)
        {
            if (message == null)
                throw new ArgumentException("'message' cannot be null or empty.");

            if (callback == null)
                throw new ArgumentNullException("callback");

            _messageToActionsMap.AddAction(message, callback.Target, callback.Method, parameterType);
        }

        /// <summary>
        /// Deregisters the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="owner">The owner.</param>
        public void Deregister(MessageType message, object owner)
        {
            _messageToActionsMap.RemoveAction(message, owner);
        }

        #endregion

        #region NotifyListeners

        /// <summary>
        /// Notifies all registered parties that a message is being broadcasted.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="parameter">The parameter to pass together with the message.</param>
        public void NotifyListeners(MessageType message, params object[] parameters)
        {
            if (message == null)
                throw new ArgumentException("'message' cannot be null or empty.");

            Type[] registeredParameters;
            if (_messageToActionsMap.TryGetParameterTypes(message, out registeredParameters))
            {
                if (registeredParameters == null && (parameters != null && parameters.Count() > 0))
                    throw new TargetParameterCountException(string.Format("parameters not expected.", message));

                if (registeredParameters != null && (parameters == null || parameters.Count() != registeredParameters.Count()))
                    throw new TargetParameterCountException(string.Format("parameter count mismatch", message));
               
            }

            var actions = _messageToActionsMap.GetActions(message);
            if (actions != null)
            {
                if (parameters == null || parameters.Count() == 0)
                {
                    actions.ForEach(action => action.DynamicInvoke());
                }
                else
                {
                    actions.ForEach(action => action.DynamicInvoke(parameters));
                }
            }
        }

         #endregion
    }
}


