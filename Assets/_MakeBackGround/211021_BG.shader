Shader "Unlit/211021_BG"
{
  Properties
  {
    _Id ("ID", Int) = 0
    _Color ("Color", Color) = (1,1,1,1)
  }
  SubShader
  {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      // make fog work
      #pragma multi_compile_fog

      #include "UnityCG.cginc"

      struct appdata
      {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };

      struct v2f
      {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      v2f vert (appdata v)
      {
        v2f o;
        o.vertex = float4( (-1. + v.uv * 2.) * float2(1, -1) , 1, 1);
        o.uv = v.uv;
        return o;
      }

      uint _Id;

      float4 _Color;

      float3 image0(float2 uv, float2 aspect)
      {
        float2 p = (-1. + 2. * uv) * aspect;

        float2 q = p;
        q.y = -0.95 + abs(q.y);
        float spacex = 1.2;
        q.x -= clamp(q.x, -spacex, spacex);
        q.x /= 3;
        float l = length(q);
        float i = saturate( 0.01 / abs(l) );

        return _Color.rgb * _Color.a * i;
      }

      float3 image1(float2 uv, float2 aspect)
      {
        return float3(uv, 0);
      }

      float dark_layer(float2 uv, float2 aspect)
      {
        float2 p = (-1. + 2. * uv) * aspect;
        float2 b = aspect-0.2;
        p -= clamp(p, -b, b);
        return lerp(0.0, 0.6, saturate( 1-length(p)*3. ));
      }

      fixed4 frag (v2f i) : SV_Target
      {
        float2 aspect = _ScreenParams.xy / min(_ScreenParams.x, _ScreenParams.y);
        float3 col = 0;

        col += image0(i.uv, aspect) * (_Id == 0);
        col += image1(i.uv, aspect) * (_Id == 1);

        col *= 1-dark_layer(i.uv, aspect);

        return float4(col, 1);
      }
      ENDCG
    }
  }
}
