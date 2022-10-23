using System;
using System.Collections.Generic;
using SimpleEcs.Runtime;

namespace Lekret.Ecs
{
    public interface IMask
    {
        int[] Indices { get; }
        bool Matches(Entity entity);
    }

    public interface INoneOfMask : IMask
    {
        int[] AllOfIndices { get; }
        int[] AnyOfIndices { get; }
        int[] NoneOfIndices { get; }
    }
    
    public interface IAnyOfMask : INoneOfMask
    {
        INoneOfMask NoneOf(MaskBuilder maskBuilder);
    }
    
    public interface IAllOfMask : IAnyOfMask
    {
        IAnyOfMask AnyOf(MaskBuilder maskBuilder);
    }

    public class Mask : IAllOfMask
    {
        [ThreadStatic] private static List<int> MergeBuffer;
        [ThreadStatic] private static HashSet<int> DistinctBuffer;
        
        private int[] _indices;
        private int[] _allOfIndices = Array.Empty<int>();
        private int[] _anyOfIndices = Array.Empty<int>();
        private int[] _noneOfIndices = Array.Empty<int>();

        private Mask() { }

        public int[] Indices => _indices ?? (_indices = MergeIndices(_allOfIndices, _anyOfIndices, _noneOfIndices));
        public int[] AllOfIndices => _allOfIndices;
        public int[] AnyOfIndices => _anyOfIndices;
        public int[] NoneOfIndices => _noneOfIndices;

        public static MaskBuilder With<T>()
        {
            return new MaskBuilder().With<T>();
        }
        
        public static IAllOfMask AllOf(MaskBuilder maskBuilder)
        {
            return AllOf(maskBuilder.Indices);
        }
        
        public static IAllOfMask AllOf(IEnumerable<int> indices)
        {
            return new Mask {_allOfIndices = DistinctIndices(indices)};
        }
        
        public static INoneOfMask AnyOf(MaskBuilder maskBuilder)
        {
            return AnyOf(maskBuilder.Indices);
        }
        
        public static IAnyOfMask AnyOf(IEnumerable<int> indices)
        {
            return new Mask {_anyOfIndices = DistinctIndices(indices)};
        }

        IAnyOfMask IAllOfMask.AnyOf(MaskBuilder maskBuilder)
        {
            _anyOfIndices = DistinctIndices(maskBuilder.Indices);
            _indices = null;
            return this;
        }

        public INoneOfMask NoneOf(MaskBuilder maskBuilder)
        {
            _noneOfIndices = DistinctIndices(maskBuilder.Indices);
            _indices = null;
            return this;
        }

        public bool Matches(Entity entity) =>
            entity.HasAll(_allOfIndices)
            && entity.HasAny(_anyOfIndices)
            && !entity.HasAny(_noneOfIndices);

        public override bool Equals(object obj)
        {
            var matcher = obj as Mask;
            if (matcher == null)
                return false;

            if (!IndicesEqual(matcher._allOfIndices, _allOfIndices))
                return false;
            if (!IndicesEqual(matcher._anyOfIndices, _anyOfIndices))
                return false;
            if (!IndicesEqual(matcher._noneOfIndices, _noneOfIndices))
                return false;
            return true;
        }

        private static bool IndicesEqual(int[] left, int[] right)
        {
            if (left.Length != right.Length)
                return false;

            for (var i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static int[] MergeIndices(int[] allOfIndices, int[] anyOfIndices, int[] noneOfIndices)
        {
            var buffer = GetMergeBuffer();
            if (allOfIndices != null)
                buffer.AddRange(allOfIndices);
            if (anyOfIndices != null)
                buffer.AddRange(anyOfIndices);
            if (noneOfIndices != null) 
                buffer.AddRange(noneOfIndices);
            var mergedIndices = DistinctIndices(buffer);
            buffer.Clear();
            return mergedIndices;
        }

        private static int[] MergeIndices(IMask[] matchers)
        {
            var indices = new int[matchers.Length];
            for (var i = 0; i < matchers.Length; i++)
            {
                var matcher = matchers[i];
                if (matcher.Indices.Length != 1)
                    throw new Exception("matcher.Indices.Length should be 1");

                indices[i] = matcher.Indices[0];
            }

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
            Array.Sort(uniqueIndices);
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