using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Bia.StylecopWrapper
{
    public static class ReflectionExtensions
    {
        public static object GetValue(this object obj, string propertyName)
        {
            var nested = propertyName.Split('.');
            object lastValue = null;
            object lastObject = obj;
            foreach (var property in nested)
            {
                lastValue = lastObject.GetType().GetProperty(property).GetValue(lastObject);
                lastObject = lastValue;
            }

            return lastValue;
        }

        public static void InvokeMethod(this object obj, string methodName, params object[] args)
        {
            obj.GetType().GetMethod(methodName).Invoke(obj, args);
        }

        public static object CreateInstance(this Assembly asm, string typeName, params object[] args)
        {
            return Activator.CreateInstance(asm.GetType(typeName), args);
        }

        public static void AddEventHandler(this object obj, string eventName, MethodInfo handlerInfo)
        {
            var eventInfo = obj.GetType().GetEvent(eventName);
            eventInfo.AddEventHandler(obj, Delegate.CreateDelegate(eventInfo.EventHandlerType, handlerInfo));
        }

        public static IList CreateList(this Type type, params object[] objects)
        {
            var genericListType = typeof(List<>);
            var specificListType = genericListType.MakeGenericType(type);
            var list = (IList)Activator.CreateInstance(specificListType);
            foreach (var o in objects)
            {
                list.Add(Convert.ChangeType(o, type));
            }

            return list;
        }
    }
}
