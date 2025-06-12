using System;
using System.Collections.Generic;
using Beamable.Common.Content;
using UnityEngine;
namespace Beamable.Common
{
    [Serializable]
    public class EntryValue
    {
        public string key;
        public int value;
    }

    [Serializable]
    public class CategoryEntry
    {
        public string category;
        public List<EntryValue> values;
    }

}