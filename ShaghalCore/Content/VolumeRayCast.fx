float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 TexGenMatrix;
float StepSize;

float3 LightDir;

//1/texSize
float4 DeltaTex;

float SamplingRate;

float Alpha;

texture VolTexture;
texture TransferFunctionPreInt;

sampler  EntryPointSampler  : register(s0);

texture ExitPointTexture;
sampler2D ExitPointSampler = sampler_state
{
    Texture = <ExitPointTexture>;
    MagFilter = POINT; 
    MinFilter = POINT; 
    AddressU = CLAMP;
    AddressV = CLAMP;
};

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
};

float4 computeGradient(float3 tCoord)
{
	float p011 = tex3Dlod(volumeSampler, float4(tCoord, 0)-DeltaTex.xwww).w;
	float p211 = tex3Dlod(volumeSampler, float4(tCoord, 0)+DeltaTex.xwww).w;

	float p101 = tex3Dlod(volumeSampler, float4(tCoord, 0)-DeltaTex.wyww).w;
	float p121 = tex3Dlod(volumeSampler, float4(tCoord, 0)+DeltaTex.wyww).w;

	float p110 = tex3Dlod(volumeSampler, float4(tCoord, 0)+DeltaTex.wwzw).w;
	float p112 = tex3Dlod(volumeSampler, float4(tCoord, 0)-DeltaTex.wwzw).w;
	
	float4 G = float4(p211-p011, p121-p101, p112-p110, 0);
	return float4(G.xyz, length(G.xyz));	
}

float computeDiffuse(float4 grad)
{
	//float3 l = mul(LightDir, World);	
	float3 l = LightDir;
	l = mul(l, View);
    float3 n = l;    
	if(grad.w > 0.001)
		n =	grad.xyz/grad.w;
	return dot(l, n);
}

//Volume rendering with preintegration
float4 PixelMainPreIntegration(float2 texCoord : TEXCOORD0) : COLOR0
{
  float4 entryPoint = tex2D(EntryPointSampler, texCoord.xy);
  float4 exitPoint = tex2D(ExitPointSampler, texCoord.xy);
  float3 p = entryPoint.xyz;
  float3 rDir = exitPoint.xyz-entryPoint.xyz;
  float rLength = length(exitPoint.xyz-entryPoint.xyz);
  rDir /= rLength;
  
  float4 dest = float4(0, 0, 0, 0);
  float4 voxel = float4(0, 0, 0, 0);
  float3 stepDir = StepSize*rDir;

  float t = 0;
  float srate = SamplingRate;
  bool inTexture = t < rLength;
  while(inTexture && dest.w<0.95)
  {
    while(inTexture && dest.w<0.95)
    {      
      voxel.x = tex3Dlod(volumeSampler, float4(p.xyz, 0)).w;
      float4 src = tex2Dlod(tfPreIntSampler, voxel);    
      float4 grad = computeGradient(p.xyz+stepDir*0.5);
      src.xyz *= computeDiffuse(grad);
	  src.w *= Alpha*srate;
      dest.xyz = dest.xyz+(1-dest.w)*src.xyz*src.w;      
      dest.w = dest.w+(1-dest.w)*src.w;      

      voxel.y = voxel.x; 
      p += stepDir;
      t += StepSize;
      inTexture = t < rLength;
    }
  }
  return dest;  
}


//Volume rendering without preintegration
float4 PixelMainNoPreIntegration(float2 texCoord : TEXCOORD0) : COLOR0
{
  float4 entryPoint = tex2D(EntryPointSampler, texCoord.xy);
  float4 exitPoint = tex2D(ExitPointSampler, texCoord.xy);

  float3 p = entryPoint.xyz;
  float3 rDir = exitPoint.xyz-entryPoint.xyz;
  float rLength = length(exitPoint.xyz-entryPoint.xyz);
  rDir /= rLength;
  float4 dest = float4(0, 0, 0, 0);
  float3 stepDir = StepSize*rDir;

  float t = 0;
  float srate = SamplingRate;
  float3 sdir = stepDir;
  bool inTexture = t<rLength;
  while(inTexture && dest.w<0.95)
  {
    while(inTexture && dest.w<0.95)
    {      
       //if ( t>0.5) {
		//sdir = StepSize*rDir*4.;
		//srate = SamplingRate*4.;
		//}
	  float4 grad = computeGradient(p.xyz+stepDir*0.5);
      float s = tex3Dlod(volumeSampler, float4(p.xyz, 0)).w;            
      float4 src = tex1Dlod(tfSampler, float4(s, 0, 0, 0));      
      src.xyz *= computeDiffuse(grad);

	  src.w *= Alpha*srate;
      dest.xyz = dest.xyz+(1-dest.w)*src.xyz*src.w;      
      dest.w = dest.w+(1-dest.w)*src.w;      
      p += sdir;
      t += StepSize;      
      inTexture = t<rLength;            
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

      //VertexShader = compile vs_3_0 VertexMain();
      PixelShader = compile ps_3_0 PixelMainNoPreIntegration();
    }
}
