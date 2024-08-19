Shader "SoftHair" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0)
        _MainTex ("Base (RGB)", 2D) = "white" { }
	
		_Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
    }

	
	Category 
	{
		Lighting Off
    SubShader {
	
 Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
 
	
        Material {
            Diffuse [_Color]
        }

Pass {
            AlphaTest Greater [_Cutoff]
            SetTexture [_MainTex] {
                combine texture * primary, texture
            }
        }


       UsePass "Transparent/Diffuse/FORWARD"
        } 
		
    }

}