using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common.MessengerService
{
    /// <summary>
    /// This class is an implementation detail of the Messenger class.
    /// </summary>
    public class MessageToActionsMap<T>
    {
        #region Fields

        // Stores a hash where the key is the message and the value is the list of callbacks to invoke.
        readonly Dictionary<T, List<WeakAction>> _map = new Dictionary<T, List<WeakAction>>();

        #endregion

        #region Constructor

        internal MessageToActionsMap()
        {
        }

        #endregion // Constructor

        #region AddAction

        internal void AddAction(T message, object target, MethodInfo method, Type[] parameterTypes)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (method == null)
                throw new ArgumentNullException("method");

            lock (_map)
            {
                if (!_map.ContainsKey(message))
                    _map[message] = new List<WeakAction>();

                _map[message].Add(new WeakAction(target, method, parameterTypes));
            }
        }

        internal void RemoveAction(T message, object owner)
        {
            if (message == null)
                throw new ArgumentNullException("message");


            lock (_map)
            {
                if (!_map.ContainsKey(message))
                    return;

                _map[message].RemoveAll(wa => wa.TargetRef != null && wa.TargetRef.Target == owner);
                if (_map[message].Count == 0)
                {
                    _map.Remove(message);
                }
            }
        }

        #endregion

        #region GetActions

        /// <summary>
        /// Gets the list of actions to be invoked for the specified message
        /// </summary>
        /// <param name="message">The message to get the actions for</param>
        /// <returns>Returns a list of actions that are registered to the specified message</returns>
        internal List<Delegate> GetActions(T message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            List<Delegate> actions;
            lock (_map)
            {
                if (!_map.ContainsKey(message))
                    return null;

                var weakActions = _map[message];
                actions = new List<Delegate>(weakActions.Count);
                for (var i = weakActions.Count - 1; i > -1; --i)
                {
                    var weakAction = weakActions[i];
                    if (weakAction == null)
                        continue;

                    var action = weakAction.CreateAction();
                    if (action != null)
                    {
                        actions.Add(action);
                    }
                    else
                    {
                        // The target object is dead, so get rid of the weak action.
                        weakActions.Remove(weakAction);
                    }
                }

                // Delete the list from the map if it is now empty.
                if (weakActions.Count == 0)
                    _map.Remove(message);
            }

            // Reverse the list to ensure the callbacks are invoked in the order they were registered.
            actions.Reverse();

            return actions;
        }

        #endregion

        #region TryGetParameterType


        internal bool TryGetParameterTypes(T message, out Type[] parameterTypes)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            parameterTypes = null;
            List<WeakAction> weakActions;
            lock (_map)
            {
                if (!_map.TryGetValue(message, out weakActions) || weakActions.Count == 0)
                    return false;
            }
            parameterTypes = weakActions[0].ParameterTypes;
            return true;
        }

        #endregion
    }
}
