using UnityEngine;
using UnityEngine.Purchasing;

public class IAPInitializer : MonoBehaviour, IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;

    void Start()
    {
        InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));

        // Enable Fake Store for testing
        builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;

        // Add your products here
        builder.AddProduct("consumable_id", ProductType.Consumable);
        builder.AddProduct("nonconsumable_id", ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        return controller != null && extensions != null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Initialization failed: " + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("Purchase failed: " + product.definition.id + ", reason: " + failureReason);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // Handle the purchase
        Debug.Log("Purchase successful: " + args.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }
}
