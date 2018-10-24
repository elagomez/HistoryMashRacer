using UnityEngine;

namespace ELine.CustomAttributes
{
	/// <summary>
	/// Causes a boolean field to be rendered as a toggleable button.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class ButtonAttribute : PropertyAttribute
	{
		#region members
		public readonly string MethodName;
		#endregion members

		#region constructors
		public ButtonAttribute( string MethodName )
		{
			this.MethodName = MethodName;
		}
		#endregion constructors
	}
}