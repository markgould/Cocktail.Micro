/*
/// <changelog>
//   <item who="jtraband" when="Nov-5-2009">Created based on a blog by Daniel Cazzulino.</item>
///   <item who="jtraband" when="May-5-2010">Moved into IdeaBlade.Core</item>
///  <item who="sbelini" when="June-15-2012">Added attribution to Daniel Cazzulino</item>
</changelog>
*/

using System.ComponentModel;
using System;

namespace Cocktail {
  /// <summary>
  /// Hides standard Object members from Intellisense  
  /// to make fluent interfaces easier to read. 
  /// May be implemented on any class.
  /// Based on blog post by @kzu here: 
  /// http://bit.ly/ifluentinterface 
  /// </summary> 
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IHideObjectMembers {
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Type GetType();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    int GetHashCode();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    string ToString();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    bool Equals(object obj);
  }

}