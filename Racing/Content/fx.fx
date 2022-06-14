#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
Texture2D tex;
sampler2D texsamp = sampler_state
{
	Texture = <tex>;
};

struct VertexShaderInput
{
	float4 pos : POSITION0;
	float4 col : COLOR0;
	float2 tex : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 pos : SV_POSITION;
	float4 col : COLOR0;
	float2 tex : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.pos = mul(input.pos, WorldViewProjection);
	output.col = input.col;
	output.tex = input.tex;

	return output;
}

float4 mainPS(VertexShaderOutput input) : COLOR
{
	float4 col = input.col;
	return col;
}
float4 pistaPS(VertexShaderOutput input) : COLOR
{
	float2 pos = input.tex -.5;
	float f = (1 - (pos.y + .5));// *.5;
	pos.x = pos.x * f;// +(1. - f) / 2.;
	float d = abs(abs(length(float2(pos.x, pos.y - clamp(pos.y,-0.2, .2))) - .25) - .03) - .001;
	float4 col = input.col;
	col.a = smoothstep(.001, .0, d);
	return col;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL mainPS();
	}
};
technique Pista
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL pistaPS();
	}
};