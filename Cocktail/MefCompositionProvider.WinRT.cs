// ====================================================================================================================
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
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Windows.ApplicationModel;
using System.Text.RegularExpressions;


namespace Cocktail
{
    /// <summary>
    ///   An implementation of <see cref="ICompositionProvider" /> which uses MEF as the underlying IoC implementation.
    /// </summary>
    internal partial class MefCompositionProvider : ICompositionProvider
    {
        private readonly ValueExportDescriptorProvider _valueExports = new ValueExportDescriptorProvider();
        private ContainerConfiguration _configuration;
        private CompositionHost _container;
        private ConventionBuilder _conventions;
        private IEnumerable<Assembly> _probeAssemblies;

        private ConventionBuilder Conventions
        {
            get { return _conventions ?? (_conventions = new ConventionBuilder()); }
        }

        public IEnumerable<Assembly> ProbeAssemblies
        {
            get
            {
                if (_probeAssemblies == null)
                    _probeAssemblies = GetAssemblies();

                return _probeAssemblies;
            }
        }

        public ContainerConfiguration Configuration
        {
            get
            {
                if (_configuration != null)
                    return _configuration;

                // Add conventions for Cocktail extensions.
                Conventions
                    .ForTypesDerivedFrom<IDiscoverableViewModel>()
                    .Export<IDiscoverableViewModel>();
                Conventions
                    .ForType<EventAggregator>()
                    .Export<IEventAggregator>()
                    .Shared();

                // Build ContainerConfiguration with the list of assemblies discovered by DevForce
                var assemblies = ProbeAssemblies.ToList();
                _configuration = new ContainerConfiguration()
                    .WithProvider(_valueExports) // Provider for manually injected singletons
                    .WithAssemblies(assemblies, Conventions);

                // Ensure Cocktail assembly is part of the ContainerConfiguration.
                if (!assemblies.Contains(GetType().GetTypeInfo().Assembly))
                    _configuration = _configuration.WithAssembly(GetType().GetTypeInfo().Assembly, Conventions);
                return _configuration;
            }
        }

        public CompositionHost Container
        {
            get { return _container ?? (_container = Configuration.CreateContainer()); }
        }

        #region ICompositionProvider Members

        public Lazy<T> GetInstance<T>() where T : class
        {
            return new Lazy<T>(() => Container.GetExport<T>());
        }

        public IEnumerable<T> GetInstances<T>() where T : class
        {
            return Container.GetExports<T>();
        }

        public Lazy<object> GetInstance(Type serviceType, string contractName)
        {
            return new Lazy<object>(() => Container.GetExport(serviceType, contractName));
        }

        public IEnumerable<object> GetInstances(Type serviceType, string contractName)
        {
            return Container.GetExports(serviceType, contractName);
        }

        public ICompositionFactory<T> GetInstanceFactory<T>() where T : class
        {
            var factory = new MefCompositionFactory<T>();
            Container.SatisfyImports(factory);
            if (factory.ExportFactory == null)
                throw new CompositionFailedException(string.Format(StringResources.NoExportFound, typeof(T)));

            return factory;
        }

        public void BuildUp(object instance)
        {
            // Skip if in design mode.
            if (DesignTime.InDesignMode())
                return;

            Container.SatisfyImports(instance);
        }

        public T TryGetInstance<T>() where T : class
        {
            T instance;
            if (!Container.TryGetExport(out instance))
                return null;

            return instance;
        }

        public object TryGetInstance(Type serviceType, string contractName)
        {
            object instance;
            if (!Container.TryGetExport(serviceType, contractName, out instance))
                return null;

            return instance;
        }

        public ICompositionFactory<T> TryGetInstanceFactory<T>() where T : class
        {
            var factory = new MefCompositionFactory<T>();
            Container.SatisfyImports(factory);
            if (factory.ExportFactory == null)
                return null;

            return factory;
        }

        #endregion

        public void Configure(ConventionBuilder conventions)
        {
            _conventions = conventions;
        }

        public void AddExportedValue<T>(T value)
        {
            _valueExports.AddExportedValue(value);
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            var ignorePatterns = new[] {"^System.", "^Microsoft.", "^Caliburn.Micro", "^Windows."};

            var task = Package.Current.InstalledLocation.GetFilesAsync().AsTask();
            task.Wait();
            if (task.IsFaulted || task.IsCanceled)
                return Enumerable.Empty<Assembly>();

            var selectFiles = task.Result.Where(f => f.Name.EndsWith(".dll")).Select(f => f.Name);

            var list = new List<Assembly>();
            foreach (var file in selectFiles)
            {
                var name = file.Remove(file.LastIndexOf('.'));

                if (ignorePatterns.Any(p => Regex.IsMatch(name, p, RegexOptions.IgnoreCase)))
                    continue;
                
                var assemblyName = new AssemblyName(name);
                var asm = Assembly.Load(assemblyName);
                list.Add(asm);
            }

            return list;
        }
    }

    internal class ValueExportDescriptorProvider : ExportDescriptorProvider
    {
        private readonly Dictionary<Type, object> _exportedValues;

        public ValueExportDescriptorProvider()
        {
            _exportedValues = new Dictionary<Type, object>();
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(
            CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (!contract.Equals(new CompositionContract(contract.ContractType)))
                return NoExportDescriptors;

            if (!_exportedValues.ContainsKey(contract.ContractType))
                return NoExportDescriptors;

            return new[]
                       {
                           new ExportDescriptorPromise(
                               contract, "CompositionProvider", true, NoDependencies,
                               _ => ExportDescriptor.Create(
                                   (c, o) => _exportedValues[contract.ContractType], NoMetadata))
                       };
        }

        public void AddExportedValue<T>(T value)
        {
            _exportedValues.Add(typeof(T), value);
        }
    }
}