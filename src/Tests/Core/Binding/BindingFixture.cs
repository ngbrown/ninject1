#region License
//
// Author: Nate Kohari <nkohari@gmail.com>
// Copyright (c) 2007-2008, Enkari, Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
#region Using Directives
using System;
using Ninject.Core;
using Ninject.Core.Activation;
using Ninject.Core.Behavior;
using Ninject.Core.Binding;
using Ninject.Core.Creation.Providers;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
#endregion

namespace Ninject.Tests.Binding
{
	[TestFixture]
	public class BindingFixture
	{
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void CanBindType()
		{
			var module = new InlineModule(m => m.Bind<IMock>().To<SimpleObject>());

			using (var kernel = new StandardKernel(module))
			{
				IContext context = kernel.Components.ContextFactory.Create(typeof(IMock));
				IBinding binding = kernel.Components.BindingSelector.SelectBinding(typeof(IMock), context);

				var provider = binding.Provider as StandardProvider;
				Assert.That(binding, Is.Not.Null);
				Assert.That(provider, Is.Not.Null);

				Assert.That(provider.GetImplementationType(context), Is.EqualTo(typeof(SimpleObject)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void CanBindConstant()
		{
			var module = new InlineModule(m => m.Bind<string>().ToConstant("test"));

			using (var kernel = new StandardKernel(module))
			{
				IContext context = kernel.Components.ContextFactory.Create(typeof(string));
				IBinding binding = kernel.Components.BindingSelector.SelectBinding(typeof(string), context);

				var provider = binding.Provider as ConstantProvider;
				Assert.That(binding, Is.Not.Null);
				Assert.That(provider, Is.Not.Null);

				Assert.That(provider.GetImplementationType(context), Is.EqualTo(typeof(string)));
				Assert.That(provider.Value, Is.EqualTo("test"));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test, ExpectedException(typeof(NotSupportedException))]
		public void DefiningMultipleDefaultBindingsThrowsException()
		{
			var module = new InlineModule(
				m => m.Bind(typeof(IMock)).To(typeof(ImplA)),
				m => m.Bind(typeof(IMock)).To(typeof(ImplB))
			);

			var kernel = new StandardKernel(module);
		}
		/*----------------------------------------------------------------------------------------*/
		[Test, ExpectedException(typeof(NotSupportedException))]
		public void MultipleInjectionConstructorsThrowsException()
		{
			using (var kernel = new StandardKernel())
			{
				kernel.Get<ObjectWithMultipleInjectionConstructors>();
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test, ExpectedException(typeof(ActivationException))]
		public void IncompatibleProviderAndBindingServiceTypeThrowsException()
		{
			var module = new InlineModule(m => m.Bind(typeof(IMock)).To(typeof(ObjectWithNoInterfaces)));
			var options = new KernelOptions { IgnoreProviderCompatibility = false };

			using (var kernel = new StandardKernel(options, module))
			{
				kernel.Get<IMock>();
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void IncompatibleProviderAllowedIfProviderCompatibilityIsIgnored()
		{
			var module = new InlineModule(m => m.Bind(typeof(IMock)).To(typeof(ObjectWithNoInterfaces)));
			var options = new KernelOptions { IgnoreProviderCompatibility = true };

			using (var kernel = new StandardKernel(options, module))
			{
				var mock = kernel.Get(typeof(IMock)) as ObjectWithNoInterfaces;
				Assert.That(mock, Is.Not.Null);
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void CanOverrideBehaviorViaBindingDeclaration()
		{
			var module = new InlineModule(m => m.Bind<IMock>().To<ImplA>().Using<SingletonBehavior>());

			using (var kernel = new StandardKernel(module))
			{
				var mock1 = kernel.Get<IMock>();
				var mock2 = kernel.Get<IMock>();

				Assert.That(mock1, Is.Not.Null);
				Assert.That(mock2, Is.Not.Null);
				Assert.That(mock1, Is.SameAs(mock2));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void IncompleteBindingCausesKernelToThrowException()
		{
			var module = new InlineModule(m => m.Bind<IMock>());
			IKernel kernel = new StandardKernel(module);
		}
		/*----------------------------------------------------------------------------------------*/
        [Test]
        public void CanBindInlineModuleTwoSeperateTimes()
        {
            var moduleA = new InlineModule(m => m.Bind<IMock>().To<ImplA>());
            var moduleB = new InlineModule(m => m.Bind<string>().ToConstant("test"));

            using (var kernel = new StandardKernel(moduleA, moduleB))
            {
				var mock1 = kernel.Get<IMock>();
				var mock2 = kernel.Get<string>();
                
				Assert.That(mock1, Is.Not.Null);
                Assert.That(mock2, Is.EqualTo("test"));
            }

        }
		/*----------------------------------------------------------------------------------------*/
	}
}