using Cthangover.Core.Scenes;
using Cthangover.Core.Settings;
using Cthangover.Core.UI.Tool;
using Cthangover.Core.Utils;
using Godot;

using Cthangover.Mods.Interface.Tool;
namespace Cthangover.Mods.Time
{
	/// <summary>
	/// Draggable lamp UI element for the player's portable light source.
    /// A four-pixel drag threshold distinguishes a click that stows the lamp from an intentional new drag.
    /// Input handling exists in both _GuiInput (bubbled) and _Input (global)
    /// to catch drags that start on the lamp but continue outside its rect.
    /// When the lamp is hidden, it resets position to offscreen (-1000,-1000)
    /// to disable the light in the shader. GetLightParams() is a static helper
    /// so the light controller can query lamp parameters without coupling to
    /// the behaviour instance. Inherits from TextureRectModIconLoader for
    /// mod-provided lamp icon textures.
    /// </summary>
	public partial class LampBehaviour : TextureRectModIconLoader
	{
		private UiLightController controller;
		private Vector2 dragPosition;
		private bool isPlaced;

        private const float DragThresholdSquared = 16f;
        private Vector2 pressPosition;
        private bool dragging;
        private bool stowOnRelease;
		private bool pressingOnLamp;

        /// <summary>
        /// Queries lamp parameters (color, radius, influence) from
        /// <see cref="GameData.Instance.Runtime.LampData"/>. Static so the
        /// <see cref="UiLightController"/> can query without a direct reference.
        /// </summary>
		public static (Color, float, float) GetLightParams()
		{
			var lampData = GameData.Instance.Runtime.LampData;
			var radius = lampData.Radius;
			var influence = lampData.Influence;
			return (Colors.Yellow, radius, influence);
		}

        /// <summary>
        /// Visibility toggle for the lamp. Setting to <c>false</c> resets the
        /// placed state and sends the light position offscreen to disable
        /// the lamp light in the shader. Uses <c>new</c> to log transitions.
        /// </summary>
		public new bool Visible
		{
			get => base.Visible;
			set
			{
				if (base.Visible != value)
					GameLogger.Log("LIGHT", $"Lamp.Visible: {base.Visible} -> {value}, isPlaced={isPlaced}", LogLevel.Debug);
				base.Visible = value;
				if (!value)
				{
					pressingOnLamp = false;
                    dragging = false;
                    stowOnRelease = false;
					if (controller != null)
					{
						isPlaced = false;
						controller?.UpdateLightPos(new Vector2(-1000f, -1000f));
					}
				}
			}
		}

		/// <summary>Finds the shared <see cref="UiLightController"/> this lamp pushes positions to.</summary>
		public override void _Ready()
		{
			base._Ready();
			MouseFilter = MouseFilterEnum.Stop;
            GuiInput += HandleLampInput;
			controller = SceneContextNode.FindNode<UiLightController>("Lights");
			GameLogger.Log("LIGHT", $"Lamp._Ready: controller={(controller != null ? "OK" : "NULL")}", LogLevel.Debug);
		}

		/// <summary>Click to place the light at the cursor, drag to move it, and click a placed lamp to pick it back up.</summary>
		private void HandleLampInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton mouseButton)
			{
				if (mouseButton.ButtonIndex == MouseButton.Left)
				{
					if (mouseButton.Pressed)
					{
						pressingOnLamp = true;
                        dragging = false;
                        stowOnRelease = isPlaced;
                        pressPosition = EventPosition(mouseButton);
						dragPosition = pressPosition;

						if (isPlaced)
						{
							dragPosition = new Vector2(-1000f, -1000f);
							controller?.UpdateLightPos(dragPosition);
							isPlaced = false;
						}
					}
					else
					{
                        ReleasePress(EventPosition(mouseButton));
					}
				}
				AcceptEvent();
			}
			else if (@event is InputEventMouseMotion mouseMotion)
			{
				if (pressingOnLamp && Input.IsMouseButtonPressed(MouseButton.Left))
				{
                    DragTo(EventPosition(mouseMotion));
					AcceptEvent();
				}
			}
		}

		/// <summary>Keeps dragging the placed light while the pointer moves outside the lamp's own control rect.</summary>
		public override void _Input(InputEvent @event)
		{
			if (!Visible)
				return;

			if (@event is InputEventMouseMotion mouseMotion
				&& Input.IsMouseButtonPressed(MouseButton.Left)
				&& pressingOnLamp)
			{
                DragTo(EventPosition(mouseMotion));
			}

			if (@event is InputEventMouseButton mouseButton
				&& mouseButton.ButtonIndex == MouseButton.Left
				&& !mouseButton.Pressed
				&& pressingOnLamp)
			{
                ReleasePress(EventPosition(mouseButton));
			}
		}
        // A click may contain small motion events (including the OS warp used by input automation).
        // Once a drag crosses the threshold it keeps following the pointer, even back near the press origin.
        // Use the event's point, not cursor polling which may already reflect a later queued OS motion.
        private Vector2 EventPosition(InputEventMouse mouse)
            => GetCanvasTransform().AffineInverse() * mouse.GlobalPosition;

        private void DragTo(Vector2 position)
        {
            if (!dragging && position.DistanceSquaredTo(pressPosition) < DragThresholdSquared) return;
            dragging = true;
            dragPosition = position;
            controller?.UpdateLightPos(dragPosition);
            isPlaced = false;
        }

        private void ReleasePress(Vector2 position)
        {
            if (!pressingOnLamp) return;
            if (dragging) dragPosition = position;
            isPlaced = !stowOnRelease || dragging;
            controller?.UpdateLightPos(isPlaced ? dragPosition : new Vector2(-1000f, -1000f));
            pressingOnLamp = false;
            dragging = false;
            stowOnRelease = false;
        }

	}
}
