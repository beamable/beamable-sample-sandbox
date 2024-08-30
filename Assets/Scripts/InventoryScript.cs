    using Beamable;
    using Beamable.Common.Api.Inventory;
    using UnityEngine;
    using UnityEngine.UI;

    public class InventoryScript: MonoBehaviour
    {
        public Button updateButton;

        private void Start()
        {
            updateButton.onClick.AddListener(OnUpdateButtonClick);
        }

        private async void OnUpdateButtonClick()
        {
            var ctx = await BeamContext.Default.Instance;
            var updateBuilder = new InventoryUpdateBuilder();

            // Simulate changing currency
            updateBuilder.CurrencyChange("currency.coins", 10);

            // Trigger the inventory update
            await ctx.Inventory.Update(updateBuilder);
        }
    }