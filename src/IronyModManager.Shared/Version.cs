// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-21-2022
//
// Last Modified By : Mario
// Last Modified On : 07-21-2022
// ***********************************************************************
// <copyright file="Version.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IronyModManager.Shared
{
#nullable enable

    /// <summary>
    /// Class Version. This class cannot be inherited.
    /// Implements the <see cref="ICloneable" />
    /// Implements the <see cref="IComparable" />
    /// Implements the <see cref="System.IComparable{IronyModManager.Shared.Version}" />
    /// Implements the <see cref="System.IEquatable{IronyModManager.Shared.Version}" />
    /// Implements the <see cref="ISpanFormattable" />
    /// </summary>
    /// <seealso cref="ICloneable" />
    /// <seealso cref="IComparable" />
    /// <seealso cref="System.IComparable{IronyModManager.Shared.Version}" />
    /// <seealso cref="System.IEquatable{IronyModManager.Shared.Version}" />
    /// <seealso cref="ISpanFormattable" />
    public sealed class Version : ICloneable, IComparable, IComparable<Version?>, IEquatable<Version?>, ISpanFormattable
    {
        #region Fields

        /// <summary>
        /// The proxy
        /// </summary>
        private readonly System.Version proxy;

        /// <summary>
        /// The minor
        /// </summary>
        private int? minor;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Version" /> class.
        /// </summary>
        public Version()
        {
            proxy = new System.Version();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version" /> class.
        /// </summary>
        /// <param name="major">The major.</param>
        public Version(int major)
        {
            minor = -1;
            proxy = new System.Version(major, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version" /> class.
        /// </summary>
        /// <param name="version">The version.</param>
        public Version(string version)
        {
            if (version.IndexOf('.') < 0)
            {
                proxy = new System.Version($"{version}.0");
                minor = -1;
            }
            else
            {
                proxy = new System.Version(version);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version" /> class.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        public Version(int major, int minor)
        {
            proxy = new System.Version(major, minor);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version" /> class.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <param name="build">The build.</param>
        public Version(int major, int minor, int build)
        {
            proxy = new System.Version(major, minor, build);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version" /> class.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <param name="build">The build.</param>
        /// <param name="revision">The revision.</param>
        public Version(int major, int minor, int build, int revision)
        {
            proxy = new System.Version(major, minor, build, revision);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version" /> class.
        /// </summary>
        /// <param name="version">The version.</param>
        private Version(System.Version version) => proxy = version;

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the build.
        /// </summary>
        /// <value>The build.</value>
        public int Build => proxy.Build;

        /// <summary>
        /// Gets the major.
        /// </summary>
        /// <value>The major.</value>
        public int Major => proxy.Major;

        /// <summary>
        /// Gets the major revision.
        /// </summary>
        /// <value>The major revision.</value>
        public short MajorRevision => proxy.MajorRevision;

        /// <summary>
        /// Gets the minor.
        /// </summary>
        /// <value>The minor.</value>
        public int Minor => minor.HasValue ? minor.GetValueOrDefault() : proxy.Minor;

        /// <summary>
        /// Gets the minor revision.
        /// </summary>
        /// <value>The minor revision.</value>
        public short MinorRevision => proxy.MinorRevision;

        /// <summary>
        /// Gets the revision.
        /// </summary>
        /// <value>The revision.</value>
        public int Revision => proxy.Revision;

        /// <summary>
        /// Gets the default format field count.
        /// </summary>
        /// <value>The default format field count.</value>
        private int DefaultFormatFieldCount => Minor != -1 ? Build == -1 ? 2 : Revision == -1 ? 3 : 4 : 1;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Version? v1, Version? v2) => !(v1 == v2);

        /// <summary>
        /// Implements the &lt; operator.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Version? v1, Version? v2)
        {
            if (v1 is null)
            {
                return v2 is not null;
            }
            return v1.CompareTo(v2) < 0;
        }

        /// <summary>
        /// Implements the &lt;= operator.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(Version? v1, Version? v2)
        {
            if (v1 is null)
            {
                return true;
            }
            return v1.CompareTo(v2) <= 0;
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Version? v1, Version? v2)
        {
            if (v2 is null)
            {
                return v1 is null;
            }
            return ReferenceEquals(v2, v1) || v2.Equals(v1);
        }

        /// <summary>
        /// Implements the &gt; operator.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Version? v1, Version? v2) => v2 < v1;

        /// <summary>
        /// Implements the &gt;= operator.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(Version? v1, Version? v2) => v2 <= v1;

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Version.</returns>
        public static Version Parse(string input)
        {
            if (input.IndexOf('.') < 0)
            {
                var v = System.Version.Parse($"{input}.0");
                return InitNulllableMinorVersion(v);
            }
            var version = System.Version.Parse(input);
            return new Version(version);
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out Version? result)
        {
            if ((input ?? string.Empty).IndexOf('.') < 0)
            {
                if (System.Version.TryParse($"{input}.0", out var v))
                {
                    result = InitNulllableMinorVersion(v);
                    return true;
                }
            }
            else if (System.Version.TryParse(input, out var v))
            {
                result = new Version(v);
                return true;
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            if (minor.HasValue)
            {
                return InitNulllableMinorVersion(proxy);
            }
            return new Version(proxy);
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentException">Must be version type.</exception>
        public int CompareTo(object? version)
        {
            if (version == null)
            {
                return 1;
            }
            else if (version is Version v)
            {
                return CompareTo(v);
            }
            throw new ArgumentException("Must be version type.");
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public int CompareTo(Version? value)
        {
            if (value != null)
            {
                if (minor.HasValue && value.minor.HasValue)
                {
                    return 0;
                }
                else if (minor.HasValue)
                {
                    return -1;
                }
                else if (value.minor.HasValue)
                {
                    return 1;
                }
            }
            return proxy.CompareTo(value?.proxy);
        }

        /// <summary>
        /// Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Equals([NotNullWhen(true)] Version? obj)
        {
            if (obj != null)
            {
                if (minor.HasValue && obj.minor.HasValue)
                {
                    return true;
                }
                else if (minor.HasValue)
                {
                    return false;
                }
                else if (obj.minor.HasValue)
                {
                    return false;
                }
            }
            return proxy.Equals(obj?.proxy);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return Equals(obj as Version);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            int accumulator = 0;
            accumulator |= (Major & 0x0000000F) << 28;
            accumulator |= (Minor & 0x000000FF) << 20;
            accumulator |= (Build & 0x000000FF) << 12;
            accumulator |= (Revision & 0x00000FFF);
            return accumulator;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="fieldCount">The field count.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString(int fieldCount) => proxy.ToString(fieldCount);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => proxy.ToString(DefaultFormatFieldCount);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.
        /// -or-
        /// A null reference (<see langword="Nothing" /> in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.
        /// -or-
        /// A null reference (<see langword="Nothing" /> in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString();

        /// <summary>
        /// Tries the format.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="fieldCount">The field count.</param>
        /// <param name="charsWritten">The chars written.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryFormat(Span<char> destination, int fieldCount, out int charsWritten) => proxy.TryFormat(destination, fieldCount, out charsWritten);

        /// <summary>
        /// Tries the format.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="charsWritten">The chars written.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryFormat(Span<char> destination, out int charsWritten) => proxy.TryFormat(destination, out charsWritten);

        /// <summary>
        /// Tries to format the value of the current instance into the provided span of characters.
        /// </summary>
        /// <param name="destination">When this method returns, this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, the number of characters that were written in <paramref name="destination" />.</param>
        /// <param name="format">A span containing the characters that represent a standard or custom format string that defines the acceptable format for <paramref name="destination" />.</param>
        /// <param name="provider">An optional object that supplies culture-specific formatting information for <paramref name="destination" />.</param>
        /// <returns><see langword="true" /> if the formatting was successful; otherwise, <see langword="false" />.</returns>
        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten);

        /// <summary>
        /// Initializes the nulllable minor version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>Version.</returns>
        private static Version InitNulllableMinorVersion(System.Version version)
        {
            var result = new Version(version)
            {
                minor = -1
            };
            return result;
        }

        #endregion Methods
    }

#nullable disable
}
