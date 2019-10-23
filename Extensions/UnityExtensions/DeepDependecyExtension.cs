using Microsoft.Extensions.Logging;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;

namespace Mel.Live.Extensions.UnityExtensions
{
    #region Extension
    internal class DeepDependencyExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Strategies.Add(new DeepDependencyStrategy(),
                UnityBuildStage.Initialization);
        }
    }
    #endregion

    #region Attributes
    /// <summary>
    /// Automatically resolves a property on initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DeepDependency : Attribute
    {
        public Type TargetType { get; set; } = null;
        public string TargetProperty { get; set; }
    }
    #endregion

    #region Strategy
    class DeepDependencyStrategy : BuilderStrategy
    {
        public override void PostBuildUp(ref BuilderContext context)
        {
            if (context.Type == typeof(object)) return;
            var container = context.Container;

            var properties = context.Existing.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.GetProperty | BindingFlags.SetProperty |
                BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(DeepDependency), false)
                .Any());

            foreach (var prop in properties)
            {
                object value = null;
                DeepDependency attribute = prop.GetCustomAttribute<DeepDependency>(false);

                // Resolve ILogger by simply creating it based on the current class
                if (!prop.PropertyType.IsGenericType && prop.PropertyType.IsAssignableTo<ILogger>())
                    value = container.TryResolve<ILoggerFactory>().CreateLogger(context.Type);
                else if (!string.IsNullOrWhiteSpace(attribute.TargetProperty) && attribute.TargetType != null)
                {
                    var target = container.TryResolve(attribute.TargetType);

                    try
                    {
                        value = attribute.TargetType.GetProperty(attribute.TargetProperty).GetValue(target);
                    }
                    catch (Exception ex)
                    {
                        Core.Log.Error($"An error occured while attempting to resolve " +
                            $"a target type.\n{ex}");
                    }
                }
                else value = context.Container.TryResolve(prop.PropertyType);

                try
                {
                    if (prop.CanWrite) prop.SetValue(context.Existing, value);
                    else prop.GetBackingField().SetValue(context.Existing, value);
                }
                catch (Exception ex)
                {
                    Core.Log.Error($"An error occured during deep property resolutiion\n{ex}");
                }
            }
        }
    }
    #endregion
}
