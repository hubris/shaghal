float4x4 MVP;
float4x4 TexGenMatrix;

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 TexCoord : TEXCOORD0;
};

/*------------------------------------------------------------------*/
VertexShaderOutput VertexGenEntryExit(float4 Position : POSITION0)
{
    VertexShaderOutput output;

    output.Position = mul(Position, MVP);    
    output.TexCoord = mul(Position, TexGenMatrix);
    
    return output;
}

float4 PixelGenEntryExit(VertexShaderOutput input) : COLOR0
{
  float3 p = input.TexCoord.xyz;
  
  return float4(p.xyz, 1.0);
}

technique VolumeRayCastGenerateRay
{
    pass Pass1
    {
      AlphaBlendEnable = FALSE;
      CullMode = CW;
      
      VertexShader = compile vs_3_0 VertexGenEntryExit();
      PixelShader = compile ps_3_0 PixelGenEntryExit();
    }
    
    pass Pass2
    {
      AlphaBlendEnable = FALSE;
      CullMode = CCW;
      
      VertexShader = compile vs_3_0 VertexGenEntryExit();
      PixelShader = compile ps_3_0 PixelGenEntryExit();
    }
}