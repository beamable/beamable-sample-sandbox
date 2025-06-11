using System;
using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Common
{
    [Serializable]
    public class CategoryEntry
    {
        public string category;
        public SerializableDictionary<string, int> values;
    }
}