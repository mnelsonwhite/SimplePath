﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePath
{
    public struct SPath
        : IEquatable<SPath>
        , IComparable<SPath>
        , IFormattable
        , IEnumerable<string>
    {
        private static readonly string _systemDelimiter = System.IO.Path.DirectorySeparatorChar.ToString();
        private readonly string[] _path;

        public string this[int key]
        {
            get => _path[key];
            set => _path[key] = value;
        }

        public int Length => _path.Length;
        public string DefaultDelimiter { get; }

        public static SPath Parse(string path, string? delimiter = null)
        {
            return new SPath(Split(path, delimiter), delimiter);
        }

        public SPath(IEnumerable<string> path, string? delimiter = null)
        {
            DefaultDelimiter = delimiter ?? _systemDelimiter;
            _path = path.ToArray();
        }

        public SPath(SPath path) : this(path._path) { }

        public SPath(params string[] path) : this((IEnumerable<string>)path) { }

        public SPath Concat(params string[] segments)
        {
            return new SPath(_path.Concat(segments), DefaultDelimiter);
        }

        public SPath Concat(SPath path)
        {
            return new SPath(_path.Concat(path._path), DefaultDelimiter);
        }

        public bool IsChildOf(SPath path)
        {
            if (path._path.Length >= _path.Length)
            {
                return false;
            }

            var self = this;
            return path._path
                .Select((segment, index) => new { segment, index })
                .All(item => self._path[item.index] == item.segment);
        }

        public SPath ToRelative(SPath path)
        {
            var self = this;
            var length = path._path
                .Select((segment, index) => new { segment, index })
                .TakeWhile(item => self._path[item.index] == item.segment)
                .Count();

            return new SPath(_path.Skip(length), DefaultDelimiter);
        }

        public SPath WithDefaultDelimiter(string delimiter)
        {
            return new SPath(_path, delimiter);
        }

        public SPath CommonParent(SPath path)
        {
            var self = this;
            var segments = path._path
                .Select((segment, index) => new { segment, index })
                .Where(item => self._path[item.index] == item.segment)
                .Select(item => item.segment);

            return new SPath(segments, DefaultDelimiter);
        }

        public int CompareTo(SPath other)
        {
            if (IsChildOf(other)) return -1;
            if (Equals(other)) return 0;
            return 1;
        }

        public override string ToString()
        {
            return ToString(DefaultDelimiter);
        }

        public string ToString(string delimiter)
        {
            if (_path == null || _path.Length == 0) return string.Empty;
            return string.Join(delimiter, _path);
        }

        public static SPath operator +(SPath lhs, string rhs)
        {
            return lhs.Concat(rhs);
        }

        public static SPath operator +(SPath lhs, string[] rhs)
        {
            return lhs.Concat(rhs);
        }

        public static SPath operator +(SPath lhs, SPath rhs)
        {
            return lhs.Concat(rhs);
        }

        public static bool operator ==(SPath lhs, SPath rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SPath lhs, SPath rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj is SPath path)
            {
                return Equals(path);
            }
            return false;
        }

        public bool Equals(SPath other)
        {
            return _path.SequenceEqual(other._path);
        }

        public override int GetHashCode()
        {
            return _path?.GetHashCode() ?? 0;
        }

        private static string[] Split(string target, string? delimiter)
        {
            return target.Split(
                new[] { delimiter ?? _systemDelimiter },
                StringSplitOptions.None);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(format);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _path.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _path.GetEnumerator();
        }
    }
}
