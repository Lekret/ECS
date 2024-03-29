﻿using System;
using System.Collections.Generic;
using ECS.Runtime.Core;

namespace ECS.Runtime.Access
{
    public sealed class CompoundMask
    {
        [ThreadStatic] private static List<int> MergeBuffer;
        [ThreadStatic] private static HashSet<int> DistinctBuffer;

        private int[] _indices;
        private int[] _allOfIndices = Array.Empty<int>();
        private int[] _anyOfIndices = Array.Empty<int>();
        private int[] _noneOfIndices = Array.Empty<int>();

        private CompoundMask()
        {
        }

        public int[] Indices => _indices ?? (_indices = MergeIndices());
        public int[] AllOfIndices => _allOfIndices;
        public int[] AnyOfIndices => _anyOfIndices;
        public int[] NoneOfIndices => _noneOfIndices;

        public CompoundMask AnyOf(MaskBuilder maskBuilder)
        {
            _anyOfIndices = DistinctIndices(maskBuilder.Indices);
            _indices = null;
            return this;
        }

        public CompoundMask NoneOf(MaskBuilder maskBuilder)
        {
            _noneOfIndices = DistinctIndices(maskBuilder.Indices);
            _indices = null;
            return this;
        }

        public bool Matches(Entity entity) =>
            (_allOfIndices.Length == 0 || entity.HasAll(_allOfIndices)) &&
            (_anyOfIndices.Length == 0 || entity.HasAny(_anyOfIndices)) &&
            (_noneOfIndices.Length == 0 || !entity.HasAny(_noneOfIndices));

        public override bool Equals(object obj)
        {
            var mask = obj as CompoundMask;
            if (mask == null)
                return false;

            if (!IndicesEqual(mask._allOfIndices, _allOfIndices))
                return false;
            if (!IndicesEqual(mask._anyOfIndices, _anyOfIndices))
                return false;
            if (!IndicesEqual(mask._noneOfIndices, _noneOfIndices))
                return false;
            return true;
        }

        internal static CompoundMask CreateAllOf(IEnumerable<int> indices)
        {
            return new CompoundMask {_allOfIndices = DistinctIndices(indices)};
        }

        internal static CompoundMask CreateAnyOf(IEnumerable<int> indices)
        {
            return new CompoundMask {_anyOfIndices = DistinctIndices(indices)};
        }

        private static bool IndicesEqual(int[] left, int[] right)
        {
            if (left.Length != right.Length)
                return false;

            for (var i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                    return false;
            }

            return true;
        }

        private int[] MergeIndices()
        {
            var buffer = GetMergeBuffer();
            buffer.AddRange(_allOfIndices);
            buffer.AddRange(_anyOfIndices);
            buffer.AddRange(_noneOfIndices);
            var indices = DistinctIndices(buffer);
            buffer.Clear();
            return indices;
        }

        private static int[] DistinctIndices(IEnumerable<int> indices)
        {
            var buffer = GetDistinctBuffer();
            foreach (var index in indices)
            {
                buffer.Add(index);
            }

            var uniqueIndices = new int[buffer.Count];
            buffer.CopyTo(uniqueIndices);
            buffer.Clear();
            return uniqueIndices;
        }

        private static List<int> GetMergeBuffer()
        {
            if (MergeBuffer == null)
            {
                MergeBuffer = new List<int>();
            }

            return MergeBuffer;
        }

        private static HashSet<int> GetDistinctBuffer()
        {
            if (DistinctBuffer == null)
            {
                DistinctBuffer = new HashSet<int>();
            }

            return DistinctBuffer;
        }
    }
}