using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class OutlineController : MonoBehaviour
{
    [SerializeField] 
    private Material outlineMaterial;
    [SerializeField] 
    private Material defaultMaterial;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private bool isSelected = false;

    public bool IsSelected => isSelected;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    private void Start()
    {
        Deselect();
    }

    private void OnMouseDown()
    {
        if (isSelected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    public void Select()
    {
        isSelected = true;
        if (outlineMaterial != null && spriteRenderer != null)
        {
            spriteRenderer.material = outlineMaterial;            
            spriteRenderer.GetPropertyBlock(mpb);

            if (spriteRenderer.sprite != null)
            {
                mpb.SetTexture("_MainTex", spriteRenderer.sprite.texture);
            }
            
            spriteRenderer.SetPropertyBlock(mpb);
        }
    }

    public void Deselect()
    {
        isSelected = false;
        if (defaultMaterial != null && spriteRenderer != null)
        {
            spriteRenderer.material = defaultMaterial;
            
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.Clear();
            spriteRenderer.SetPropertyBlock(mpb);
        }
    }
}