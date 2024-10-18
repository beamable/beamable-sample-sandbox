using UnityEngine;

namespace DefaultNamespace
{
    public class TestReproMonoBehaviour: MonoBehaviour
    {
        async void Start()
        {
            // Create an instance of TestRepro and call the ReproduceBug method
            TestRepro testRepro = new TestRepro();
            await testRepro.ReproduceBug();  // Await the async task to complete
        }
    }
}