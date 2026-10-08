using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Products
{
    public ProductInfo[] products;
}


[Serializable]
public class ProductInfo
{
    public string name;
    public string optValue;
    public string mainCategory;
    public string subCategory;
    public string modelCategory1;
    public string modelCategory2;
    public string modelCategory3;
    public string modelCategory4;
    public string application;
    public string range;
    public string headline;
    public string releaseDate;
    public string brand;
    public string[] features;

    public ImagesData images;
}

[Serializable]
public class ImagesData
{
    public string catalog;
    public GaleryImageData[] galleryImages;
}

[Serializable]
public class GaleryImageData
{
    public string galleryType;
    public string galleryB;
    public string galleryDThumb;
    public string galleryXL;
}

public static class ProductInfoManager
{
    private const string PRODUCT_URL = "https://www.behringer.com/.rest/musictribe/v1/downloadcenter/all-products-with-images?modelCode=";


    /// <summary>
    /// Get's product info from the online database.
    /// </summary>
    public static void GetProductInfo(string sProductCode, Action<ProductInfo> callback)
    {
#if UNITY_EDITOR
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(GetProductInfoCoroutine(sProductCode, callback));
#endif
    }


    public static void GetProductImage(string sUrl, Action<Sprite> callback)
    {
#if UNITY_EDITOR
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(GetImageCoroutine(sUrl, callback));
#endif
    }


    private static IEnumerator GetProductInfoCoroutine(string sProductCode, Action<ProductInfo> callback)
    {
        // If it's the old style of product code, change it to the one needed for the URL.
        if (true == sProductCode.StartsWith("000-"))
        {
            sProductCode = "P0" + sProductCode.Substring(4); // Length of "000-"
        }

        string sUrl = PRODUCT_URL + sProductCode;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(sUrl))
        {
            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);

                    Products product = new Products();
                    product = JsonUtility.FromJson<Products>(webRequest.downloadHandler.text);

                    if (product.products.Length > 0)
                    {
                        if (product.products.Length > 1)
                        {
                            Debug.LogError("Found multiple products!!! :S");
                        }

                        callback.Invoke(product.products[0]);
                    }
                    else
                    {
                        Debug.LogError("Could not get product data for " + sProductCode);
                        callback.Invoke(null);
                    }

                    break;
            }
        }
    }


    public static IEnumerator GetImageCoroutine(string sUrl, Action<Sprite> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(sUrl))
        {
            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);

                    Texture2D newTexture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                    Rect spriteRect = new Rect(0f, 0f, newTexture.width, newTexture.height);
                    Sprite downloadedSprite = Sprite.Create(newTexture, spriteRect, Vector2.zero);

                    callback.Invoke(downloadedSprite);

                    break;
            }
        }
    }


    // Based on https://www.owasp.org/index.php/Certificate_and_Public_Key_Pinning#.Net
    class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
    {
        // Encoded RSAPublicKey
        private static string PUB_KEY = "04 4e 86 00 c2 f0 b2 17 0a a8 e3 fd 8e 27 7b 60 2e 09 99 46 d3 d9 2c 2d fc e6 c5 88 66 86 1d ab 08 53 74 82 db 16 db 2f 92 53 27 f4 02 50 19 de 1f 74 ea eb e8 ab 20 6b a7 3c 66 e4 f0 ac 2d ff 5b";
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // TODO: Actually make this work properly!
            return true;

            X509Certificate2 certificate = new X509Certificate2(certificateData);
            string pk = certificate.GetPublicKeyString();
            if (pk.ToLower().Equals(PUB_KEY.ToLower()))
            {
                return true;
            }

            return false;
        }
    }
}