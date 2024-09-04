using System;
using System.Collections.Generic;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public object Test()
        {
            // Example 1: Return null (which might not be expected)
            return null;

            // Example 2: Return a non-serializable object (uncomment to test)
            // return new GameObject("NonSerializableObject");
        }
    }
}