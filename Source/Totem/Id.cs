﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;

namespace Totem
{
	/// <summary>
	/// Identifies a persistent object by a string. May be assigned or unassigned.
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public struct Id : IEquatable<Id>, IComparable<Id>
	{
		private readonly string _value;

		private Id(string value)
		{
			_value = value;
		}

		public bool IsUnassigned => string.IsNullOrEmpty(_value);
		public bool IsAssigned => !string.IsNullOrEmpty(_value);

    public override string ToString() => _value ?? "";

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return obj is Id && Equals((Id) obj);
		}

		public bool Equals(Id other)
		{
			return Eq.Values(this, other).Check(x => x.ToString());
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public int CompareTo(Id other)
		{
			return Cmp.Values(this, other).Check(x => x.ToString());
		}

		public static bool operator ==(Id x, Id y) => Eq.Op(x, y);
		public static bool operator !=(Id x, Id y) => Eq.OpNot(x, y);
		public static bool operator >(Id x, Id y) => Cmp.Op(x, y) > 0;
		public static bool operator <(Id x, Id y) => Cmp.Op(x, y) < 0;
		public static bool operator >=(Id x, Id y) => Cmp.Op(x, y) >= 0;
		public static bool operator <=(Id x, Id y) => Cmp.Op(x, y) <= 0;

		//
		// Factory
		//

		public static readonly Id Unassigned = new Id();

		public static Id From(string value)
		{
			return new Id((value ?? "").Trim());
		}

    public static Id From<T>(T value)
    {
      return From(value?.ToString());
    }

    public static Id FromGuid()
		{
			return new Id(Guid.NewGuid().ToString());
		}

		public static Id FromMany(IEnumerable<string> ids)
		{
			return From(ids.ToTextSeparatedBy("/").ToString());
		}

		public static Id FromMany(params string[] ids)
		{
			return FromMany(ids as IEnumerable<string>);
		}

		public static Id FromMany(IEnumerable<Id> ids)
		{
			return From(ids.ToTextSeparatedBy("/").ToString());
		}

		public static Id FromMany(params Id[] ids)
		{
			return FromMany(ids as IEnumerable<Id>);
		}

		public sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value) => From(value);
		}
	}
}