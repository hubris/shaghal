float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 TexGenMatrix;
float4 CamPosTexSpace;

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);    
    output.TexCoord = mul(input.Position, TexGenMatrix);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
  float4 rDir = normalize(input.TexCoord-CamPosTexSpace);
  return float4(rDir.xyz, 1.);
}

technique Technique1
{
    pass Pass1
    {

        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
