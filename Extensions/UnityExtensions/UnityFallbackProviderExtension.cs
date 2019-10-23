using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;

namespace Mel.Live.Extensions.UnityExtensions
{
    public class UnityFallbackProviderExtension : UnityContainerExtension
    {
        #region Const

        ///Used for Resolving the Default Container inside the UnityFallbackProviderStrategy class
        public const string FALLBACK_PROVIDER_NAME = "UnityFallbackProvider";

        #endregion

        #region Overrides of UnityContainerExtension

        /// <summary>
        /// Initializes the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="T:Microsoft.Practices.Unity.ExtensionContext" /> by adding strategies, policies, etc. to
        /// install it's functions into the container.</remarks>
        protected override void Initialize()
        {
            // Register the default IServiceProvider with a name.
            // Now the UnityFallbackProviderStrategy can Resolve the default Provider if needed
            Context.Container.RegisterInstance(FALLBACK_PROVIDER_NAME, Core.ServiceProvider);

            // Create the UnityFallbackProviderStrategy with our UnityContainer
            var strategy = new UnityFallbackProviderStrategy(Context.Container);

            // Adding the UnityFallbackProviderStrategy to be executed with the PreCreation LifeCycleHook
            // PreCreation because if it isnt registerd with the IUnityContainer there will be an Exception
            // Now if the IUnityContainer "magically" gets a Instance of a Type it will accept it and move on
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }

        #endregion
    }

    class UnityFallbackProviderStrategy : BuilderStrategy
    {
        private IUnityContainer _container;

        public UnityFallbackProviderStrategy(IUnityContainer container)
        {
            _container = container;
        }

        #region Overrides of BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            // Checking if the Type we are resolving is registered with the Container
            if (!_container.IsRegistered(context.Type) && Core.ServiceProvider != null)
            {
                // If not we first get our default IServiceProvider and then try to resolve the type with it
                // Then we save the Type in the Existing Property of IBuilderContext to tell Unity
                // that it doesnt need to resolve the Type
                IServiceProvider provider = _container.Resolve<IServiceProvider>(UnityFallbackProviderExtension.FALLBACK_PROVIDER_NAME);

                context.Existing = provider.GetService(context.Type);
            }

            // Otherwise we do the default stuff
            try
            {
                base.PreBuildUp(ref context);
            }
            catch (Exception ex)
            {
                Core.Log.Debug($"Failed to resolve {context.RegistrationType}\n{ex}");
            }
            
        }

        #endregion
    }
}