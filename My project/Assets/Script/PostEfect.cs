using UnityEngine;

public class PostEfect : MonoBehaviour
{
    private Material mat;
    public Texture texture;
    public int pixelDensity = 80;

    void Start()
    {
        mat = new Material(Shader.Find("Custom/PixelPallet"));
        mat.SetTexture("_ColorTheme", texture);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Vector2 aspectRatioData;
        if (Screen.height > Screen.width)
            aspectRatioData = new Vector2((float)Screen.width / Screen.height, 1);
        else
            aspectRatioData = new Vector2(1, (float)Screen.height / Screen.width);
        mat.SetVector("_ScreenAspectRatioMultiplier", aspectRatioData);
        mat.SetInt("_PixelDensity", pixelDensity);

        // Read pixels from the source RenderTexture, apply the material, copy the updated results to the destination RenderTexture
        Graphics.Blit(src, dest, mat);
    }
}
