using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class OutlineController : MonoBehaviour
{
    private static OutlineController currentlySelected;

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
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.Deselect();
        }

        isSelected = true;
        currentlySelected = this;

        if (outlineMaterial != null && spriteRenderer != null)
        {
            spriteRenderer.sharedMaterial = outlineMaterial;            

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

        if (currentlySelected == this)
        {
            currentlySelected = null;
        }

        if (defaultMaterial != null && spriteRenderer != null)
        {
            spriteRenderer.sharedMaterial = defaultMaterial;            
            spriteRenderer.SetPropertyBlock(null);
        }
    }
}