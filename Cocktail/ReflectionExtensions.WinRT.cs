using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Cocktail {

  #region helper functions that may differ by env

  internal static partial class ReflectionExtensions {

    public const BindingFlags Default = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

    public static Assembly LoadAssembly(string assemblyName) {
      var aName = new AssemblyName(assemblyName);
      return Assembly.Load(aName);
    }

    public static Assembly GetAssembly(this Type type) {
      return type.GetTypeInfo().Assembly;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="typeName">Full type name</param>
    /// <param name="throwOnError"></param>
    /// <param name="ignoreCase">Not currently used</param>
    /// <returns></returns>
    public static Type GetType(this Assembly assembly, string typeName, bool throwOnError = false, bool ignoreCase = false) {
      var ti = assembly.DefinedTypes.FirstOrDefault(i => i.FullName == typeName);   // todo - case
      if (ti == null && throwOnError) throw new TypeLoadException(string.Format("Type {0} not found", typeName));
      return ti != null ? ti.AsType() : null;
    }

    public static IEnumerable<Type> GetAllTypes(this Assembly assembly) {
      return assembly.DefinedTypes.Select(t => t.AsType());
    }

    public static object ConvertType(object value, Type conversionType, bool throwIfError) {
      try {
        return Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture);
      } catch {
        if (throwIfError) throw;
        return null;
      }
    }

    public static MethodInfo GetSetMethod(this PropertyInfo member, bool nonPublic = false) {
      return member.SetMethod;
    }
    public static MethodInfo GetGetMethod(this PropertyInfo member, bool nonPublic = false) {
      return member.GetMethod;
    }

    #region GetAttribute / GetAttributes
    public static T GetAttribute<T>(this Type provider) where T : Attribute {
      return provider.GetTypeInfo().GetCustomAttributes<T>(true).FirstOrDefault();
    }

    public static T GetAttribute<T>(this MemberInfo provider) where T : Attribute {
      return provider.GetCustomAttributes<T>(true).FirstOrDefault();
    }

    public static T GetAttribute<T>(this Module provider) where T : Attribute {
      return provider.GetCustomAttributes<T>().FirstOrDefault();
    }

    public static T GetAttribute<T>(this Assembly provider) where T : Attribute {
      return provider.Modules.Select(m => m.GetAttribute<T>()).FirstOrDefault(a => a != null);
    }

    public static IEnumerable<T> GetAttributes<T>(this Type provider, bool inherit = true) where T : Attribute {
      return provider.GetTypeInfo().GetCustomAttributes<T>(inherit);
    }

    public static IEnumerable<Attribute> GetAttributes(this Type provider, Type attrType, bool inherit = true) {
      return provider.GetTypeInfo().GetCustomAttributes(attrType, inherit);
    }

    public static IEnumerable<T> GetAttributes<T>(this MemberInfo provider, bool inherit = true) where T : Attribute {
      return provider.GetCustomAttributes<T>(inherit);
    }

    public static IEnumerable<T> GetAttributes<T>(this MethodInfo provider, bool inherit = true) where T : Attribute {
      return provider.GetCustomAttributes<T>(inherit);
    }
    #endregion

  }

  #endregion

  #region For "parity" of a sort with Type in SL/NET
  internal static class TypeExtensions_Metro
  {

    public static bool IsAssignableFrom(this Type type, Type anotherType) {
      return type.GetTypeInfo().IsAssignableFrom(anotherType.GetTypeInfo());
    }

    public static TypeCode GetTypeCode(this Type type) {
      // TypeCode is not in metro ... f'in pain - do we need this stuff??
      if (type == null) return TypeCode.Empty;
      if (type.GetTypeInfo().IsEnum) return TypeCode.Object;
      if (type == typeof(byte)) return TypeCode.Byte;
      if (type == typeof(sbyte)) return TypeCode.SByte;
      if (type == typeof(Int16)) return TypeCode.Int16;
      if (type == typeof(Int32)) return TypeCode.Int32;
      if (type == typeof(Int64)) return TypeCode.Int64;
      if (type == typeof(UInt16)) return TypeCode.UInt16;
      if (type == typeof(UInt32)) return TypeCode.UInt32;
      if (type == typeof(UInt64)) return TypeCode.UInt64;
      if (type == typeof(Single)) return TypeCode.Single;
      if (type == typeof(Double)) return TypeCode.Double;
      if (type == typeof(Decimal)) return TypeCode.Decimal;
      return TypeCode.Empty;      // ok not true, but too lazy to enumerate them all
    }


    public static MemberTypes GetMemberType(this MemberInfo member) {
      if (member is FieldInfo)
        return MemberTypes.Field;
      if (member is ConstructorInfo)
        return MemberTypes.Constructor;
      if (member is PropertyInfo)
        return MemberTypes.Property;
      if (member is EventInfo)
        return MemberTypes.Event;
      if (member is MethodInfo)
        return MemberTypes.Method;

      var typeInfo = member as TypeInfo;
      if (!typeInfo.IsPublic && !typeInfo.IsNotPublic)
        return MemberTypes.NestedType;

      return MemberTypes.TypeInfo;
    }


    public static ConstructorInfo GetConstructor(this Type type, Type[] paramTypes) {
      return GetConstructors(type, ReflectionExtensions.Default).FirstOrDefault(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(paramTypes));
    }

    public static ConstructorInfo[] GetConstructors(this Type type) {
      return GetConstructors(type, ReflectionExtensions.Default);
    }

    public static ConstructorInfo[] GetConstructors(this Type type, BindingFlags flags) {
      var props = type.GetTypeInfo().DeclaredConstructors;
      return props.Where(p =>
        ((flags.HasFlag(BindingFlags.Static) == p.IsStatic) ||
         (flags.HasFlag(BindingFlags.Instance) == !p.IsStatic)
        ) &&
        ((flags.HasFlag(BindingFlags.Public) == p.IsPublic) ||
          (flags.HasFlag(BindingFlags.NonPublic) == p.IsPrivate)
        )).ToArray();
    }

    public static EventInfo GetEvent(this Type type, string name) {
      return GetEvent(type, name, ReflectionExtensions.Default);
    }
    public static EventInfo GetEvent(this Type type, string name, BindingFlags flags) {
      return GetEvents(type, flags).FirstOrDefault(f => f.Name == name);
    }
    public static EventInfo[] GetEvents(this Type type) {
      return GetEvents(type, ReflectionExtensions.Default);
    }
    public static EventInfo[] GetEvents(this Type type, BindingFlags flags) {
      var props = flags.HasFlag(BindingFlags.DeclaredOnly) ? type.GetTypeInfo().DeclaredEvents : type.GetRuntimeEvents();
      //todo - this is probably not correct
      // also assumes only the getter matters ..
      return props.Where(p =>
        ((flags.HasFlag(BindingFlags.Static) == p.AddMethod.IsStatic) ||
         (flags.HasFlag(BindingFlags.Instance) == !p.AddMethod.IsStatic)
        ) &&
        ((flags.HasFlag(BindingFlags.Public) == p.AddMethod.IsPublic) ||
          (flags.HasFlag(BindingFlags.NonPublic) == p.AddMethod.IsPrivate)
        )).ToArray();
    }

    public static FieldInfo GetField(this Type type, string name) {
      return GetField(type, name, ReflectionExtensions.Default);
    }

    public static FieldInfo GetField(this Type type, string name, BindingFlags flags) {
      return GetFields(type, flags).FirstOrDefault(f => f.Name == name);
    }

    public static FieldInfo[] GetFields(this Type type) {
      return GetFields(type, ReflectionExtensions.Default);
    }

    public static FieldInfo[] GetFields(this Type type, BindingFlags flags) {
      var fields = flags.HasFlag(BindingFlags.DeclaredOnly) ? type.GetTypeInfo().DeclaredFields : type.GetRuntimeFields();
      //todo - this is probably not correct
      // also assumes only the getter matters ..
      return fields.Where(p =>
        ((flags.HasFlag(BindingFlags.Static) == p.IsStatic) || (flags.HasFlag(BindingFlags.Instance) == !p.IsStatic)
        ) &&
        ((flags.HasFlag(BindingFlags.Public) == p.IsPublic) || (flags.HasFlag(BindingFlags.NonPublic) == p.IsPrivate)
        )).ToArray();
    }

    public static Type[] GetGenericArguments(this Type type) {
      return type.GetTypeInfo().GenericTypeArguments;
    }

    public static MemberInfo[] GetMember(this Type type, string name) {
      return GetMember(type, name, ReflectionExtensions.Default);
    }
    public static MemberInfo[] GetMember(this Type type, string name, BindingFlags flags) {
      return GetMembers(type, flags).Where(m => m.Name == name).ToArray();
    }
    //public MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr);

    public static MemberInfo[] GetMembers(this Type type) {
      return GetMembers(type, ReflectionExtensions.Default);
    }
    public static MemberInfo[] GetMembers(this Type type, BindingFlags flags) {
      // Metro does have DeclaredMembers but nothing otherwise
      return GetEvents(type, flags).Cast<MemberInfo>()
        .Concat(GetFields(type, flags).Cast<MemberInfo>())
        .Concat(GetMethods(type, flags).Cast<MemberInfo>())
        .Concat(GetProperties(type, flags).Cast<MemberInfo>())
        .ToArray();
    }

    public static MethodInfo GetMethod(this Type type, string name) {
      return GetMethod(type, name, ReflectionExtensions.Default);
    }
    public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags) {
      return GetMethods(type, flags).FirstOrDefault(m => m.Name == name);
    }
    //public MethodInfo GetMethod(string name, Type[] types);

    public static MethodInfo[] GetMethods(this Type type) {
      return GetMethods(type, ReflectionExtensions.Default);
    }

    public static MethodInfo[] GetMethods(this Type type, BindingFlags flags) {
      var methods = flags.HasFlag(BindingFlags.DeclaredOnly) ? type.GetTypeInfo().DeclaredMethods : type.GetRuntimeMethods();
      //todo - this is probably not correct
      return methods.Where(m =>
        ((flags.HasFlag(BindingFlags.Static) == m.IsStatic) || (flags.HasFlag(BindingFlags.Instance) == !m.IsStatic)
        ) &&
        ((flags.HasFlag(BindingFlags.Public) == m.IsPublic) || (flags.HasFlag(BindingFlags.NonPublic) == m.IsPrivate)
        )).ToArray();
    }

    public static Type GetNestedType(this Type type, string name, BindingFlags flags) {
      // todo - flags are ignored
      var ti = type.GetTypeInfo().DeclaredNestedTypes.FirstOrDefault(t => t.Name == name);
      return ti == null ? null : ti.AsType();
    }

    public static PropertyInfo[] GetProperties(this Type type) {
      return GetProperties(type, ReflectionExtensions.Default);
    }

    public static PropertyInfo[] GetProperties(this Type type, BindingFlags flags) {
      var props = flags.HasFlag(BindingFlags.DeclaredOnly) ? type.GetTypeInfo().DeclaredProperties : type.GetRuntimeProperties();
      //todo - this is probably not correct
      // also assumes only the getter matters ..
      return props.Where(p =>
        ((flags.HasFlag(BindingFlags.Static) == (p.GetMethod != null && p.GetMethod.IsStatic)) ||
          (flags.HasFlag(BindingFlags.Instance) == (p.GetMethod != null && !p.GetMethod.IsStatic))
        ) &&
        ((flags.HasFlag(BindingFlags.Public) == (p.GetMethod != null && p.GetMethod.IsPublic)) ||
          (flags.HasFlag(BindingFlags.NonPublic) == (p.GetMethod != null && p.GetMethod.IsPrivate))
        )).ToArray();
    }

    public static PropertyInfo GetProperty(this Type type, string name) {
      return GetProperty(type, name, ReflectionExtensions.Default);
    }

    public static PropertyInfo GetProperty(this Type type, string name, BindingFlags flags) {
      return GetProperties(type, flags).FirstOrDefault(p => p.Name == name);
    }
  }
  #endregion

}
