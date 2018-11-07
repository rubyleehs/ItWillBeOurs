using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Light2D : ShadowCaster {

    public Color color;

	Mesh mesh;
	new MeshRenderer renderer;
    
    public float falloffExponent = 1.0f;
    public float angleFalloffExponent = 1.0f;

    float _radius = 10f;
    public float radius {

        get {

            return _radius;

        }

        set {

            _radius = value;
            RebuildQuad();

        }

    }

    float _spread = 360;
    public float spread {
        
		get {
            
			return _spread;
        
		}
        
		set {
            
			_spread = value;
            RebuildQuad();

        }

    }

	public void Start () {
		
        // transform.localScale = Vector3.one;
        RebuildQuad();

    }

    // Build the light's quad mesh. This aims to fit the light cone as best as possible.
    public void RebuildQuad () {

        if (mesh == null) mesh = GetComponent<MeshFilter>().mesh;

        List<Vector3> verts = new List<Vector3>();

		if (spread > 180.0f) {

			verts.Add(new Vector3(-radius,-radius));
			verts.Add(new Vector3(radius,radius));
			verts.Add(new Vector3(radius,-radius));
			verts.Add(new Vector3(-radius,radius));

        } else {
            
			float minAngle = -spread * 0.5f;
			float maxAngle = spread * 0.5f;

            Bounds aabb = new Bounds(Vector3.zero, Vector3.zero);
            aabb.Encapsulate(new Vector3(radius, 0.0f));
            aabb.Encapsulate(new Vector3(Mathf.Cos(Mathf.Deg2Rad * minAngle), Mathf.Sin(Mathf.Deg2Rad * minAngle)) * radius);
            aabb.Encapsulate(new Vector3(Mathf.Cos(Mathf.Deg2Rad * maxAngle), Mathf.Sin(Mathf.Deg2Rad * maxAngle)) * radius);

            verts.Add(new Vector3(aabb.min.x, aabb.min.y));
            verts.Add(new Vector3(aabb.max.x, aabb.max.y));
            verts.Add(new Vector3(aabb.max.x, aabb.min.y));
            verts.Add(new Vector3(aabb.min.x, aabb.max.y));
        }

        mesh.SetVertices(verts);
        mesh.RecalculateBounds();
    }

    // This function sets up the parameters needed to DRAW the shadow map, plus the parameters to USE the shadow map when this object is rendered.
    public override MaterialPropertyBlock BindShadowMap (Texture shadowMapTexture) {

        if (renderer == null) renderer = GetComponent<MeshRenderer>();

        float angle = transform.eulerAngles.z;
		Vector4 shadowMapParams = GetShadowMapParams(mShadowMapSlot);

		mMaterialPropertyBlock.SetVector("_LightPosition",new Vector4(transform.position.x,transform.position.y,
			angle * Mathf.Deg2Rad,spread * Mathf.Deg2Rad * 0.5f));
		
        mMaterialPropertyBlock.SetVector("_ShadowMapParams",shadowMapParams);

		Material mat = renderer.materials[0];

		mat.SetVector("_Color", color);

		mat.SetVector("_LightPosition",new Vector4(transform.position.x,transform.position.y,falloffExponent,angleFalloffExponent));

		mat.SetVector("_Params2",new Vector4(angle * Mathf.Deg2Rad,spread * Mathf.Deg2Rad * 0.5f,
                                             1f / radius,radius));
		
        mat.SetVector("_ShadowMapParams", shadowMapParams);

        mat.SetTexture("_ShadowTex", shadowMapTexture);
			
        return mMaterialPropertyBlock;

    }

}
