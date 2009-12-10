using System;

namespace OX.Copyable
{
    /// <summary>
    /// An interface defining an instance provider, i.e. an object that can create instances of a specific type.
    /// If an instance of a class cannot be deduced automatically by the Copyable framework, and the class
    /// cannot be made a subclass of <see cref="Copyable" />, then creating an instance provider is the preferred
    /// way of making the class copyable.
    /// </summary>
    public interface IInstanceProvider
    {
        /// <summary>
        /// The type for which the provider provides instances.
        /// </summary>
        Type Provided { get; }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="toBeCopied">The object to be copied.</param>
        /// <returns>The created instance.</returns>
        object CreateCopy(object toBeCopied);
    }

    /// <summary>
    /// The generic version of <see cref="IInstanceProvider" />, defining a strongly typed way of providing instances.
    /// </summary>
    /// <typeparam name="T">The type of the instances provided by the implementor.</typeparam>
    public interface IInstanceProvider<T> : IInstanceProvider
    {
        /// <summary>
        /// Creates a new typed instance.
        /// </summary>
        /// <returns></returns>
        T CreateTypedCopy(T toBeCopied);
    }
}
