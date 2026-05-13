using UnityEngine;

public abstract class BaseTurret : MonoBehaviour
{
    public Transform player;
    public float range = 10f;
    public bool isActive = true;
    public LineRenderer lineRenderer;

    protected virtual void Start()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        DrawRange();
    }

    protected abstract void DrawRange();
    protected abstract void Attack();
    protected bool PlayerRange()
    {
        if (!isActive || player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    public void Deactivate()
    {
        isActive = false;
        if (lineRenderer != null) lineRenderer.enabled = false;
    }
}