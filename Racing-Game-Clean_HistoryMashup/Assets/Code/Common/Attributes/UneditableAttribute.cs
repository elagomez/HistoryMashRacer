using UnityEngine;

namespace ELine.CustomAttributes
{
	/// <summary>
	/// <para>Causes the annotated field to be uneditable in the inspector.</para>
	/// </summary>
	public class UneditableAttribute : PropertyAttribute
	{
		#region inner types
		/// <summary>
		/// Describes when an UnediableAttribute causes the annotated field to be uneditable.
		/// </summary>
		public enum Effective
		{
			/// <summary>
			/// The property is always uneditable in the inspector.
			/// </summary>
			Always,

			/// <summary>
			/// The property is editable only if the game is not currently playing.
			/// </summary>
			OnlyWhilePlaying,

			/// <summary>
			/// The property is editable only while the game is playing.
			/// </summary>
			OnlyWhileEditing,
		}
		#endregion inner types

		#region members
		/// <summary>
		/// Controls when this UneditableAttribute will actually cause the annotated field to be uneditable in the inspector.
		/// </summary>
		public readonly Effective EffectiveWhen;
		#endregion members

		#region constructors
		/// <summary>
		/// Causes the annotated field to never be editable in the inspector.
		/// </summary>
		public UneditableAttribute() : this( Effective.Always ) { }

		/// <summary>
		/// Causes the annotated field to be uneditable in the inspector.
		/// </summary>
		/// <param name="effectiveWhen">Describes when the annotated field should be uneditable.</param>
		public UneditableAttribute( Effective effectiveWhen )
		{
			this.EffectiveWhen = effectiveWhen;
		}
		#endregion constructors
	}
}