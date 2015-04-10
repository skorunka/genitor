// ReSharper disable CheckNamespace
namespace Genitor.Library.Core
// ReSharper restore CheckNamespace
{
	using System;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Contains extension methods of <see cref="ReaderWriterLockSlim"/>.
	/// </summary>
	public static class ReaderWriterLockSlimExtensions
	{
		/// <summary>
		/// Starts thread safe read write code block.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static IDisposable ReadAndWrite(this ReaderWriterLockSlim instance)
		{
			Guard.IsNotNull(instance, "instance");

			instance.EnterUpgradeableReadLock();

			return new DisposableCodeBlock(instance.ExitUpgradeableReadLock);
		}

		/// <summary>
		/// Starts thread safe read code block.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static IDisposable Read(this ReaderWriterLockSlim instance)
		{
			Guard.IsNotNull(instance, "instance");

			instance.EnterReadLock();

			return new DisposableCodeBlock(instance.ExitReadLock);
		}

		/// <summary>
		/// Starts thread safe write code block.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static IDisposable Write(this ReaderWriterLockSlim instance)
		{
			Guard.IsNotNull(instance, "instance");

			instance.EnterWriteLock();

			return new DisposableCodeBlock(instance.ExitWriteLock);
		}

		private sealed class DisposableCodeBlock : IDisposable
		{
			private readonly Action _action;

			[DebuggerStepThrough]
			public DisposableCodeBlock(Action action)
			{
				this._action = action;
			}

			[DebuggerStepThrough]
			public void Dispose()
			{
				this._action();
			}
		}
	}
}