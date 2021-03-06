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
using System.Collections.Generic;
using Ninject.Core.Infrastructure;
#endregion

namespace Ninject.Core.Logging
{
	/// <summary>
	/// A baseline definition of a logger factory, which tracks loggers as flyweights by type.
	/// Custom logger factories should generally extend this type.
	/// </summary>
	public abstract class LoggerFactoryBase : KernelComponentBase, ILoggerFactory
	{
		/*----------------------------------------------------------------------------------------*/
		#region Fields
		private readonly Dictionary<Type, ILogger> _loggers = new Dictionary<Type, ILogger>();
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Disposal
		/// <summary>
		/// Releases all resources held by the object.
		/// </summary>
		/// <param name="disposing"><see langword="True"/> if managed objects should be disposed, otherwise <see langword="false"/>.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
			{
				DisposeDictionary(_loggers);
				_loggers.Clear();
			}
				
			base.Dispose(disposing);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Public Methods
		/// <summary>
		/// Gets the logger for the specified type, creating it if necessary.
		/// </summary>
		/// <param name="type">The type to create the logger for.</param>
		/// <returns>The newly-created logger.</returns>
		public ILogger GetLogger(Type type)
		{
			lock (_loggers)
			{
				if (_loggers.ContainsKey(type))
					return _loggers[type];

				ILogger logger = CreateLogger(type);
				_loggers.Add(type, logger);

				return logger;
			}
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Protected Methods
		/// <summary>
		/// Creates a logger for the specified type.
		/// </summary>
		/// <param name="type">The type to create the logger for.</param>
		/// <returns>The newly-created logger.</returns>
		protected abstract ILogger CreateLogger(Type type);
		#endregion
		/*----------------------------------------------------------------------------------------*/
	}
}