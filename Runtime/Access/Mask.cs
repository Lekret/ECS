using System.Collections.Generic;

namespace Lekret.Ecs
{
    public static class Mask
    {
        public static MaskBuilder With<T>()
        {
            return new MaskBuilder().With<T>();
        }

        public static CompoundMask AllOf(MaskBuilder maskBuilder)
        {
            return AllOf(maskBuilder.Indices);
        }

        public static CompoundMask AllOf(IEnumerable<int> indices)
        {
            return CompoundMask.CreateAllOf(indices);
        }

        public static CompoundMask AnyOf(MaskBuilder maskBuilder)
        {
            return AnyOf(maskBuilder.Indices);
        }

        public static CompoundMask AnyOf(IEnumerable<int> indices)
        {
            return CompoundMask.CreateAnyOf(indices);
        }
    }
}