﻿// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    ///   An implementation of <see cref="ICompositionProvider" /> which uses MEF as the underlying IoC implementation.
    /// </summary>
    internal partial class MefCompositionProvider : ISupportsRecomposition
    {
        private ComposablePartCatalog _pluginCatalog;
        private ComposablePartCatalog _defaultCatalog;
        private ComposablePartCatalog _catalog;
        private CompositionContainer _container;

        /// <summary>
        ///   Returns the current catalog in use.
        /// </summary>
        /// <returns> Unless a custom catalog is provided through <see cref="Configure" /> , this property returns <see
        ///    cref="DefaultCatalog" /> </returns>
        public ComposablePartCatalog Catalog
        {
            get { return _catalog ?? DefaultCatalog; }
        }

        /// <summary>
        ///   Returns the default catalog in use by DevForce.
        /// </summary>
        public ComposablePartCatalog DefaultCatalog
        {
            get { return _defaultCatalog ?? (_defaultCatalog = CreateDefaultCatalog()); }
        }

        /// <summary>
        ///    Returns the plugin catalog in use.
        /// </summary>
        public ComposablePartCatalog PluginCatalog
        {
            get { return _pluginCatalog ?? (_pluginCatalog = CreatePluginCatalog()); }
        }

        /// <summary>
        ///   Returns the CompositionContainer in use.
        /// </summary>
        public CompositionContainer Container
        {
            get
            {
                if (_container != null)
                    return _container;

                var exportProviders = new List<CatalogExportProvider>();
                if (PluginCatalog != null && PluginCatalog.Parts.Any())
                    exportProviders.Add(new CatalogExportProvider(PluginCatalog));

                exportProviders.Add(new CatalogExportProvider(Catalog));
                _container = new CompositionContainer(exportProviders.Cast<ExportProvider>().ToArray());
                exportProviders.ForEach(ep => ep.SourceProvider = _container);
                return _container;
            }
        }

        #region ISupportsRecomposition Members

        /// <summary>
        ///   Returns a lazy instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <returns> The requested instance. </returns>
        public Lazy<T> GetInstance<T>() where T : class
        {
            var exports = GetExportsCore(typeof (T), null).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  typeof (T).FullName));

            return new Lazy<T>(() => (T) exports.First().Value);
        }

        public T TryGetInstance<T>() where T : class
        {
            if (!IsTypeRegistered<T>())
                return null;

            return GetInstance<T>().Value;
        }

        /// <summary>
        ///   Returns all instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instances. </typeparam>
        /// <returns> The requested instances. </returns>
        public IEnumerable<T> GetInstances<T>() where T : class
        {
            var exports = GetExportsCore(typeof (T), null);
            return exports.Select(x => (T) x.Value);
        }

        /// <summary>
        ///   Returns a lazy instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used. </param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <returns> The requested instance. </returns>
        public Lazy<object> GetInstance(Type serviceType, string contractName)
        {
            var exports = GetExportsCore(serviceType, contractName).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  serviceType != null ? serviceType.ToString() : contractName));

            return new Lazy<object>(() => exports.First().Value);
        }

        public object TryGetInstance(Type serviceType, string contractName)
        {
            var exports = GetExportsCore(serviceType, contractName).ToList();
            if (!exports.Any())
                return null;

            return exports.First().Value;
        }

        /// <summary>
        ///   Returns all lazy instances of the provided type.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used. </param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <returns> The requested instances. </returns>
        public IEnumerable<object> GetInstances(Type serviceType, string contractName)
        {
            var exports = GetExportsCore(serviceType, contractName);
            return exports.Select(x => x.Value);
        }

        public ICompositionFactory<T> GetInstanceFactory<T>() where T : class
        {
            var factory = new MefCompositionFactory<T>();
            Container.SatisfyImportsOnce(factory);
            if (factory.ExportFactory == null)
                throw new CompositionException(string.Format(StringResources.NoExportFound, typeof (T)));

            return factory;
        }

        public ICompositionFactory<T> TryGetInstanceFactory<T>() where T : class
        {
            var factory = new MefCompositionFactory<T>();
            Container.SatisfyImportsOnce(factory);
            if (factory.ExportFactory == null)
                return null;

            return factory;
        }

        /// <summary>
        ///   Manually performs property dependency injection on the provided instance.
        /// </summary>
        /// <param name="instance"> The instance needing property injection. </param>
        public void BuildUp(object instance)
        {
            // Skip if in design mode.
            if (DesignTime.InDesignMode())
                return;

            Container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        ///   Returns true if the CompositionProvider is currently in the process of recomposing.
        /// </summary>
        public bool IsRecomposing { get; internal set; }

        /// <summary>
        ///   Fired when the composition container is modified after initialization.
        /// </summary>
        public event EventHandler Recomposed;

        #endregion

        /// <summary>
        ///   Returns true if the provided type has been previously registered.
        /// </summary>
        internal bool IsTypeRegistered<T>() where T : class
        {
            return Container.GetExports<T>().Any();
        }

        /// <summary>
        ///   Configures the CompositionHost.
        /// </summary>
        /// <param name="compositionBatch"> Optional changes to the <see cref="CompositionContainer" /> to include during the composition. </param>
        /// <param name="catalog"> The custom catalog to be used by Cocktail to get access to MEF exports. </param>
        public void Configure(CompositionBatch compositionBatch = null, ComposablePartCatalog catalog = null)
        {
            _catalog = catalog;

            var batch = compositionBatch ?? new CompositionBatch();
            if (!IsTypeRegistered<IEventAggregator>())
                batch.AddExportedValue<IEventAggregator>(new EventAggregator());

            Compose(batch);
        }

        /// <summary>
        ///   Executes composition on the container, including the changes in the specified <see cref="CompositionBatch" /> .
        /// </summary>
        /// <param name="compositionBatch"> Changes to the <see cref="CompositionContainer" /> to include during the composition. </param>
        public void Compose(CompositionBatch compositionBatch)
        {
            if (compositionBatch == null)
                throw new ArgumentNullException("compositionBatch");

            Container.Compose(compositionBatch);
        }

        private IEnumerable<Lazy<object>> GetExportsCore(Type serviceType, string key)
        {
            return Container.GetExports(serviceType, null, key);
        }

        private ComposablePartCatalog CreateDefaultCatalog()
        {
#if SILVERLIGHT
            var aggregateCatalog = new AggregateCatalog();

            if (!DesignTime.InDesignMode())
            {
                var parts = Deployment.Current.Parts.Where(x => x.Source.EndsWith(".dll"));
                var assemblies = new List<Assembly>();

                // Load assemblies from current xap
                foreach (var part in parts)
                    using (var stream = Application.GetResourceStream(new Uri(part.Source, UriKind.Relative)).Stream)
                        assemblies.Add(part.Load(stream));

                // Create an AssemblyCatalog for every assembly and add to aggregate catalog
                foreach (var asm in assemblies)
                {
                    var assemblyCatalog = new AssemblyCatalog(asm);
                    if (assemblyCatalog.Parts.Any())
                        aggregateCatalog.Catalogs.Add(assemblyCatalog);
                    else
                        assemblyCatalog.Dispose();
                }
            }

            return aggregateCatalog;
#else
            var aggregateCatalog = new AggregateCatalog();

            if (!DesignTime.InDesignMode())
            {
                var searchPatterns = new[] {"*.dll", "*.exe"};

                var catalogs = searchPatterns.SelectMany(
                    searchPattern => Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern,
                                                        SearchOption.TopDirectoryOnly)
                                              .Select(codeBase => new AssemblyCatalog(codeBase)));

                foreach (var assemblyCatalog in catalogs)
                {
                    if (assemblyCatalog.Parts.Any())
                        aggregateCatalog.Catalogs.Add(assemblyCatalog);
                    else
                        assemblyCatalog.Dispose();
                }

            }

            return aggregateCatalog;
#endif
        }

        private ComposablePartCatalog CreatePluginCatalog()
        {
#if SILVERLIGHT
            throw new NotImplementedException();
#else 
            var baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var pluginPath = Path.Combine((baseDir.Parent ?? baseDir).FullName, "Plugins");
            return Directory.Exists(pluginPath) ? new DirectoryCatalog(pluginPath) : null;
#endif
        }
    }
}