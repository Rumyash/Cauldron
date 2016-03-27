﻿using Couldron.Behaviours;
using Couldron.Core;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Couldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Performs certain types of conversions between compatible reference types or nullable types
        /// <para/>
        /// Returns null if convertion was not successfull
        /// </summary>
        /// <typeparam name="T">The type to convert the <paramref name="target"/> to</typeparam>
        /// <param name="target">The object to convert</param>
        /// <returns>The converted object</returns>
        public static T CastTo<T>(this object target) where T : class
        {
            return target as T;
        }

        /// <summary>
        /// Copy and modifies the alpha channel of the <see cref="SolidColorBrush"/>'s <see cref="Color"/>
        /// </summary>
        /// <param name="brush">The Solidcolorbrush to copy the color from</param>
        /// <param name="alpha">The new alpha channel of the <see cref="SolidColorBrush"/></param>
        /// <returns>A new instance of the <see cref="SolidColorBrush"/></returns>
        public static SolidColorBrush ChangeAlpha(this SolidColorBrush brush, byte alpha)
        {
            return new SolidColorBrush(new Color { A = alpha, R = brush.Color.R, G = brush.Color.G, B = brush.Color.B });
        }

        /// <summary>
        /// Determines whether an element is in the array
        /// </summary>
        /// <typeparam name="T">The type of elements in the array</typeparam>
        /// <param name="array">The array that could contain the item</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if item is found in the array; otherwise, false.</returns>
        public static bool Contains<T>(this T[] array, Func<T, bool> predicate)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (predicate(array[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="target">The <see cref="IEnumerable"/></param>
        /// <returns>The total count of items in the <see cref="IEnumerable"/></returns>
        public static int Count(this IEnumerable target)
        {
            int count = 0;

            foreach (var item in target)
                count++;

            return count;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// <para/>
        /// This will dispose an object if it implements the <see cref="IDisposable "/> interface.
        /// <para/>
        /// If the object is a <see cref="FrameworkElement"/> it will try to find known diposable attached properties.
        /// <para />
        /// It will also dispose the the <see cref="FrameworkElement.DataContext"/> content.
        /// </summary>
        /// <param name="context">The object to dispose</param>
        public static void DisposeAll(this object context)
        {
            if (context == null)
                return;

            // dispose the diposables
            (context as IDisposable).IsNotNull(x => x.Dispose());
            (context as FrameworkElement).IsNotNull(x =>
            {
                // Dispose the attach behaviours
                foreach (var child in x.FindVisualChildren<FrameworkElement>())
                    child.DisposeAll();

                Interaction.GetBehaviours(x).IsNotNull(o => o.Dispose());

                // Dispose the datacontext
                x.DataContext.DisposeAll();
            });
        }

        /// <summary>
        /// Gets a specified length of bytes.
        /// <para />
        /// If the specified length <paramref name="length"/> is longer than the source array the source array will be returned instead.
        /// </summary>
        /// <param name="target">The Array that contains the data to copy.</param>
        /// <param name="length">A 32-bit integer that represents the number of elements to copy.</param>
        /// <returns>Returns an array of bytes</returns>
        public static byte[] GetBytes(this byte[] target, int length)
        {
            if (length >= target.Length)
                return target;

            byte[] value = new byte[length];

            Array.Copy(target, value, length);
            return value;
        }

        /// <summary>
        /// Gets a specified length of bytes
        /// </summary>
        /// <param name="target">The Array that contains the data to copy.</param>
        /// <param name="startingPosition">A 32-bit integer that represents the index in the sourceArray at which copying begins.</param>
        /// <param name="length">A 32-bit integer that represents the number of elements to copy.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="startingPosition"/> and <paramref name="length"/> are out of range</exception>
        public static byte[] GetBytes(this byte[] target, int startingPosition, int length)
        {
            if (length + startingPosition > target.Length)
                throw new ArgumentOutOfRangeException("length", "Parameter startingPosition and length are out of range");

            byte[] value = new byte[length];

            Array.Copy(target, startingPosition, value, 0, length);
            return value;
        }

        /// <summary>
        /// Hashes a string with MD5
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <returns>An array of bytes that represents the hash</returns>
        public static byte[] GetMD5HashBytes(this string target)
        {
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
                return md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(target));
        }

        /// <summary>
        /// Hashes a string with MD5
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <returns>A string representing the hash of the original stirng</returns>
        public static string GetMD5HashString(this string target)
        {
            return BitConverter.ToString(target.GetMD5HashBytes()).Replace("-", string.Empty);
        }

        /// <summary>
        /// Hashes a string with Sha256
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <returns>An array of bytes that represents the hash</returns>
        public static byte[] GetSha256HashBytes(this string target)
        {
            using (SHA256 sha = SHA256.Create())
                return sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(target));
        }

        /// <summary>
        /// Hashes a string with Sha256
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <returns>An string that represents the hash of the original string</returns>
        public static string GetSha256HashString(this string target)
        {
            return BitConverter.ToString(target.GetSha256HashBytes()).Replace("-", string.Empty);
        }

        /// <summary>
        /// Gets the window handle for a Windows Presentation Foundation (WPF) window
        /// </summary>
        /// <param name="window">A WPF window object.</param>
        /// <returns>The Windows Presentation Foundation (WPF) window handle (HWND).</returns>
        public static IntPtr GetWindowHandle(this Window window)
        {
            return new System.Windows.Interop.WindowInteropHelper(window).Handle;
        }

        /// <summary>
        /// Checks if the type has implemented the defined interface
        /// </summary>
        /// <typeparam name="T">The type of interface to look for</typeparam>
        /// <param name="type">The type that may implements the interface <typeparamref name="T"/></param>
        /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is not an interface</exception>
        /// <returns>True if the <paramref name="type"/> has implemented the interface <typeparamref name="T"/></returns>
        public static bool ImplementsInterface<T>(this Type type)
        {
            var typeOfInterface = typeof(T);

            if (!typeOfInterface.IsInterface)
                throw new ArgumentException("T is not an interface", nameof(T));

            return type.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeOfInterface);
        }

        /// <summary>
        /// Searches for the specified byte array and returns the zero-based index of the first
        /// occurrence within the entire <see cref="Array"/>
        /// </summary>
        /// <param name="data">The <see cref="Array"/> that could contain <paramref name="value"/></param>
        /// <param name="value">The object to locate in the <see cref="Array"/>. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire <see cref="Array"/>, if found; otherwise, –1.</returns>
        public static long IndexOf(this byte[] data, byte[] value)
        {
            if (value.Length > data.Length)
                return -1;

            unsafe
            {
                var dataLength = data.Length;
                var findLength = value.Length;
                var unequal = false;

                fixed (byte* pData = data, pFind = value)
                {
                    byte* dataPointer = pData;
                    byte* findPointer = pFind;

                    do
                    {
                        byte* currentDataPointer = dataPointer;

                        do
                        {
                            if (*currentDataPointer != *findPointer)
                            {
                                unequal = true;
                                break;
                            }

                            findPointer++;
                            currentDataPointer++;
                        }
                        while (--findLength > 0);

                        if (!unequal)
                            return dataPointer - pData;

                        unequal = false;
                        // reset the length
                        findLength = value.Length;
                        // move the pointer back to the next byte
                        dataPointer++;
                        // reset the find pointer
                        findPointer = pFind;
                    }
                    while (--dataLength > findLength);
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the entire <see cref="Array"/>
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Array"/></typeparam>
        /// <param name="target">The <see cref="Array"/> that could contain <paramref name="value"/></param>
        /// <param name="value">The object to locate in the <see cref="Array"/>. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire <see cref="Array"/>, if found; otherwise, –1.</returns>
        public static int IndexOf<T>(this T[] target, T value)
        {
            for (int i = 0; i < target.Length; i++)
                if (target[i].Equals(value))
                    return i;

            return -1;
        }

        /// <summary>
        /// Checks if the value is null. If not, it will invoke <paramref name="action"/>
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The value to check</param>
        /// <param name="action">The action to invoke if <paramref name="value"/> is not null</param>
        public static void IsNotNull<T>(this T value, Action<T> action)
        {
            if (value != null)
                action(value);
        }

        /// <summary>
        /// Gets a value indicating whether the current type is a <see cref="Nullable{T}"/>
        /// </summary>
        /// <param name="target">The type to test</param>
        /// <returns>Returns true if the type is <see cref="Nullable{T}"/></returns>
        public static bool IsNullable(this Type target)
        {
            return target.IsGenericType && Nullable.GetUnderlyingType(target) != null;
        }

        /// <summary>
        /// Replaces the values of data in memory with random values. The GC handle will be freed.
        /// </summary>
        /// <remarks>Will only work on <see cref="GCHandleType.Pinned"/></remarks>
        /// <param name="target"></param>
        /// <param name="targetLength"></param>
        public static void RandomizeValues(this GCHandle target, int targetLength)
        {
            unsafe
            {
                byte* insecureData = (byte*)target.AddrOfPinnedObject();

                for (int i = 0; i < targetLength; i++)
                    insecureData[i] = Randomizer.NextByte();

                target.Free();
            }
        }

        /// <summary>
        /// Reads all characters from the <see cref="SeekOrigin.Begin"/> to the end of the stream
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read</param>
        /// <returns>The stream as a string</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="stream"/> is null</exception>
        /// <exception cref="NotSupportedException">Parameter <paramref name="stream"/> is not seekable</exception>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public static string ReadToEnd(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanSeek)
                throw new NotSupportedException("Unseekable streams are not supported");

            stream.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                stream.Dispose();
                return content;
            }
        }

        /// <summary>
        /// Converts a <see cref="IEnumerable"/> to an array
        /// </summary>
        /// <typeparam name="T">The type of elements the <see cref="IEnumerable"/> contains</typeparam>
        /// <param name="items">The <see cref="IEnumerable"/> to convert</param>
        /// <returns>An array of <typeparamref name="T"/></returns>
        public static T[] ToArray<T>(this IEnumerable items)
        {
            if (items == null)
                return new T[0];

            T[] result = new T[items.Count()];
            int counter = 0;

            foreach (T item in items)
            {
                result[counter] = item;
                counter++;
            }

            return result;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BitmapImage"/> and assigns the <see cref="Stream"/> to its <see cref="BitmapImage.StreamSource"/> property
        /// <para/>
        /// Returns null if <paramref name="stream"/> is null.
        /// </summary>
        /// <param name="stream">The stream that contains an image</param>
        /// <returns>A new instance of <see cref="BitmapImage"/></returns>
        public static BitmapImage ToBitmapImage(this Stream stream)
        {
            if (stream == null)
                return null;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }

        /// <summary>
        /// Converts a <see cref="Stream"/> to <see cref="byte"/> array
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to convert</param>
        /// <returns>An array of bytes</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="stream"/> is null</exception>
        /// <exception cref="NotSupportedException">Parameter <paramref name="stream"/> is not seekable</exception>
        public static async Task<byte[]> ToBytesAsync(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanSeek)
                throw new NotSupportedException("Unseekable streams are not supported");

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.SetLength(stream.Length);
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="target">An array of bytes.</param>
        /// <returns>A 32-bit signed integer formed by four bytes</returns>
        public static int ToInteger(this byte[] target)
        {
            return BitConverter.ToInt32(target, 0);
        }

        /// <summary>
        /// Converts a string to a <see cref="SecureString"/>
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>The <see cref="SecureString"/> equivalent of the string</returns>
        [SecurityCritical]
        public static SecureString ToSecureString(this string value)
        {
            var result = new SecureString();

            for (int i = 0; i < value.Length; i++)
                result.AppendChar(value[i]);

            return result;
        }
    }
}