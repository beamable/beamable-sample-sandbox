using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SpriteLoader : MonoBehaviour
{
    [SerializeField]
    private AssetReference spriteReference; // Reference to the sprite

    private AsyncOperationHandle<Sprite> _iconHandle;

    private async void Start()
    {
        // Start multiple async operations to load the sprite concurrently
        Task<Sprite> loadTask1 = LoadSprite();
        Task<Sprite> loadTask2 = LoadSprite();

        try
        {
            // Wait for all tasks to complete
            Sprite[] sprites = await Task.WhenAll(loadTask1, loadTask2);
            Debug.Log("All sprite loading tasks completed.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during concurrent sprite loading: {e.Message}");
        }
    }

    private async Task<Sprite> LoadSprite()
    {
        Sprite sprite = null;
        try
        {
            if (!_iconHandle.IsValid() || !_iconHandle.IsDone)
            {
                _iconHandle = spriteReference.LoadAssetAsync<Sprite>();
                await _iconHandle.Task;
                sprite = _iconHandle.Result;
                Debug.Log("Sprite loaded successfully.");
            }
            else
            {
                Debug.LogWarning("Attempted to load a sprite that's already been loaded.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading sprite: {e.Message}");
        }

        return sprite;
    }

    private void OnDestroy()
    {
        // Commented out to simulate handle mismanagement and trigger potential errors.
        Addressables.Release(_iconHandle);
    }

    public void NavToOtherScene()
    {
        SceneManager.LoadScene("OtherScene");
    }
}