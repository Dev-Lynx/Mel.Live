using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Mel.Live.Extensions.UnityExtensions
{
    /// <summary>
    /// Extensions methods to extend and facilitate the usage of <see cref="IUnityContainer"/>.
    /// </summary>
    public static class UnityContainerHelper
    {
        /// <summary>
        /// Returns whether a specified type has a type mapping registered in the container.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to check for the type mapping.</param>
        /// <param name="type">The type to check if there is a type mapping for.</param>
        /// <returns><see langword="true"/> if there is a type mapping registered for <paramref name="type"/>.</returns>
        /// <remarks>In order to use this extension method, you first need to add the
        /// <see cref="IUnityContainer"/> extension to the extension to the UnityBootstrapperExtension.
        /// </remarks>        
        public static bool IsTypeRegistered(this IUnityContainer container, Type type)
        {
            return container.IsRegistered(type);
        }

        /// <summary>
        /// Utility method to try to resolve a service from the container avoiding an exception if the container cannot build the type.
        /// </summary>
        /// <param name="container">The cointainer that will be used to resolve the type.</param>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>The instance of <typeparamref name="T"/> built up by the container.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T TryResolve<T>(this IUnityContainer container)
        {
            object result = TryResolve(container, typeof(T));
            if (result != null)
            {
                return (T)result;
            }
            return default(T);
        }

        /// <summary>
        /// Utility method to try to resolve a service from the container avoiding an exception if the container cannot build the type.
        /// </summary>
        /// <param name="container">The cointainer that will be used to resolve the type.</param>
        /// <param name="typeToResolve">The type to resolve.</param>
        /// <returns>The instance of <paramref name="typeToResolve"/> built up by the container.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static object TryResolve(this IUnityContainer container, Type typeToResolve)
        {
            try
            {
                return container.Resolve(typeToResolve);
            }
            catch
            {
                return null;
            }
        }


        public static void RegisterControllers(this IUnityContainer container)
        {
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

            foreach (var controller in controllers)
                container.RegisterType(controller, new TransientLifetimeManager());
        }

        public static void RegisterScoped(this IUnityContainer container, Type from, Type to) =>
            container.RegisterType(from, to, new HierarchicalLifetimeManager());

        public static void RegisterTransient(this IUnityContainer container, Type from, Type to) =>
            container.RegisterType(from, to, new TransientLifetimeManager());

        public static void RegisterTransient<TTFrom, TTo>(this IUnityContainer container) where TTo : TTFrom 
            => container.RegisterType<TTFrom, TTo>(new TransientLifetimeManager());

        public static void RegisterTransient<T>(this IUnityContainer container)
            => container.RegisterType<T>(new TransientLifetimeManager());

        public static void RegisterScoped<TTFrom, TTo>(this IUnityContainer container) where TTo : TTFrom
            => container.RegisterType<TTFrom, TTo>(new HierarchicalLifetimeManager());

        public static void RegisterScoped<T>(this IUnityContainer container)
            => container.RegisterType<T>(new HierarchicalLifetimeManager());
    }
}