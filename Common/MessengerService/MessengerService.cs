using System;
using System.Linq;
using System.Reflection;

namespace Common.MessengerService
{
    /// <summary>
    /// Provides loosely-coupled messaging between
    /// various objects.  All references to objects
    /// are stored weakly, to prevent memory leaks.
    /// </summary>
    public class MessengerService<TMessageType>
    {
        #region Fields

        readonly MessageToActionsMap<TMessageType> _messageToActionsMap = new MessageToActionsMap<TMessageType>();

        #endregion

        #region Register

        /// <summary>
        /// Registers a callback method, with no parameter, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register(TMessageType message, Action callback)
        {
            Register(message, callback, null);
        }

        /// <summary>
        /// Registers a callback method, with a parameter, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register<T>(TMessageType message, Action<T> callback)
        {
            Register(message, callback, typeof(T));
        }

        /// <summary>
        /// Registers a callback method, with 2 parameters, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register<T, TU>(TMessageType message, Action<T, TU> callback)
        {
            Register(message, callback, typeof(T), typeof(TU));
        }

        /// <summary>
        /// Registers a callback method, with 3 parameters, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register<T, TU, TV>(TMessageType message, Action<T, TU, TV> callback)
        {
            Register(message, callback, typeof(T), typeof(TU), typeof(TV));
        }

        /// <summary>
        /// Registers a callback method, with 4 parameters, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register<T, TU, TV, TW>(TMessageType message, Action<T, TU, TV, TW> callback)
        {
            Register(message, callback, typeof(T), typeof(TU), typeof(TV), typeof(TW));
        }

        /// <summary>
        /// Registers a callback method, with 5 parameters, to be invoked when a specific message is broadcasted.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The callback to be called when this message is broadcasted.</param>
        public void Register<T, TU, TV, TW, TX>(TMessageType message, Action<T, TU, TV, TW, TX> callback)
        {
            Register(message, callback, typeof(T), typeof(TU), typeof(TV), typeof(TW), typeof(TX));
        }

        /// <summary>
        /// Registers the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <exception cref="System.ArgumentException">'message' cannot be null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">callback</exception>
        private void Register(TMessageType message, Delegate callback, params Type[] parameterType)
        {
            if (message == null)
                throw new ArgumentException("'message' cannot be null or empty.");

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _messageToActionsMap.AddAction(message, callback.Target, callback.Method, parameterType);
        }

        /// <summary>
        /// Deregisters the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="owner">The owner.</param>
        public void Deregister(TMessageType message, object owner)
        {
            _messageToActionsMap.RemoveAction(message, owner);
        }

        #endregion

        #region NotifyListeners

        /// <summary>
        /// Notifies all registered parties that a message is being broadcasted.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="parameters">The parameter to pass together with the message.</param>
        public void NotifyListeners(TMessageType message, params object[] parameters)
        {
            if (message == null)
                throw new ArgumentException("'message' cannot be null or empty.");

            Type[] registeredParameters;
            if (_messageToActionsMap.TryGetParameterTypes(message, out registeredParameters))
            {
                if (registeredParameters == null && (parameters != null && parameters.Any()))
                    throw new TargetParameterCountException("parameters not expected.");

                if (registeredParameters != null && (parameters == null || parameters.Length != registeredParameters.Length))
                    throw new TargetParameterCountException("parameter count mismatch");
               
            }

            var actions = _messageToActionsMap.GetActions(message);
            if (actions == null) return;

            if (parameters == null || !parameters.Any())
            {
                actions.ForEach(action => action.DynamicInvoke());
            }
            else
            {
                actions.ForEach(action => action.DynamicInvoke(parameters));
            }
        }

         #endregion
    }
}


