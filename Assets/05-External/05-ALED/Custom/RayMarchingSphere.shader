Shader "Custom/RayMarchingLargeSphere"
{
    Properties
    {
        [Header(Ray March Settings)]
        _MaxRayDistance ("Max Ray Distance", Float) = 1000.0

        [Header(Sphere Settings)]
        _Color ("Sphere Color", Color) = (1,1,1,1)
        _SphereRadius ("Sphere Radius", Float) = 2.0
        _SpherePosition ("Sphere Position", Vector) = (0, 0, 0)
        _RotationSpeed ("Rotation Speed", Float) = 1.0

        [Header(Sun Settings)]
        _SunColor ("Sun Color", Color) = (1,1,0,1)
        _SunRadius ("Sun Radius", Float) = 1.0
        _SunPosition ("Sun Position", Vector) = (5, 5, 5)
        _OrbitSpeed ("Orbit Speed", Float) = 1.0

        [Header(Land Noise Settings)]
        _LandNoiseScale ("Land Noise Scale", Float) = 1.0
        _LandNoiseAmplitude ("Land Noise Amplitude", Float) = 0.1
        _LandNoiseTexture ("Land Noise Texture", 2D) = "white" {}

        [Header(Cloud Noise Settings)]
        _CloudNoiseScale ("Cloud Noise Scale", Float) = 1.0
        _CloudNoiseAmplitude ("Cloud Noise Amplitude", Float) = 0.1
        _CloudNoiseTexture ("Cloud Noise Texture", 2D) = "white" {}
        _CloudThreshold ("Cloud Threshold", Float) = 0.5
        _IsInverted ("Invert Noise", Range(-1,1)) = 0 
        _CloudDistance ("Cloud Distance", Float) = 0.5
        _CloudHeight ("Cloud Height", Float) = 0.1
        _InnerRadius ("Inner Radius", Float) = 1.0

        [Header(Color Settings)]
        _WaterColor ("Water Color", Color) = (0,1,0,1)
        _SandColor ("Sand Color", Color) = (1,1,0,1)
        _LandColor ("Land Color", Color) = (0.5,0.25,0,1)
        _MountainColor ("Mountain Color", Color) = (0.5,0.5,0.5,1)
        _MountainColorPeak ("Mountain Color Peak", Color) = (1,1,1,1)
        _AirColor ("Air Color", Color) = (0,0,0,0)
        _CloudColor ("Cloud Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            float _MaxRayDistance;
            float _SphereRadius;
            float4 _Color;
            float3 _SpherePosition;
            float _RotationSpeed;
            float4 _SunColor;
            float _SunRadius;
            float3 _SunPosition;
            float _OrbitSpeed;
            float _LandNoiseScale;
            float _LandNoiseAmplitude;
            sampler2D _LandNoiseTexture;
            float _CloudNoiseScale;
            float _CloudNoiseAmplitude;
            sampler2D _CloudNoiseTexture;
            float _CloudThreshold;
            float _CloudHeight;
            int _IsInverted;
            float _CloudDistance;
            float _InnerRadius;
            float4 _WaterColor;
            float4 _SandColor;
            float4 _LandColor;
            float4 _MountainColor;
            float4 _MountainColorPeak;
            float4 _AirColor;
            float4 _CloudColor;

            float3 _NoisePosBuffer;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.vertex.xy * 0.5 + 0.5; // Convertir les coordonnées en UV
                return o;
            }

            float3 RotateY(float3 pos, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                return float3(c * pos.x - s * pos.z, pos.y, s * pos.x + c * pos.z);
            }

            float3 CalculateOrbitPosition(float3 center, float radius, float speed)
            {
                float angle = speed * _Time.y;
                return center + float3(cos(angle) * radius, 0, sin(angle) * radius);
            }

            float SampleNoiseTexture(sampler2D noiseTexture, float3 pos, float scale)
            {
                float2 uv = pos.xy * scale;
                return tex2D(noiseTexture, uv).r;
            }

            float DiscreteNoise(float noise)
            {
                if (noise < 0.2) return 0.2;
                else if (noise < 0.21) return 0.3;
                else if (noise < 0.4) return 0.4;
                else if (noise < 0.5) return 0.6;
                else if (noise < 0.6) return 0.7;
                else if (noise < 0.8) return 0.8;
                else return 1.0;
            }

            float SphereSDF(float3 pos, float radius)
            {
                float3 orbitPos = CalculateOrbitPosition(_SunPosition, length(_SpherePosition), _OrbitSpeed);
                float angle = _RotationSpeed * _Time.y;
                float3 spherePos = RotateY(pos - orbitPos, angle);
                _NoisePosBuffer = spherePos - _SpherePosition;
                float noise = SampleNoiseTexture(_LandNoiseTexture, _NoisePosBuffer, _LandNoiseScale);
                float discreteNoise = _IsInverted * DiscreteNoise(noise) * _LandNoiseAmplitude;
                return length(spherePos) - (radius + discreteNoise);
            }

            float SunSDF(float3 pos, float radius)
            {
                return length(pos - _SunPosition) - radius;
            }

            float CloudSDF(float3 pos, float radius)
            {
                float3 orbitPos = CalculateOrbitPosition(_SunPosition, length(_SpherePosition), _OrbitSpeed);
                float angle = _RotationSpeed * _Time.y;
                float3 cloudPos = RotateY(pos - orbitPos, angle);
                _NoisePosBuffer = cloudPos - _SpherePosition;
                float cloudRadius = radius + _CloudDistance;
                float innerRadius = _InnerRadius;
                float cloudNoise = SampleNoiseTexture(_CloudNoiseTexture, _NoisePosBuffer, _CloudNoiseScale);

                if (cloudNoise < _CloudThreshold) return 10000.0;

                float distToSurface = length(cloudPos) - (cloudRadius + cloudNoise * _CloudNoiseAmplitude);
                float distToInnerSurface = innerRadius - length(cloudPos);

                // Retourner la distance minimale pour créer une sphère creuse
                return max(distToSurface, distToInnerSurface);
            }

            float3 CalculateNormal(float3 pos, float radius)
            {
                float2 epsilon = float2(0.01, 0);
                float3 normal = float3(
                    SphereSDF(pos + epsilon.xyy, radius) - SphereSDF(pos - epsilon.xyy, radius),
                    SphereSDF(pos + epsilon.yxy, radius) - SphereSDF(pos - epsilon.yxy, radius),
                    SphereSDF(pos + epsilon.yyx, radius) - SphereSDF(pos - epsilon.yyx, radius)
                );
                return normalize(normal);
            }

            float RayMarch(float3 ro, float3 rd, out float3 hitPos, out bool hitSun, out bool hitCloud)
            {
                float minDist = 0.001;
                float totalDist = 0.0;
                hitSun = false;
                hitCloud = false;

                for (int i = 0; i < 200; i++) // Utilisation de la nouvelle propriété
                {
                    hitPos = ro + totalDist * rd;
                    float distSphere = SphereSDF(hitPos, _SphereRadius);
                    float distSun = SunSDF(hitPos, _SunRadius);
                    float distCloud = CloudSDF(hitPos, _SphereRadius);
                    float dist = min(min(distSphere, distSun), distCloud);
                    if (dist < minDist)
                    {
                        hitSun = (dist == distSun);
                        hitCloud = (dist == distCloud);
                        return totalDist;
                    }
                    if (totalDist > _MaxRayDistance) break;
                    totalDist += dist;
                }
                return -1.0; // No hit
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.worldPos - _WorldSpaceCameraPos);

                float3 hitPos;
                bool hitSun;
                bool hitCloud;
                float t = RayMarch(rayOrigin, rayDir, hitPos, hitSun, hitCloud);

                if (t > 0.0)
                {
                    if (hitSun)
                    {
                        return _SunColor;
                    }
                    else if (hitCloud)
                    {
                        return _CloudColor;
                    }
                    else
                    {
                        float3 normal = CalculateNormal(hitPos, _SphereRadius);
                        float3 lightDir = normalize(_SunPosition - hitPos);
                        float diff = max(dot(normal, lightDir), 0.0);

                        float landNoise = SampleNoiseTexture(_LandNoiseTexture, _NoisePosBuffer, _LandNoiseScale);
                        float discreteNoise = DiscreteNoise(landNoise);
                        float3 color;

                        if (discreteNoise == 0.2) color = _WaterColor.rgb;
                        else if (discreteNoise == 0.3) color = _SandColor.rgb;
                        else if (discreteNoise == 0.4) color = _LandColor.rgb;
                        else if (discreteNoise == 0.6) color = _MountainColor.rgb;
                        else if (discreteNoise == 0.7) color = _MountainColorPeak.rgb;
                        else if (discreteNoise == 0.8) color = _AirColor.rgb;
                        else color = _AirColor.rgb;

                        return float4(color * diff, 1.0);
                    }
                }

                // Retourner une couleur noire pour le fond
                return float4(0.0, 0.0, 0.0, 1.0);
            }
            ENDCG
        }
    }
}
