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
    #region Extension
    internal class AutomaticBuildExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Strategies.Add(new AutomaticBuildStrategy(),
                UnityBuildStage.Initialization);
        }
    }
    #endregion

    #region Attributes
    /// <summary>
    /// Notifies Unity to automatically build the class once it is initialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoBuild : Attribute { }
    #endregion

    #region Strategy
    class AutomaticBuildStrategy : BuilderStrategy
    {
        public override void PostBuildUp(ref BuilderContext context)
        {
            if (context.BuildComplete) return;
            bool autoBuild = Attribute.IsDefined(context.Type, typeof(AutoBuild));
            if (autoBuild) context.Container.BuildUp(context.Existing);
        }
    }
    #endregion
}
