using System;
using Engine.MathEx;
using Engine.MapSystem;
using Engine.PhysicsSystem;

namespace Orvid.Utils
{
	public static class RayExtensions
	{
		public static bool HitsMapObject(this Ray ray, MapObject obj, float maxDistance = 2048)
		{
			ray.Direction = ray.Direction.GetNormalize() * maxDistance;
			RayCastResult[] piercingResult = PhysicsWorld.Instance.RayCastPiercing(ray, (int)ContactGroup.CastAll);
			foreach (var res in piercingResult)
			{
				if (res.Distance < maxDistance && res.Shape.Body._InternalUserData != null && res.Shape.Body._InternalUserData == obj)
					return true;
			}
			return false;
		}

		public static bool HitsMesh(this Ray ray, MapObjectAttachedMesh mesh, float maxDistance = 2048)
		{
			ray.Direction = ray.Direction.GetNormalize() * maxDistance;
			Body bod = mesh.Body;
			if (bod == null)
				bod = mesh.CollisionBody;
			MapObject parentObj = mesh.Owner;
			RayCastResult[] piercingResult = PhysicsWorld.Instance.RayCastPiercing(ray, (int)ContactGroup.CastAll);
			foreach (var res in piercingResult)
			{
				if (res.Distance < maxDistance && res.Shape.Body._InternalUserData != null && res.Shape.Body._InternalUserData == parentObj)
				{
					if (Array.Find(bod.Shapes, s2 => s2 == res.Shape) != null)
						return true;
				}
			}
			return false;
		}
	}
}
