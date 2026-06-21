/* This Class defines the Item spawned in the World. With Collision and Pickup */
using Godot;

public partial class Item : CharacterBody2D
{
    [Export]
    public ItemDataResource ItemData;

    [Export]
    private CollectableComponent CollectableComponent;

    [Export]
    private Sprite2D Sprite2D;

    bool isMoving = true;

    public override void _Ready()
    {
        base._Ready();

        if (CollectableComponent == null)
        {
            CollectableComponent = FindChild("CollectableComponent", true) as CollectableComponent;
        }
        if (Sprite2D == null)
        {
            Sprite2D = FindChild("Sprite2D", true) as Sprite2D;
        }

        UpdateTexture();

        if (CollectableComponent == null)
        {
            Logger.Error($"Item: {Name} CollectableComponent == null");
        }
        if (Sprite2D == null)
        {
            Logger.Error($"Item: {Name} Sprite2D == null");
        }

        if (Engine.IsEditorHint())
        {
            return;
        }

        if (CollectableComponent != null)
        {
            CollectableComponent.OnItemPickedUp += ItemPickedup;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var collision_info = MoveAndCollide(new Vector2((float)(Velocity.X * delta), (float)(Velocity.Y * delta)));
        if (collision_info != null)
        {
            Velocity = Velocity.Bounce(collision_info.GetNormal());
        }
        Velocity -= new Vector2((float)(Velocity.X * delta), (float)(Velocity.Y * delta)) * 4;
        isMoving = false;
    }

    private void ItemPickedup()
    {	
        if(!isMoving) {
            if (CollectableComponent != null)
            {
                CollectableComponent.BodyEntered -= CollectableComponent.OnBodyEntered;
                CollectableComponent.OnItemPickedUp -= ItemPickedup;
            }
            Visible = false;
            //await _AduioStreamPlayer.finished() TODO: When adding sounds this has to be in here
            QueueFree();
        }
    }

    public void UpdateTexture()
    {
        if (ItemData != null && Sprite2D != null)
        {
            Sprite2D.Texture = ItemData.Texture;
        }

        if (ItemData == null)
        {
            Logger.Error($"Item: {Name} ItemData == null in UpdateTexture()");
        }
        if (Sprite2D == null)
        {
            Logger.Error($"Item: {Name} Sprite2D == null in UpdateTexture()");
        }
    }
}
