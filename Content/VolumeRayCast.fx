float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 TexGenMatrix;
float4 CamPosTexSpace;

float Alpha;

texture VolTexture;

sampler volumeSampler = sampler_state
{
    Texture = <VolTexture>;
    MagFilter = LINEAR; 
    MinFilter = LINEAR; 
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexMain(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);    
    output.TexCoord = mul(input.Position, TexGenMatrix);

    return output;
}

float4 PixelMain(VertexShaderOutput input) : COLOR0
{
  float3 p = input.TexCoord.xyz;
  float3 rDir = normalize(input.TexCoord-CamPosTexSpace).xyz;
  float4 dest = float4(0, 0, 0, 0);
  float stepSize = 1./512;

  rDir = float3(0, 0, 1);
  stepSize = 0;
  while(dest.w<0.95)
  {
    float s = tex3Dlod(volumeSampler, float4(p.xyz, 0)).w;
    s = s>0.6?0:s;
    float4 src = float4(s, s, s, s);
    dest.xyz = dest.xyz+(1-dest.w)*src.w*src.xyz;
    dest.w = dest.w+(1-dest.w)*src.w;
    p += rDir*stepSize;
  }
  return dest;  
}

technique VolumeRayCast
{
    pass Pass1
    {
      AlphaBlendEnable = TRUE;
      SrcBlend = INVSRCALPHA;
      DestBlend = INVSRCALPHA;

      VertexShader = compile vs_2_0 VertexMain();
      PixelShader = compile ps_3_0 PixelMain();
    }
}
