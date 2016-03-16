using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Svyaznoy.Core.MVC
{
    public abstract class MefHttpApplication : HttpApplication
    {
        private CompositionContainer _container;

        public void Application_Start()
        {
            var catalog = CreateCatalog();
            _container = new CompositionContainer(catalog, true);

            var batch = CreateCompositionBatch();
            _container.Compose(batch);

            ControllerBuilder.Current.SetControllerFactory(new MefControllerFactory(_container));

            OnApplicationStarted();
        }

        protected virtual CompositionBatch CreateCompositionBatch()
        {
            return null;
        }

        protected virtual void OnApplicationStarted() { }

        protected abstract ComposablePartCatalog CreateCatalog();
    }

    public class MefControllerFactory : DefaultControllerFactory
    {
        private CompositionContainer container;

        public MefControllerFactory(CompositionContainer container)
        {
            this.container = container;
        }

        public override IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            IController controller = base.CreateController(requestContext, controllerName);

            container.SatisfyImportsOnce(controller);

            return controller;
        }
    }
}