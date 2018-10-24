using UnityEngine;
using System.Collections;

/// <summary>
/// Twists a SplineMesh around the spline's local tangents.
/// </summary>
[AddComponentMenu( "SuperSplines/Other/Spline Mesh Modifiers/Curve Twist Modifier" )]
public class SplineCurveTwistModifier : SplineMeshModifier
{
	//The AnimationCurve representing the twist of the mesh
	public AnimationCurve twistCurve = AnimationCurve.EaseInOut(0,1,1,1);

	private Quaternion rotationQuaternion;

	public override Vector3 ModifyVertex( SplineMesh splineMesh, Vector3 vertex, float splineParam )
	{
		rotationQuaternion = Quaternion.AngleAxis( twistCurve.Evaluate( splineParam ) * 360f, Vector3.forward );
		return rotationQuaternion * vertex;
	}

	public override Vector2 ModifyUV( SplineMesh splineMesh, Vector2 uvCoord, float splineParam )
	{
		return uvCoord;
	}

	public override Vector3 ModifyNormal( SplineMesh splineMesh, Vector3 normal, float splineParam )
	{
		return rotationQuaternion * normal;
	}

	public override Vector4 ModifyTangent( SplineMesh splineMesh, Vector4 tangent, float splineParam )
	{
		return rotationQuaternion * tangent;
	}
}
