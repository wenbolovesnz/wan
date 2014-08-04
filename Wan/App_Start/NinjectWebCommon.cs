using System.Web.Http;
using FormBuilder.Business.Entities;
using FormBuilder.Data;
using FormBuilder.Data.Contracts;
using FormBuilder.Data.Data_Repositories;
using WebApiContrib.IoC.Ninject;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Wan.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Wan.App_Start.NinjectWebCommon), "Stop")]

namespace Wan.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                //Add support for Ninject & WebApi to work together
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<FormBuilderContext>().To<FormBuilderContext>().InRequestScope();
            kernel.Bind<IApplicationUnit>().To<ApplicationUnit>().InRequestScope();
            kernel.Bind<IGenericRepository<User>>().To<GenericRepository<User>>().InRequestScope();
            kernel.Bind<IGenericRepository<Role>>().To<GenericRepository<Role>>().InRequestScope();
            kernel.Bind<IGenericRepository<Group>>().To<GroupsRepository>().InRequestScope();
            kernel.Bind<IGenericRepository<Event>>().To<GenericRepository<Event>>().InRequestScope();
            kernel.Bind<IGenericRepository<Sponsor>>().To<GenericRepository<Sponsor>>().InRequestScope();
            kernel.Bind<IGenericRepository<JoinGroupRequest>>().To<GenericRepository<JoinGroupRequest>>().InRequestScope();
            kernel.Bind<IGenericRepository<GroupPhoto>>().To<GenericRepository<GroupPhoto>>().InRequestScope();
            kernel.Bind<IGenericRepository<PersonalMessage>>().To<GenericRepository<PersonalMessage>>().InRequestScope();
        }        
    }
}
