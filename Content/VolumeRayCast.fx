float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 TexGenMatrix;
float4 CamPosTexSpace;

float Alpha;

texture VolTexture;
texture TransferFunctionPreInt;

sampler volumeSampler = sampler_state
{
    Texture = <VolTexture>;
    MagFilter = LINEAR; 
    MinFilter = LINEAR; 
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
};

texture TransferFunction;

sampler tfSampler = sampler_state
{
    Texture = <TransferFunction>;
    MagFilter = LINEAR; 
    MinFilter = LINEAR; 
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler tfPreIntSampler = sampler_state
{
    Texture = <TransferFunctionPreInt>;
    MagFilter = LINEAR; 
    MinFilter = LINEAR; 
    AddressU = CLAMP;
    AddressV = CLAMP;
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
  const float stepSize = 1./512.0f;
  float4 voxel = float4(0, 0, 0, 0);
  float3 stepDir = stepSize*rDir;

  bool inTexture = (p.x>=0&&p.x<=1)&&(p.y>=0&&p.y<=1)&&(p.z>=0&&p.z<=1);
  while(inTexture && dest.w<0.95)
  {
    while(inTexture && dest.w<0.95)
    {      
      voxel.x = tex3Dlod(volumeSampler, float4(p.xyz, 0)).w;      
      float4 src = tex2Dlod(tfPreIntSampler, voxel);      
      voxel.y = voxel.x;      
      
    dest = dest+(1-dest.w)*src;   
//      dest.xyz = dest.xyz+(1-dest.w)*src.xyz*src.w;      
//     dest.w = dest.w+(1-dest.w)*src.w;      

      p += stepDir;
      inTexture = (p.x>=0&&p.x<=1)&&(p.y>=0&&p.y<=1)&&(p.z>=0&&p.z<=1);            
    }
  }

  return dest;  
}

float4 PixelMainNoPreInt(VertexShaderOutput input) : COLOR0
{
  float3 p = input.TexCoord.xyz;
  float3 rDir = normalize(input.TexCoord-CamPosTexSpace).xyz;
  float4 dest = float4(0, 0, 0, 0);
  const float stepSize = 1./64.0f;
  float3 stepDir = stepSize*rDir;

  bool inTexture = (p.x>=0&&p.x<=1)&&(p.y>=0&&p.y<=1)&&(p.z>=0&&p.z<=1);
  while(inTexture && dest.w<0.95)
  {
    while(inTexture && dest.w<0.95)
    {      
      float s = tex3Dlod(volumeSampler, float4(p.xyz, 0)).w;            
      float4 src = tex1Dlod(tfSampler, float4(s, 0, 0, 0));      
      
      dest = dest+(1-dest.w)*src;      
      //dest.xyz = dest.xyz+(1-dest.w)*src.xyz*src.w;      
      //dest.w = dest.w+(1-dest.w)*src.w;      
      p += stepDir;
      inTexture = (p.x>=0&&p.x<=1)&&(p.y>=0&&p.y<=1)&&(p.z>=0&&p.z<=1);            
    }
  }

  return dest;  
}

technique VolumeRayCast
{
    pass Pass1
    {
      AlphaBlendEnable = FALSE;
      SrcBlend = SRCALPHA;
      DestBlend = INVSRCALPHA;

      VertexShader = compile vs_3_0 VertexMain();
      PixelShader = compile ps_3_0 PixelMain();
    }
}

technique VolumeRayCastNoPreInt
{
    pass Pass1
    {
      AlphaBlendEnable = TRUE;
      SrcBlend = SRCALPHA;
      DestBlend = INVSRCALPHA;

      VertexShader = compile vs_3_0 VertexMain();
      PixelShader = compile ps_3_0 PixelMainNoPreInt();
    }
}
