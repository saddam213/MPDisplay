using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    /// <summary>
    /// This class is an implementation detail of the MessageToActionsMap class.
    /// </summary>
    public class WeakAction
    {
        #region Fields

        internal readonly Type[] ParameterTypes;
        readonly Type _delegateType;
        internal readonly MethodInfo _method;
        internal readonly WeakReference _targetRef;

        #endregion

        #region Constructor

        internal WeakAction(object target, MethodInfo method, params Type[] parameterTypes)
        {
            if (target == null)
            {
                _targetRef = null;
            }
            else
            {
                _targetRef = new WeakReference(target);
            }

            _method = method;

            this.ParameterTypes = parameterTypes;

            if (parameterTypes == null || parameterTypes.Count() == 0)
            {
                _delegateType = typeof(Action);
            }
            else if (parameterTypes.Count() == 1)
            {
                _delegateType = typeof(Action<>).MakeGenericType(parameterTypes);
            }
            else if (parameterTypes.Count() == 2)
            {
                _delegateType = typeof(Action<,>).MakeGenericType(parameterTypes);
            }
            else if (parameterTypes.Count() == 3)
            {
                _delegateType = typeof(Action<,,>).MakeGenericType(parameterTypes);
            }
        }

        #endregion

        #region CreateAction

        /// <summary>
        /// Creates a "throw away" delegate to invoke the method on the target, or null if the target object is dead.
        /// </summary>
        internal Delegate CreateAction()
        {
            // Rehydrate into a real Action object, so that the method can be invoked.
            if (_targetRef == null)
            {
                return Delegate.CreateDelegate(_delegateType, _method);
            }
            else
            {
                try
                {
                    object target = _targetRef.Target;
                    if (target != null)
                        return Delegate.CreateDelegate(_delegateType, target, _method);
                }
                catch
                {
                }
            }

            return null;
        }

        #endregion
    }
}
