using System;
using System.Reflection;

namespace Cocktail {

  /// <summary>
  /// For internal use only.
  /// </summary>
  public delegate void UnregisterCallback<E>(EventHandler<E> eventHandler) where E : EventArgs;

  /// <summary>
  /// For internal use only.
  /// </summary>
  /// <typeparam name="E"></typeparam>
  public interface IWeakEventHandler<E> where E : EventArgs {

    /// <summary/>
    EventHandler<E> Handler { get; }
  }

  /// <summary>
  /// Event handler to be used in those cases where a publisher should not hold references to its subscribers, 
  /// as implicitly occurs in the regular event model.
  /// </summary>
  /// <remarks>
  /// In the .NET event model, when subscribing to an event as follows:
  /// <code>Publisher.Event += new xxxEventHandler(subscriberMethod)</code>     
  /// the publisher will have a reference to the subscriber. The alternative is to create
  /// a weak eventHandler using the following syntax.
  /// 
  ///   Example:
  /// provider.MyEvent += new EventHandler&lt;EventArgs>(MyWeakEventHandler).MakeWeak(eh => provider.MyEvent -= eh);
  /// 
  /// private void MyWeakEventHandler(object sender, EventArgs e) { ... }
  /// </remarks>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="E"></typeparam>
  public class WeakEventHandler<T, E> : IWeakEventHandler<E>
    where T : class
    where E : EventArgs {

   /// <summary/>
    public WeakEventHandler(EventHandler<E> eventHandler, UnregisterCallback<E> unregister) {
      _targetRef = new WeakReference(eventHandler.Target);
#if NETFX_CORE
      _openHandler = (OpenEventHandler) eventHandler.GetMethodInfo().CreateDelegate(typeof(OpenEventHandler));
#else
      _openHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler),
        null, eventHandler.Method);
#endif
      _handler = Invoke;
      _unregister = unregister;
    }

    /// <summary/>
    public void Invoke(object sender, E e) {
      T target = (T)_targetRef.Target;

      if (target != null) {
        _openHandler.Invoke(target, sender, e);
      }  else if (_unregister != null) {
        _unregister(_handler);
        _unregister = null;
      }
    }

    /// <summary/>
    public EventHandler<E> Handler {
      get { return _handler; }
    }

    /// <summary/>
    public static implicit operator EventHandler<E>(WeakEventHandler<T, E> weh) {
      return weh._handler;
    }

    private delegate void OpenEventHandler(T @this, object sender, E e);

    private WeakReference _targetRef;
    private OpenEventHandler _openHandler;
    private EventHandler<E> _handler;
    private UnregisterCallback<E> _unregister;

  }

  /// <summary>
  /// 
  /// </summary>
  public static class EventHandlerUtils {

    /// <summary/>
    public static EventHandler<E> MakeWeak<E>(this EventHandler<E> eventHandler, UnregisterCallback<E> unregister)
      where E : EventArgs {
      if (eventHandler == null) {
        throw new ArgumentNullException("eventHandler");
      }
#if NETFX_CORE
      var method = eventHandler.GetMethodInfo();
#else
      var method = eventHandler.Method;
#endif
      
      if (method.IsStatic || eventHandler.Target == null) {
        throw new ArgumentException("Only instance methods are supported.", "eventHandler");
      }

      Type wehType = typeof(WeakEventHandler<,>).MakeGenericType(method.DeclaringType, typeof(E));
      ConstructorInfo wehConstructor = wehType.GetConstructor(new Type[] { typeof(EventHandler<E>), 
        typeof(UnregisterCallback<E>) });

      IWeakEventHandler<E> weh = (IWeakEventHandler<E>)wehConstructor.Invoke(
        new object[] { eventHandler, unregister });

      return weh.Handler;
    }


  }

  
  


}
