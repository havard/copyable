using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace OX.Copyable
{
    /// <summary>
    /// This class defines all the extension methods provided by the Copyable framework 
    /// on the <see cref="System.Object"/> type.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// A list of instance providers that are available.
        /// </summary>
        private static List<IInstanceProvider> Providers = IntializeInstanceProviders();

        /// <summary>
        /// Updates the list of instance providers with any found in the newly loaded assembly.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="args">The event arguments.</param>
        private static void AssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            UpdateInstanceProviders(args.LoadedAssembly, Providers);
        }

        /// <summary>
        /// Initializes the list of instance providers.
        /// </summary>
        /// <returns>A list of instance providers that are used by the Copyable framework.</returns>
        private static List<IInstanceProvider> IntializeInstanceProviders()
        {
            List<IInstanceProvider> providers = new List<IInstanceProvider>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                UpdateInstanceProviders(assembly, providers);
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(AssemblyLoaded);
            return providers;
        }

        /// <summary>
        /// Updates the list of instance providers with the ones found in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly with which the list of instance providers will be updated.</param>
        private static void UpdateInstanceProviders(Assembly assembly, List<IInstanceProvider> providerList)
        {
            providerList.AddRange(GetInstanceProviders(assembly));
        }

        /// <summary>
        /// Yields all instance providers defined in the assembly, if and only if they are instantiable
        /// without any arguments. <b>NOTE: Instance providers that cannot be instantiated in this 
        /// way are not used by the Copyable framework!</b>
        /// </summary>
        /// <param name="assembly">The assembly from which instance providers will be retrieved.</param>
        /// <returns>An <see cref="IEnumerable" /> of the instance providers of the assembly.</returns>
        private static IEnumerable<IInstanceProvider> GetInstanceProviders(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (typeof(IInstanceProvider).IsAssignableFrom(t))
                {
                    IInstanceProvider instance = null;
                    try
                    {
                        instance = (IInstanceProvider)Activator.CreateInstance(t);
                    }
                    catch { } // Ignore provider if it cannot be created
                    if (instance != null)
                        yield return instance;
                }
            }
        }

        /// <summary>
        /// Creates a copy of the object.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <returns>A deep copy of the object.</returns>
        public static object Copy(this object instance)
        {
            if (instance == null)
                return null;
            return Copy(instance, DeduceInstance(instance));
        }

        /// <summary>
        /// Creates a deep copy of the object using the supplied object as a target for the copy operation.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <param name="copy">The object to copy values to. All fields of this object will be overwritten.</param>
        /// <returns>A deep copy of the object.</returns>
        public static object Copy(this object instance, object copy)
        {
            if (instance == null)
                return null;
            if (copy == null)
                throw new ArgumentNullException("The copy instance cannot be null");
            return Clone(instance, new VisitedGraph(), copy);
        }

        /// <summary>
        /// Creates a deep copy of an object using the supplied dictionary of visited objects as 
        /// a source of objects already encountered in the copy traversal. The dictionary of visited 
        /// objects is used for holding objects that have already been copied, to avoid erroneous 
        /// duplication of parts of the object graph.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <param name="visited">The graph of objects visited so far.</param>
        /// <returns></returns>
        private static object Clone(this object instance, VisitedGraph visited)
        {
            if (instance == null)
                return null;

            Type instanceType = instance.GetType();

            if(typeof(Type).IsAssignableFrom(instanceType))
            {
                return instance;
            }

            if (instanceType.IsPointer || instanceType == typeof(Pointer) || instanceType.IsPrimitive || instanceType == typeof(string))
                return instance; // Pointers, primitive types and strings are considered immutable
            
            if (instanceType.IsArray)
            {
                int length = ((Array)instance).Length;
                Array copied = (Array)Activator.CreateInstance(instanceType, length);
                visited.Add(instance, copied);
                for (int i = 0; i < length; ++i)
                    copied.SetValue(((Array)instance).GetValue(i).Clone(visited), i);
                return copied;
            }
            
            return Clone(instance, visited, DeduceInstance(instance));
        }

        private static object Clone(this object instance, VisitedGraph visited, object copy)
        {
            if (visited.ContainsKey(instance))
                return visited[instance];
            else
                visited.Add(instance, copy);

            Type type = instance.GetType();

            while (type != null)
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    object value = field.GetValue(instance);
                    if (visited.ContainsKey(value))
                        field.SetValue(copy, visited[value]);
                    else
                        field.SetValue(copy, value.Clone(visited));
                }

                type = type.BaseType;
            }
            return copy;
        }

        private static object DeduceInstance(object instance)
        {
            Type instanceType = instance.GetType();

            if (typeof(Copyable).IsAssignableFrom(instanceType))
                return ((Copyable)instance).CreateInstanceForCopy();
            foreach (IInstanceProvider provider in Providers)
            {
                if (provider.Provided == instanceType)
                    return provider.CreateCopy(instance);
            }

            try
            {
                return FormatterServices.GetUninitializedObject(instanceType);
            }
            catch
            {
                throw new ArgumentException(string.Format("Object of type {0} cannot be cloned because an uninitialized object could not be created.", instanceType.FullName));
            }
        }
    }
}
