using System;

namespace OX.Copyable
{
    /// <summary>
    /// Abstract class that implements the <see cref="IInstanceProvider"/> interface,
    /// and can be used as a base class for an instance provider. The class simplifies
    /// implementation by partially implementing the interface, leaving the implementation
    /// of the <see cref="CreateTypedCopy"/> method to the concrete subclass.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class InstanceProvider<T> : IInstanceProvider<T>
    {
        #region IInstanceProvider Members

        public Type Provided
        {
            get { return typeof(T); }
        }

        public object CreateCopy(object toBeCopied)
        {
            return CreateTypedCopy((T)toBeCopied);
        }

        public abstract T CreateTypedCopy(T toBeCopied);

        #endregion
    }
}
