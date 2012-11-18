using System;

namespace Cocktail
{

  // Summary:
  //     Marks each type of member that is defined as a derived class of MemberInfo.
  [Flags]
  internal enum MemberTypes {
    // Summary:
    //     Specifies that the member is a constructor, representing a System.Reflection.ConstructorInfo
    //     member. Hexadecimal value of 0x01.
    Constructor = 1,
    //
    // Summary:
    //     Specifies that the member is an event, representing an System.Reflection.EventInfo
    //     member. Hexadecimal value of 0x02.
    Event = 2,
    //
    // Summary:
    //     Specifies that the member is a field, representing a System.Reflection.FieldInfo
    //     member. Hexadecimal value of 0x04.
    Field = 4,
    //
    // Summary:
    //     Specifies that the member is a method, representing a System.Reflection.MethodInfo
    //     member. Hexadecimal value of 0x08.
    Method = 8,
    //
    // Summary:
    //     Specifies that the member is a property, representing a System.Reflection.PropertyInfo
    //     member. Hexadecimal value of 0x10.
    Property = 16,
    //
    // Summary:
    //     Specifies that the member is a type, representing a System.Reflection.MemberTypes.TypeInfo
    //     member. Hexadecimal value of 0x20.
    TypeInfo = 32,
    //
    // Summary:
    //     Specifies that the member is a custom member type. Hexadecimal value of 0x40.
    Custom = 64,
    //
    // Summary:
    //     Specifies that the member is a nested type, extending System.Reflection.MemberInfo.
    NestedType = 128,
    //
    // Summary:
    //     Specifies all member types.
    All = 191,
  }

  // Summary:
  //     Specifies the type of an object.
  internal enum TypeCode
  {
    // Summary:
    //     A null reference.
    Empty = 0,
    //
    // Summary:
    //     A general type representing any reference or value type not explicitly represented
    //     by another TypeCode.
    Object = 1,
    //
    // Summary:
    //     A database null (column) value.
    DBNull = 2,
    //
    // Summary:
    //     A simple type representing Boolean values of true or false.
    Boolean = 3,
    //
    // Summary:
    //     An integral type representing unsigned 16-bit integers with values between
    //     0 and 65535. The set of possible values for the System.TypeCode.Char type
    //     corresponds to the Unicode character set.
    Char = 4,
    //
    // Summary:
    //     An integral type representing signed 8-bit integers with values between -128
    //     and 127.
    SByte = 5,
    //
    // Summary:
    //     An integral type representing unsigned 8-bit integers with values between
    //     0 and 255.
    Byte = 6,
    //
    // Summary:
    //     An integral type representing signed 16-bit integers with values between
    //     -32768 and 32767.
    Int16 = 7,
    //
    // Summary:
    //     An integral type representing unsigned 16-bit integers with values between
    //     0 and 65535.
    UInt16 = 8,
    //
    // Summary:
    //     An integral type representing signed 32-bit integers with values between
    //     -2147483648 and 2147483647.
    Int32 = 9,
    //
    // Summary:
    //     An integral type representing unsigned 32-bit integers with values between
    //     0 and 4294967295.
    UInt32 = 10,
    //
    // Summary:
    //     An integral type representing signed 64-bit integers with values between
    //     -9223372036854775808 and 9223372036854775807.
    Int64 = 11,
    //
    // Summary:
    //     An integral type representing unsigned 64-bit integers with values between
    //     0 and 18446744073709551615.
    UInt64 = 12,
    //
    // Summary:
    //     A floating point type representing values ranging from approximately 1.5
    //     x 10 -45 to 3.4 x 10 38 with a precision of 7 digits.
    Single = 13,
    //
    // Summary:
    //     A floating point type representing values ranging from approximately 5.0
    //     x 10 -324 to 1.7 x 10 308 with a precision of 15-16 digits.
    Double = 14,
    //
    // Summary:
    //     A simple type representing values ranging from 1.0 x 10 -28 to approximately
    //     7.9 x 10 28 with 28-29 significant digits.
    Decimal = 15,
    //
    // Summary:
    //     A type representing a date and time value.
    DateTime = 16,
    //
    // Summary:
    //     A sealed class type representing Unicode character strings.
    String = 18,
  }

  // Summary:
  //     Specifies flags that control binding and the way in which the search for
  //     members and types is conducted by reflection.
  [Flags]
  internal enum BindingFlags {
    // Summary:
    //     No binding flag.
    Default = 0,
    //
    // Summary:
    //     The case of the member name should not be considered when binding.
    IgnoreCase = 1,
    //
    // Summary:
    //     Only members declared at the level of the supplied type's hierarchy should
    //     be considered. Inherited members are not considered.
    DeclaredOnly = 2,
    //
    // Summary:
    //     Instance members should be included in the search.
    Instance = 4,
    //
    // Summary:
    //     Static members should be included in the search.
    Static = 8,
    //
    // Summary:
    //     Public members should be included in the search.
    Public = 16,
    //
    // Summary:
    //     Non-public members should be included in the search.
    NonPublic = 32,
    //
    // Summary:
    //     Public and protected static members up the hierarchy should be returned.
    //     Private static members in inherited classes are not returned. Static members
    //     include fields, methods, events, and properties. Nested types are not returned.
    FlattenHierarchy = 64,
    //
    // Summary:
    //     A method is to be invoked. This must not be a constructor or a type initializer.
    InvokeMethod = 256,
    //
    // Summary:
    //     Reflection should create an instance of the specified type. This flag calls
    //     the constructor that matches the given arguments. The supplied member name
    //     is ignored. If the type of lookup is not specified, (Instance | Public) will
    //     apply. It is not possible to call a type initializer.
    CreateInstance = 512,
    //
    // Summary:
    //     The value of the specified field should be returned.
    GetField = 1024,
    //
    // Summary:
    //     The value of the specified field should be set.
    SetField = 2048,
    //
    // Summary:
    //     The value of the specified property should be returned.
    GetProperty = 4096,
    //
    // Summary:
    //     The value of the specified property should be set. For COM properties, specifying
    //     this binding flag is equivalent to specifying PutDispProperty and PutRefDispProperty.
    SetProperty = 8192,
    //
    // Summary:
    //     The PROPPUT member on a COM object should be invoked. PROPPUT specifies a
    //     property-setting function that uses a value. Use PutDispProperty if a property
    //     has both PROPPUT and PROPPUTREF and you need to distinguish which one is
    //     called.
    PutDispProperty = 16384,
    //
    // Summary:
    //     The PROPPUTREF member on a COM object should be invoked. PROPPUTREF specifies
    //     a property-setting function that uses a reference instead of a value. Use
    //     PutRefDispProperty if a property has both PROPPUT and PROPPUTREF and you
    //     need to distinguish which one is called.
    PutRefDispProperty = 32768,
    //
    // Summary:
    //     Types of the supplied arguments must exactly match the types of the corresponding
    //     formal parameters. Reflection throws an exception if the caller supplies
    //     a non-null Binder object, because that implies that the caller is supplying
    //     BindToXXX implementations that will pick the appropriate method. The default
    //     binder ignores this flag, whereas custom binders can implement the semantics
    //     of this flag.
    ExactBinding = 65536,
    //
    // Summary:
    //     Not implemented.
    SuppressChangeType = 131072,
    //
    // Summary:
    //     The set of members whose parameter count matches the number of supplied arguments
    //     should be returned. This binding flag is used for methods with parameters
    //     that have default values and methods with variable arguments (varargs). This
    //     flag should only be used with the System.Type.InvokeMember(System.String,System.Reflection.BindingFlags,System.Reflection.Binder,System.Object,System.Object[],System.Reflection.ParameterModifier[],System.Globalization.CultureInfo,System.String[])
    //     method. Parameters with default values are used only in calls where trailing
    //     arguments are omitted. They must be the last arguments.
    OptionalParamBinding = 262144,
    //
    // Summary:
    //     Used in COM interop to specify that the return value of the member can be
    //     ignored.
    IgnoreReturn = 16777216,
  }
}

namespace System.ComponentModel {

  // Summary:
  //     Defines members that data entity classes can implement to provide custom
  //     validation support.
    internal interface IDataErrorInfo
    {
    // Summary:
    //     Gets a message that describes any validation errors for the object.
    //
    // Returns:
    //     The validation error on the object; or null or System.String.Empty, if no
    //     errors are present.
    string Error { get; }

    // Summary:
    //     Gets a message that describes any validation errors for the specified property
    //     or column name.
    //
    // Parameters:
    //   columnName:
    //     The name of the property or column to retrieve validation errors for.
    //
    // Returns:
    //     The validation error on the specified property; or null or System.String.Empty,
    //     if no errors are present.
    string this[string columnName] { get; }
  }

  internal class DescriptionAttribute : Attribute
  {
    public DescriptionAttribute(string description)
      : base() {
        Description = description;
    }
    public string Description { get; private set; }
  }

  internal class ReadOnlyAttribute : Attribute
  {
    public ReadOnlyAttribute(bool value)
      : base() {
        IsReadOnly = value;
    }
    public bool IsReadOnly { get; private set; }
  }

  // Summary:
  //     Specifies the direction of a sort operation.
  internal enum ListSortDirection
  {
    // Summary:
    //     Sorts in ascending order.
    Ascending = 0,
    //
    // Summary:
    //     Sorts in descending order.
    Descending = 1,
  }

  [AttributeUsage(AttributeTargets.All)]
  internal sealed class BindableAttribute : Attribute
  {
 
 
    //
    // Summary:
    //     Initializes a new instance of the System.ComponentModel.BindableAttribute
    //     class with a Boolean value.
    //
    // Parameters:
    //   bindable:
    //     true to use property for binding; otherwise, false.
    public BindableAttribute(bool bindable) {
      this.Bindable = bindable;
    }
 
    //
    // Summary:
    //     Initializes a new instance of the System.ComponentModel.BindableAttribute
    //     class.
    //
    // Parameters:
    //   bindable:
    //     true to use property for binding; otherwise, false.
    //
    //   direction:
    //     One of the System.ComponentModel.BindingDirection values.
    public BindableAttribute(bool bindable, BindingDirection direction) {
      this.Bindable = bindable;
      this.Direction = direction;
    }

    // Summary:
    //     Gets a value indicating that a property is typically used for binding.
    //
    // Returns:
    //     true if the property is typically used for binding; otherwise, false.
    public bool Bindable { get; set; }
    //
    // Summary:
    //     Gets a value indicating the direction or directions of this property's data
    //     binding.
    //
    // Returns:
    //     A System.ComponentModel.BindingDirection.
    public BindingDirection Direction { get; set; }


  }
  // Summary:
  //     Specifies whether the template can be bound one way or two ways.
  internal enum BindingDirection
  {
    // Summary:
    //     The template can only accept property values. Used with a generic System.Web.UI.ITemplate.
    OneWay = 0,
    //
    // Summary:
    //     The template can accept and expose property values. Used with an System.Web.UI.IBindableTemplate.
    TwoWay = 1,
  }
}