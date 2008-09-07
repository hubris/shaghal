float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 TexGenMatrix;
float4 CamPosTexSpace;
float StepSize;

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

//Volume rendering with preintegration
float4 PixelMainPreIntegration(VertexShaderOutput input) : COLOR0
{
  float3 p = input.TexCoord.xyz;
  float3 rDir = normalize(input.TexCoord-CamPosTexSpace).xyz;
  float4 dest = float4(0, 0, 0, 0);
  float4 voxel = float4(0, 0, 0, 0);
  float3 stepDir = StepSize*rDir;

  bool inTexture = (p.x>=0&&p.x<=1)&&(p.y>=0&&p.y<=1)&&(p.z>=0&&p.z<=1);
  while(inTexture && dest.w<0.95)
  {
    while(inTexture && dest.w<0.95)
    {      
      voxel.x = tex3Dlod(volumeSampler, float4(p.xyz, 0)).w;      
      float4 src = tex2Dlod(tfPreIntSampler, voxel);      
      voxel.y = voxel.x; 
      dest = dest+(1-dest.w)*src;   
      p += stepDir;
      inTexture = (p.x>=0&&p.x<=1)&&(p.y>=0&&p.y<=1)&&(p.z>=0&&p.z<=1);            
    }
  }

  return dest;  
}

//Volume rendering without preintegration
float4 PixelMainNoPreIntegration(VertexShaderOutput input) : COLOR0
{
  float3 p = input.TexCoord.xyz;
  float3 rDir = normalize(input.TexCoord-CamPosTexSpace).xyz;
  float4 dest = float4(0, 0, 0, 0);
  float3 stepDir = StepSize*rDir;

  bool inTexture = (p.x>=0&&p.x<=1)&&(p.y>=0&&p.y<=1)&&(p.z>=0&&p.z<=1);
  while(inTexture && dest.w<0.95)
  {
    while(inTexture && dest.w<0.95)
    {      
      float s = tex3Dlod(volumeSampler, float4(p.xyz, 0)).w;            
      float4 src = tex1Dlod(tfSampler, float4(s, 0, 0, 0));      
      dest = dest+(1-dest.w)*src;      
      p += stepDir;
      inTexture = (p.x>=0&&p.x<=1)&&(p.y>=0&&p.y<=1)&&(p.z>=0&&p.z<=1);            
    }
  }

  return dest;  
}

technique VolumeRayCastPreIntegration
{
    pass Pass1
    {
      AlphaBlendEnable = TRUE;
      SrcBlend = SRCALPHA;
      DestBlend = INVSRCALPHA;

      VertexShader = compile vs_3_0 VertexMain();
      PixelShader = compile ps_3_0 PixelMainPreIntegration();
    }
}

technique VolumeRayCastNoPreIntegration
{
    pass Pass1
    {
      AlphaBlendEnable = TRUE;
      SrcBlend = SRCALPHA;
      DestBlend = INVSRCALPHA;

      VertexShader = compile vs_3_0 VertexMain();
      PixelShader = compile ps_3_0 PixelMainNoPreIntegration();
    }
}
