float4x4 MVPI;
float4x4 TexGenMatrix;

// Input parameters.
texture ExitPointTexture;

// Input parameters.
sampler  TextureSampler  : register(s0);
sampler TextureSamplerExit = sampler_state
{
    Texture = <ExitPointTexture>;
    MagFilter = POINT; 
    MinFilter = POINT; 
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
};


float4 SpritePixelMain(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 entry = tex2D(TextureSampler, texCoord);
    float4 exit = tex2D(TextureSamplerExit, texCoord);
	if(entry.w == 0 && exit.w != 0) {
		float4 ndc = float4(texCoord*2.-float2(1, 1), 0., 1.);
		ndc.y = 1.-ndc.y;
		float4 pos = mul(ndc, MVPI);
		pos /= pos.w;	
		entry = mul(pos, TexGenMatrix);			
	}
	return entry; 
}

technique VolumeRayCastClipEntryPoint
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 SpritePixelMain();
    }
}