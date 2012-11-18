using System;
#if !NETFX_CORE
using System.ComponentModel.Composition;
#else
using System.Composition;
#endif

namespace Cocktail
{
#if !NETFX_CORE
    /// <summary>
    ///     Used internally by the DevForce framework to mark a type as exported.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(
        AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property |
        AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class InterfaceExportAttribute : InheritedExportAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="exportedType"></param>
        public InterfaceExportAttribute(Type exportedType)
            : base(exportedType)
        {
        }
    }

#else
    /// <summary>
    ///     Used internally by the DevForce framework to mark a type as exported.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(
        AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property |
        AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class InterfaceExportAttribute : ExportAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="exportedType"></param>
        public InterfaceExportAttribute(Type exportedType)
            : base(exportedType)
        {
        }
    }

#endif
}