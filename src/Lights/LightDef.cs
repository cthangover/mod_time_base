using System.Text.Json.Serialization;
using Godot;

namespace Cthangover.Mods.Time
{
    /// <summary>
    /// Serializable light definition for JSON scene lighting. Coordinates are
    /// target-relative so scenes adapt to different resolutions. Backgrounds use 0–1; free map artwork can extend beyond that coordinate basis.
    /// ToPixelPos/FromPixelPos convert between normalized space and pixel space
    /// using a reference viewport size. Color is stored as a hex string for
    /// human-readable JSON editing.
    /// </summary>
	public class LightDef
	{
        /// <summary>Optional resource-local identity for keyed patches; legacy scenario lights need no id.</summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Id { get; set; }

        /// <summary>Fields supplied by dependent mods survive an editor save.</summary>
        [JsonExtensionData]
        public System.Collections.Generic.Dictionary<string, System.Text.Json.JsonElement> ExtraFields { get; set; }

        /// <summary>
        /// Horizontal position relative to the target width. Backgrounds use 0–1; free map artwork may extend beyond that basis.
        /// </summary>
		[JsonPropertyName("x")]
		public float X { get; set; }

        /// <summary>
        /// Vertical position relative to the target height. Free map lights can lie on walls beyond the ground boundary.
        /// </summary>
		[JsonPropertyName("y")]
		public float Y { get; set; }

        /// <summary>
        /// Light radius in target units (background pixels or map cells). Determines the falloff distance in the
        /// lighting shader.
        /// </summary>
		[JsonPropertyName("radius")]
		public float Radius { get; set; }

        /// <summary>
        /// Light intensity (0.0–1.0). At <c>1.0</c>, the light fully brightens
        /// pixels within its radius. Lower values create subtler effects.
        /// </summary>
		[JsonPropertyName("influence")]
		public float Influence { get; set; } = 1f;

        /// <summary>
        /// Light color as a hex string (e.g. "#ff8800"). Parsed by
        /// <see cref="ToColor"/> for shader upload.
        /// </summary>
		[JsonPropertyName("color")]
		public string ColorHex { get; set; } = "#ffff00";

        /// <summary>
        /// Converts <see cref="ColorHex"/> to a Godot <c>Color</c> struct.
        /// </summary>
		public Color ToColor()
		{
			return new Color(ColorHex);
		}

        /// <summary>
        /// Converts normalized coordinates (<see cref="X"/>, <see cref="Y"/>) to
        /// pixel coordinates based on <paramref name="viewportSize"/>.
        /// </summary>
        /// <param name="viewportSize">Current viewport dimensions in pixels.</param>
		public Vector2 ToPixelPos(Vector2 viewportSize)
		{
			return new Vector2(X * viewportSize.X, Y * viewportSize.Y);
		}

        /// <summary>
        /// Computes normalized coordinates from pixel coordinates by dividing by
        /// <paramref name="referenceSize"/>. Enables saving light positions
        /// across different resolutions.
        /// </summary>
        /// <param name="pixelPos">Light position in pixel space.</param>
        /// <param name="referenceSize">Viewport dimensions used as reference.</param>
		public void FromPixelPos(Vector2 pixelPos, Vector2 referenceSize)
		{
			X = pixelPos.X / referenceSize.X;
			Y = pixelPos.Y / referenceSize.Y;
		}
	}
}
