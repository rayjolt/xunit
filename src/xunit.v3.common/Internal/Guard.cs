﻿#nullable enable  // This file is temporarily shared with xunit.v1.tests and xunit.v2.tests, which are not nullable-enabled

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace Xunit.Internal
{
	/// <summary>
	/// Helper class for guarding value arguments and valid state.
	/// </summary>
	public static class Guard
	{
		/// <summary>
		/// Ensures that an enum value is valid by comparing against a list of valid values.
		/// </summary>
		/// <typeparam name="T">The argument type</typeparam>
		/// <param name="argValue">The value of the argument</param>
		/// <param name="validValues">The list of valid values</param>
		/// <param name="argName">The name of the argument</param>
		/// <exception cref="ArgumentException"></exception>
		public static T ArgumentEnumValid<T>(
			T argValue,
			HashSet<T> validValues,
			[CallerArgumentExpression("argValue")] string? argName = null)
				where T : Enum
		{
			if (!validValues.Contains(argValue))
				throw new ArgumentException($"Enum value {argValue} not in valid set: [{string.Join(",", validValues)}]", argName?.TrimStart('@'));

			return argValue;
		}

		/// <summary>
		/// Ensures that a nullable value type argument is not null.
		/// </summary>
		/// <typeparam name="T">The argument type</typeparam>
		/// <param name="argValue">The value of the argument</param>
		/// <param name="argName">The name of the argument</param>
		/// <returns>The argument value as a non-null value</returns>
		/// <exception cref="ArgumentNullException">Thrown when the argument is null</exception>
		public static T ArgumentNotNull<T>(
			[NotNull] T? argValue,
			[CallerArgumentExpression("argValue")] string? argName = null)
				where T : struct
		{
			if (!argValue.HasValue)
				throw new ArgumentNullException(argName?.TrimStart('@'));

			return argValue.Value;
		}

		/// <summary>
		/// Ensures that a nullable reference type argument is not null.
		/// </summary>
		/// <typeparam name="T">The argument type</typeparam>
		/// <param name="argValue">The value of the argument</param>
		/// <param name="argName">The name of the argument</param>
		/// <returns>The argument value as a non-null value</returns>
		/// <exception cref="ArgumentNullException">Thrown when the argument is null</exception>
		public static T ArgumentNotNull<T>(
			[NotNull] T? argValue,
			[CallerArgumentExpression("argValue")] string? argName = null)
				where T : class
		{
			if (argValue == null)
				throw new ArgumentNullException(argName?.TrimStart('@'));

			return argValue;
		}

		/// <summary>
		/// Ensures that a nullable reference type argument is not null.
		/// </summary>
		/// <typeparam name="T">The argument type</typeparam>
		/// <param name="message">The exception message to use when the argument is null</param>
		/// <param name="argValue">The value of the argument</param>
		/// <param name="argName">The name of the argument</param>
		/// <returns>The argument value as a non-null value</returns>
		/// <exception cref="ArgumentNullException">Thrown when the argument is null</exception>
		public static T ArgumentNotNull<T>(
			string message,
			[NotNull] T? argValue,
			string? argName = null)
				where T : class
		{
			if (argValue == null)
				throw new ArgumentNullException(argName, message);

			return argValue;
		}

		/// <summary>
		/// Ensures that a nullable enumerable type argument is not null or empty.
		/// </summary>
		/// <typeparam name="T">The argument type</typeparam>
		/// <param name="argValue">The value of the argument</param>
		/// <param name="argName">The name of the argument</param>
		/// <returns>The argument value as a non-null, non-empty value</returns>
		/// <exception cref="ArgumentException">Thrown when the argument is null or empty</exception>
		public static T ArgumentNotNullOrEmpty<T>(
			[NotNull] T? argValue,
			[CallerArgumentExpression("argValue")] string? argName = null)
				where T : class, IEnumerable
		{
			ArgumentNotNull(argValue, argName);

			if (!argValue.GetEnumerator().MoveNext())
				throw new ArgumentException("Argument was empty", argName?.TrimStart('@'));

			return argValue;
		}

		/// <summary>
		/// Ensures that a nullable enumerable type argument is not null or empty.
		/// </summary>
		/// <typeparam name="T">The argument type</typeparam>
		/// <param name="message">The exception message to use when the argument is null or empty</param>
		/// <param name="argValue">The value of the argument</param>
		/// <param name="argName">The name of the argument</param>
		/// <returns>The argument value as a non-null, non-empty value</returns>
		/// <exception cref="ArgumentException">Thrown when the argument is null or empty</exception>
		public static T ArgumentNotNullOrEmpty<T>(
			string message,
			[NotNull] T? argValue,
			string? argName = null)
				where T : class, IEnumerable
		{
			if (argValue == null || !argValue.GetEnumerator().MoveNext())
				throw new ArgumentException(message, argName);

			return argValue;
		}

		/// <summary>
		/// Ensures that an argument is valid.
		/// </summary>
		/// <param name="message">The exception message to use when the argument is not valid</param>
		/// <param name="test">The validity test value</param>
		/// <param name="argName">The name of the argument</param>
		/// <returns>The argument value as a non-null value</returns>
		/// <exception cref="ArgumentException">Thrown when the argument is not valid</exception>
		public static void ArgumentValid(
			string message,
			bool test,
			string? argName = null)
		{
			if (!test)
				throw new ArgumentException(message, argName);
		}

		/// <summary>
		/// Ensures that a filename argument is not null or empty, and that the file exists on disk.
		/// </summary>
		/// <param name="fileName">The file name value</param>
		/// <param name="argName">The name of the argument</param>
		/// <returns>The file name as a non-null value</returns>
		/// <exception cref="ArgumentException">Thrown when the argument is null, empty, or not on disk</exception>
		public static string FileExists(
			[NotNull] string? fileName,
			[CallerArgumentExpression("fileName")] string? argName = null)
		{
			ArgumentNotNullOrEmpty(fileName, argName);
			ArgumentValid($"File not found: {fileName}", File.Exists(fileName), argName?.TrimStart('@'));

			return fileName;
		}

		/// <summary>
		/// Ensures that a value is not default value. This is used for values of generic types
		/// where nullability is not known.
		/// </summary>
		/// <typeparam name="T">The argument type</typeparam>
		/// <param name="argValue">The value of the argument</param>
		/// <param name="argName">The name of the argument</param>
		/// <returns>The argument value as a non-default value</returns>
		/// <exception cref="ArgumentNullException">Thrown when the argument is default</exception>
		public static T GenericArgumentNotNull<T>(
			[NotNull] T? argValue,
			[CallerArgumentExpression("argValue")] string? argName = null)
		{
			if (argValue == null)
				throw new ArgumentNullException(argName?.TrimStart('@'));

			return argValue;
		}

		/// <summary>
		/// Ensure that a value is not null.
		/// </summary>
		/// <typeparam name="T">The value type</typeparam>
		/// <param name="message">The exception message to use when the value is not valid</param>
		/// <param name="value">The value to test for null</param>
		/// <returns>The value as a non-null value</returns>
		/// <exception cref="InvalidOperationException">Thrown when the value is not valid</exception>
		public static T NotNull<T>(
			string message,
			[NotNull] T? value)
				where T : class
		{
			if (value == null)
				throw new InvalidOperationException(message);

			return value;
		}
	}
}

#if !NETCOREAPP3_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class CallerArgumentExpressionAttribute : Attribute
	{
		public CallerArgumentExpressionAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		public string ParameterName { get; }
	}
}

#endif
