namespace MorganDI
{
    public delegate void ServiceFactoryConfigurationHandler(IServiceFactory serviceFactory);

    /// <summary>
    /// Provides the ability to instantiate a service.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Bind the specified construction parameter to a service registered in the service collection.
        /// </summary>
        /// <param name="parameterName">The name of the construction parameter.</param>
        /// <param name="serviceIdentifier">The service to associate with the parameter.</param>
        void BindParameter(string parameterName, ServiceIdentifier serviceIdentifier);

        /// <summary>
        /// Bind the specified construction parameter to the result of the supplied delegate.
        /// </summary>
        /// <typeparam name="TService">The return type of the delegate.</typeparam>
        /// <param name="parameterName">The name of the construction parameter.</param>
        /// <param name="serviceDelegate">The delegate to invoke to populate the parameter.</param>
        void BindParameter<TService>(string parameterName, ServiceDelegate<TService> serviceDelegate);

        /// <summary>
        /// Bind the specified construction parameter to the supplied static value.
        /// </summary>
        /// <param name="parameterName">The name of the construction parameter.</param>
        /// <param name="value">The static value to provide to the parameter during construction.</param>
        void BindParameter(string parameterName, object value);
    }
}
