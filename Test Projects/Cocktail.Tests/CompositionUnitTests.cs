//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using Cocktail.Tests.Helpers;
#if !NETFX_CORE
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Cocktail.Tests
{
    [TestClass]
    public class CompositionUnitTests : CocktailTestBase
    {
        [TestMethod]
        public void ShouldReturnSharedInstance()
        {
            var instance1 = Composition.GetInstance<SharedObject>();
            var instance2 = Composition.GetInstance<SharedObject>();

            Assert.IsTrue(ReferenceEquals(instance1, instance2), "Should have the same instance");
        }

        [TestMethod]
        public void ShouldReturnNonSharedInstance()
        {
            var instance1 = Composition.GetInstance<NonSharedObject>();
            var instance2 = Composition.GetInstance<NonSharedObject>();

            Assert.IsFalse(ReferenceEquals(instance1, instance2), "Should have two separate instances");
        }

        [TestMethod]
        public void ShouldUseFactory()
        {
            var partLocator = new PartLocator<NonSharedObject>(true);
            var instance = partLocator.GetPart();

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void ObjectManagerShouldCreateObject()
        {
            var objectManager = new ObjectManager<Guid, NonSharedObject>();

            var key = Guid.NewGuid();
            var obj = objectManager.GetObject(key);
            Assert.IsNotNull(obj);

            obj = objectManager.TryGetObject(key);
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void ObjectManagerShouldReturnNullIfObjectDoesntExist()
        {
            var objectManager = new ObjectManager<Guid, NonSharedObject>();
            var obj = objectManager.TryGetObject(Guid.NewGuid());
            Assert.IsNull(obj);
        }

        [TestMethod]
        public void ShouldGetGenericInstance()
        {
            var instance = Composition.TryGetInstance<GenericExport<object>>();

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void ShouldGetGenericInstanceWithContractName()
        {
            var instance = Composition.TryGetInstance(typeof(GenericExportWithTypeAndContractName<object>),
                                                      "ExportWithTypeAndContract");

            
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance is GenericExportWithTypeAndContractName<object>);
        }
    }
}