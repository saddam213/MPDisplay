using System;
using System.Linq;
using System.Reflection;

namespace Common.MessengerService
{
    /// <summary>
    /// This class is an implementation detail of the MessageToActionsMap class.
    /// </summary>
    public class WeakAction
    {
        #region Fields

        internal readonly Type[] ParameterTypes;
        readonly Type _delegateType;
        internal readonly MethodInfo Method;
        internal readonly WeakReference TargetRef;

        #endregion

        #region Constructor

        internal WeakAction(object target, MethodInfo method, params Type[] parameterTypes)
        {
            TargetRef = target == null ? null : new WeakReference(target);

            Method = method;

            ParameterTypes = parameterTypes;

            if (parameterTypes == null || !parameterTypes.Any())
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
            else if (parameterTypes.Count() == 4)
            {
                _delegateType = typeof(Action<,,,>).MakeGenericType(parameterTypes);
            }
            else if (parameterTypes.Count() == 5)
            {
                _delegateType = typeof(Action<,,,,>).MakeGenericType(parameterTypes);
            }
            else if (parameterTypes.Count() == 6)
            {
                _delegateType = typeof(Action<,,,,,>).MakeGenericType(parameterTypes);
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
            if (TargetRef == null)
            {
                return Delegate.CreateDelegate(_delegateType, Method);
            }
            else
            {
                try
                {
                    object target = TargetRef.Target;
                    if (target != null)
                        return Delegate.CreateDelegate(_delegateType, target, Method);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        #endregion
    }
}
