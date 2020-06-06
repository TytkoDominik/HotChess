using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace GameDesire.Rest.Utility
{
    public class CallbackWrapper
    {
        public CallbackWrapper(object action)
        {
            Action = action;
        }

        private const string METHOD_NAME = "Invoke";

        private object Action { get; set; }
        [CanBeNull]
        public Type ActionArgumentType
        {
            get
            {
                if (Action == null)
                {
                    return null;
                }

                var invokeMethod = Action.GetType().GetMethod(METHOD_NAME);

                return invokeMethod == null ? null : invokeMethod.GetParameters().First().ParameterType;
            }
        }

        public void Invoke(object arg)
        {
            if (Action == null)
            {
                return;
            }

            var action = Action.GetType().GetMethod(METHOD_NAME);

            if (action != null)
            {
                action.Invoke(Action, new[] {arg});
            }
        }
    }
}