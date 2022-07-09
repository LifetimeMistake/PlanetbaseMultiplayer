using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Utils
{
    public static class Reflection
    {
        public static MethodInfo GetPrivateMethod(Type obj, string methodName, bool instance)
        {
            try
            {
                BindingFlags flags = BindingFlags.NonPublic | ((instance) ? BindingFlags.Instance : BindingFlags.Static);
                return obj.GetMethod(methodName, flags);
            }
            catch
            {
                return null;
            }
        }

        public static bool TryGetPrivateMethod(Type obj, string methodName, bool instance, out MethodInfo methodInfo)
        {
            methodInfo = GetPrivateMethod(obj, methodName, instance);
            return (methodInfo != null);
        }

        public static MethodInfo GetPrivateMethodOrThrow(Type obj, string methodName, bool instance)
        {
            MethodInfo methodInfo = GetPrivateMethod(obj, methodName, instance);
            if (methodInfo == null)
                throw new MissingMethodException($"Could not find \"{methodName}\"");

            return methodInfo;
        }

        public static FieldInfo GetPrivateField(Type obj, string fieldName, bool instance)
        {
            try
            {
                BindingFlags flags = BindingFlags.NonPublic | ((instance) ? BindingFlags.Instance : BindingFlags.Static);
                return obj.GetField(fieldName, flags);
            }
            catch
            {
                return null;
            }
        }

        public static FieldInfo GetPrivateFieldOrThrow(Type obj, string fieldName, bool instance)
        {
            FieldInfo fieldInfo = GetPrivateField(obj, fieldName, instance);
            if (fieldInfo == null)
                throw new MissingMethodException($"Could not find \"{fieldName}\"");

            return fieldInfo;
        }

        public static bool TryGetPrivateField(Type obj, string fieldName, bool instance, out FieldInfo fieldInfo)
        {
            fieldInfo = GetPrivateField(obj, fieldName, instance);
            return (fieldInfo != null);
        }

        public static object InvokeStaticMethod(MethodInfo method, params object[] args)
        {
            return method.Invoke(null, args);
        }

        public static object InvokeInstanceMethod(object instance, MethodInfo method, params object[] args)
        {
            return method.Invoke(instance, args);
        }

        public static object GetStaticFieldValue(FieldInfo field)
        {
            return field.GetValue(null);
        }

        public static object GetInstanceFieldValue(object instance, FieldInfo field)
        {
            return field.GetValue(instance);
        }

        public static void SetStaticFieldValue(FieldInfo field, object value)
        {
            field.SetValue(null, value);
        }

        public static void SetInstanceFieldValue(object instance, FieldInfo field, object value)
        {
            field.SetValue(instance, value);
        }
    }
}
