// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Triplanar Color Gradient Lit"
{
	Properties
	{
		_ColorSide("Color Side", Color) = (1,0,0,0)
		_ColorTop("Color Top", Color) = (0.05236471,1,0,0)
		_ColorFront("Color Front", Color) = (0,0.2136707,1,0)
		_ColorGradient("Color Gradient", Color) = (1,1,1,0)
		[Toggle(_GRADIENTBYWORLDORUV_ON)] _GradientbyworldorUV("Gradient by world or UV", Float) = 1
		_GradientOffset("Gradient Offset", Range( 0 , 1)) = 0.1
		_GradientAmount("Gradient Amount", Range( 0 , 1)) = 0.5
		_GradientFalloff("Gradient Falloff", Range( 0 , 10)) = 0.3
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _GRADIENTBYWORLDORUV_ON
		struct Input
		{
			float3 worldNormal;
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 _ColorGradient;
		uniform float4 _ColorSide;
		uniform float4 _ColorTop;
		uniform float4 _ColorFront;
		uniform float _GradientOffset;
		uniform float _GradientAmount;
		uniform float _GradientFalloff;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float4 lerpResult32 = lerp( float4( 0,0,0,0 ) , _ColorSide , abs( ase_worldNormal.x ));
			float4 lerpResult33 = lerp( float4( 0,0,0,0 ) , _ColorTop , abs( ase_worldNormal.y ));
			float4 lerpResult34 = lerp( float4( 0,0,0,0 ) , _ColorFront , abs( ase_worldNormal.z ));
			float3 ase_worldPos = i.worldPos;
			#ifdef _GRADIENTBYWORLDORUV_ON
				float staticSwitch94 = ase_worldPos.y;
			#else
				float staticSwitch94 = i.uv_texcoord.y;
			#endif
			float4 lerpResult75 = lerp( _ColorGradient , ( lerpResult32 + ( lerpResult33 + lerpResult34 ) ) , saturate( pow( ( ( staticSwitch94 * _GradientOffset ) + _GradientAmount ) , ( 10.0 * _GradientFalloff ) ) ));
			o.Albedo = lerpResult75.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
709;396;1744;1157;2986.364;-1524.799;1.395353;True;False
Node;AmplifyShaderEditor.CommentaryNode;64;-2344.69,-49.825;Inherit;False;1752.279;1329.213;Triplanar colors;15;24;41;39;20;21;37;19;33;34;32;35;36;16;18;17;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;83;-1845.415,1710.618;Inherit;False;1232.61;570.7927;gradient values;9;92;91;90;89;88;87;86;85;84;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;93;-2668.881,1903.25;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;82;-2715.122,1758.215;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;24;-2294.69,621.1994;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;94;-2383.161,1762.746;Inherit;False;Property;_GradientbyworldorUV;Gradient by world or UV;4;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-1806.922,2183.608;Inherit;False;Property;_GradientOffset;Gradient Offset;5;0;Create;True;0;0;0;False;0;False;0.1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-1085.828,2180.514;Inherit;False;Property;_GradientFalloff;Gradient Falloff;7;0;Create;True;0;0;0;False;0;False;0.3;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1440.972,2184.243;Inherit;False;Property;_GradientAmount;Gradient Amount;6;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-1766.5,1765.549;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-1088.542,2023.11;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;20;-1631.808,464.246;Inherit;False;Property;_ColorTop;Color Top;1;0;Create;True;0;0;0;False;0;False;0.05236471,1,0,0;0.05236471,1,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;39;-1630.855,663.0567;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;-1619.448,950.0457;Inherit;False;Property;_ColorFront;Color Front;2;0;Create;True;0;0;0;False;0;False;0,0.2136707,1,0;0,0.2136707,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;41;-1623.744,1145.868;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;34;-1317.171,943.1567;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;33;-1326.328,468.8846;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.AbsOpNode;37;-1629.849,204.8915;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;19;-1631.014,1.54981;Inherit;False;Property;_ColorSide;Color Side;0;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-1400.604,1766.788;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-892.543,2022.11;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-947.7755,468.6581;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;91;-1094.129,1766.902;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;32;-1319.06,0.724432;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;51;-832.7487,1480.109;Inherit;False;Property;_ColorGradient;Color Gradient;3;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;92;-787.805,1760.618;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-746.4117,0.1749935;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-332.9787,347.9489;Inherit;False;Property;_Smoothness;Smoothness;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;75;-346.1832,2.456665;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;85.15906,-0.9075487;Float;False;True;-1;2;;0;0;Standard;TidalFlask/Triplanar Color Gradient Lit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;16;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;17;-1776.332,956.3713;Inherit;False;100;100;front;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;18;-1789.475,5.201062;Inherit;False;100;100;side;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;16;-1787.501,468.1703;Inherit;False;100;100;top;0;;1,1,1,1;0;0
WireConnection;94;1;82;2
WireConnection;94;0;93;2
WireConnection;88;0;94;0
WireConnection;88;1;84;0
WireConnection;39;0;24;2
WireConnection;41;0;24;3
WireConnection;34;1;21;0
WireConnection;34;2;41;0
WireConnection;33;1;20;0
WireConnection;33;2;39;0
WireConnection;37;0;24;1
WireConnection;89;0;88;0
WireConnection;89;1;86;0
WireConnection;90;0;85;0
WireConnection;90;1;87;0
WireConnection;35;0;33;0
WireConnection;35;1;34;0
WireConnection;91;0;89;0
WireConnection;91;1;90;0
WireConnection;32;1;19;0
WireConnection;32;2;37;0
WireConnection;92;0;91;0
WireConnection;36;0;32;0
WireConnection;36;1;35;0
WireConnection;75;0;51;0
WireConnection;75;1;36;0
WireConnection;75;2;92;0
WireConnection;0;0;75;0
WireConnection;0;4;1;0
ASEEND*/
//CHKSM=132242CE356AB112592C7F0189135D5D29164B51